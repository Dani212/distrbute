using System.Text;
using App.Distrbute.Api.Common.Authentication;
using App.Distrbute.Api.Common.Dtos.auth;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common;
using App.Distrbute.Common.Exceptions;
using App.Distrbute.Common.Models;
using DataProtection.Sdk.Core;
using Ledgr.Sdk.Dtos;
using Ledgr.Sdk.Dtos.Client.Create;
using Ledgr.Sdk.Exceptions;
using Ledgr.Sdk.Services.Interfaces;
using Messaging.Sdk.Mail.Core;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using Persistence.Sdk.Core.Interfaces;
using Redis.Sdk.Exceptions;
using Redis.Sdk.Repositories.Interfaces;
using SendGrid.Helpers.Mail;
using Utility.Sdk;
using Utility.Sdk.Dtos;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Services.Providers;

public class AuthenticationService : IAuthenticationService
{
    private readonly BearerAuthHandler _bearerAuthHandler;
    private readonly IDbRepository _dbRepository;
    private readonly IEmailClient _emailClient;
    private readonly IRedisRepository _redisRepository;
    private readonly ISavingsClientSdk _savingsClientSdk;
    private readonly MailTemplateConfig  _mailTemplateConfig;
    private readonly IDataProtectionService _dataProtectionService;

    public AuthenticationService(
        IOptions<MailTemplateConfig> mailTemplateConfig,
        IEmailClient emailClient,
        BearerAuthHandler bearerAuthHandler,
        IRedisRepository redisRepository,
        IDbRepository dbRepository,
        ISavingsClientSdk savingsClientSdk,
        IDataProtectionService dataProtectionService)
    {
        _mailTemplateConfig = mailTemplateConfig.Value;
        _emailClient = emailClient;
        _bearerAuthHandler = bearerAuthHandler;
        _redisRepository = redisRepository;
        _dbRepository = dbRepository;
        _savingsClientSdk = savingsClientSdk;
        _dataProtectionService = dataProtectionService;
    }

    public async Task<IApiResponse<GeneratedOtp>> SendOtpAsync(LoginRequest request)
    {
        // Use current trace ID as request ID
        var traceId = TracerProvider.Default.GetTracer(name: null).StartActiveSpan("send-otp").Context.TraceId
            .ToString();

        // Generate OTP and prefix
        var random = new Random();
        var otpCode = random.Next(100000, 999999).ToString();
        var otpPrefixCode = random.Next(1000, 9999).ToString();

        var otpPrefixBuilder = new StringBuilder();
        foreach (var c in otpPrefixCode) otpPrefixBuilder.Append((char)(int.Parse(c.ToString()) + 'A'));
        var otpPrefix = otpPrefixBuilder.ToString();

        // Get existing email if it does
        var existingUser = await _dbRepository
            .GetAsync<Email>(b => b.Where(e => !e.IsDeleted && e.Address == request.Email))
            .IgnoreAndDefault<NotFound, Email>();

        var generatedOtp = new GeneratedOtp
        {
            RequestId = traceId,
            VerificationId = Guid.NewGuid().ToString(),
            Email = Core.ResolveValue(existingUser?.Address, request.Email),
            OtpPrefix = otpPrefix
        };

        // if it's not an existing user, and name not provided, we return without sending OTP
        // to prevent enumeration attacks
        if (existingUser == null && string.IsNullOrWhiteSpace(request.Name))
            throw new BadRequest($"User with email {request.Email} not found. Did you forget to sign up?");

        // cache otp
        var ttl = TimeSpan.FromMinutes(5);
        var cacheKey = $"{CommonConstants.RedisKeys.OtpCacheKey}:{request.Email}";

        var otpCache = new GeneratedOtpCachePayload();
        otpCache.RequestId = generatedOtp.RequestId;
        otpCache.VerificationId = generatedOtp.VerificationId;
        otpCache.Email = generatedOtp.Email;
        otpCache.OtpPrefix = generatedOtp.OtpPrefix;
        otpCache.Name = Core.ResolveValue(existingUser?.Name, request.Name ?? string.Empty);
        var hashedOtpCode = _dataProtectionService.Hash(otpCode);
        otpCache.OtpCode = hashedOtpCode;
        otpCache.ExistingUser = existingUser != null;

        // cache in Redis
        await _redisRepository.SetAsync(cacheKey, otpCache, ttl)
            .CatchAndThrowAs<RedisOperationException, FailedDependency>
                ("We're having trouble generating your OTP. Please try again later.");

        // Prepare email
        var templateData = new Dictionary<string, string>();
        templateData.Add("name", otpCache.Name);
        templateData.Add("otpPrefix", otpPrefix);
        templateData.Add("otpCode", otpCode);
        templateData.Add("ttl", $"{ttl.Minutes}");
        templateData.Add("year", DateTime.UtcNow.Year.ToString());

        var personalization = new Personalization();
        personalization.Subject = "🔐 Your OTP Code for Distrbute — Don’t Share It!";
        templateData.Add("subject", personalization.Subject);
        personalization.Tos = new List<EmailAddress> { new(request.Email) };
        personalization.TemplateData = templateData;

        var mail = new BulkEmail();
        mail.Subject = personalization.Subject;
        mail.TemplateId = _mailTemplateConfig.OtpTemplateId;
        mail.Personalizations = [personalization];

        // Send mail
        await _emailClient
            .SendBulkAsync(mail)
            .CatchAndRethrow<FailedDependency>("We're having trouble sending your OTP. Please try again later.");

        return generatedOtp.ToOkApiResponse();
    }

    public async Task<IApiResponse<VerifiedOtpResponse>> VerifyOtpAsync(OtpVerificationRequest otp)
    {
        // Get OTP from cache
        var cacheKey = $"{CommonConstants.RedisKeys.OtpCacheKey}:{otp.Email}";

        // Attempt to fetch otp from cache
        var cachedOtp =
            await _redisRepository.GetAsync<GeneratedOtpCachePayload>(cacheKey)
                .CatchAndThrowAsOrReturn<RedisOperationException, Unauthorized, GeneratedOtpCachePayload>
                    ("Invalid OTP code.");

        // Verify OTP
        await cachedOtp.IsValid(otp, _dataProtectionService)
            .ThrowIfFalse<Unauthorized>("Invalid OTP code.");

        // Get or create new user
        var existingUser = await _dbRepository
            .GetAsync<Email>(b => b.Where(e => !e.IsDeleted && e.Address == otp.Email))
            .OnExceptionRecover<NotFound, Email>(async () =>
            {
                // create client on fineract
                var id = Guid.NewGuid().ToString();
                var createClientReq = new CreateClientRequest();
                createClientReq.FullName = otp.Email;
                createClientReq.AccountId = id;

                var createClientRes = await _savingsClientSdk
                    .CreateAsync(createClientReq)
                    .CatchAndRethrowOrReturn<LedgerException, BaseFineractResponseDto>(
                        "Failed to create savings client for new user, please try again later");

                var newUser = new Email
                {
                    Id = id,
                    Name = cachedOtp.Name!, // this will be checked at send-otp that it's not null for new users
                    Address = otp.Email,
                    LedgerClientId = createClientRes.ClientId
                };

                var saved = await _dbRepository
                    .AddAsync(newUser)
                    .CatchAndRethrowOrReturn<FailedDependency, Email>(
                        "Failed to persist new user, please try again later");

                return saved;
            });

        // Generate token
        var (token, expiryMillis) = _bearerAuthHandler.GenerateJwtToken(existingUser);

        // Delete OTP so it cannot be reused
        await _redisRepository
            .DeleteAsync(cacheKey)
            .Ignore<RedisOperationException>();

        // Prepare response
        var response = new VerifiedOtpResponse
        {
            Email = otp.Email,
            Token = token,
            ExpirationMillis = expiryMillis
        };

        return response.ToOkApiResponse();
    }
}
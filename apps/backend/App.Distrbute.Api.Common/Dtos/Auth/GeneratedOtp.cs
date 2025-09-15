using Redact.Sdk.Attributes;

namespace App.Distrbute.Api.Common.Dtos.auth;

public class GeneratedOtp
{
    public string RequestId { get; set; } = null!;
    public string VerificationId { get; set; } = null!;
    public string OtpPrefix { get; set; } = null!;
    public string Email { get; set; } = null!;
}

[Redactable]
public class GeneratedOtpCachePayload : GeneratedOtp
{
    [Redacted] public string OtpCode { get; set; } = null!;
    public string? Name { get; set; }
    public bool ExistingUser { get; set; }
}

public static class CacheExtensions
{
    public static bool IsValid(this GeneratedOtpCachePayload cachePayload, OtpVerificationRequest request)
    {
        var isValid = cachePayload.RequestId.Equals(request.RequestId) &&
                      cachePayload.VerificationId.Equals(request.VerificationId) &&
                      cachePayload.OtpPrefix.Equals(request.OtpPrefix) &&
                      cachePayload.Email.Equals(request.Email) &&
                      cachePayload.OtpCode.Equals(request.OtpCode);

        return isValid;
    }
}
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Distrbute.Api.Common.Authentication;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly BasicAuthConfig _authConfig;
    private static readonly char[] Delimiters = new[]
    {
        ':'
    };

    public BasicAuthHandler
    (
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptionsMonitor<BasicAuthConfig> authOptions,
        IOptionsMonitor<AuthenticationSchemeOptions> options
    ) : base(options, logger, encoder)
    {
        _authConfig = authOptions.CurrentValue;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (string.IsNullOrWhiteSpace(Request.Headers.Authorization)) return AuthenticateResult.Fail("Missing Authorization Header");

        await Task.CompletedTask;

        var passed = false;
        try
        {
            var auth = AuthenticationHeaderValue.Parse(Request.Headers.Authorization!);

            if (auth.Parameter is null || !CommonConstants.AuthScheme.BASIC.Equals(auth.Scheme, StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.NoResult();
            var bytes = Convert.FromBase64String(auth.Parameter);
            var credentials = Encoding.UTF8.GetString(bytes).Split(Delimiters, 2);
            var username = credentials[0];
            var password = credentials[1];


            if (IsValid(username, password)) passed = true;
            
            if (!passed)
                return AuthenticateResult.Fail("Invalid credentials");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, username), new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }

    private bool IsValid(string username, string password)
    {
        return _authConfig.Username == username && _authConfig.Password == password;
    }
}

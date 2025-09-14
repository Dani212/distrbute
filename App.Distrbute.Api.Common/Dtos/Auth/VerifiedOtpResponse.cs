using Redact.Sdk.Attributes;

namespace App.Distrbute.Api.Common.Dtos.auth;

[Redactable]
public class VerifiedOtpResponse
{
    public string Email { get; set; } = null!;
    [Redacted] public string Token { get; set; } = null!;
    public long ExpirationMillis { get; set; }
}
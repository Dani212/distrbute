using System.ComponentModel.DataAnnotations;
using Paystack.Sdk.Enums;
using Redact.Sdk.Attributes;

namespace App.Distrbute.Common.Dtos;

[Redactable]
public class WalletResDto
{
    [Required] public string Id { get; set; } = null!;
    [Redacted] public string AccountNumber { get; set; } = null!;
    public string? AccountName { get; set; }
    public PaymentChannel? Type { get; set; }
    public PaymentChannelProvider? Provider { get; set; }
    public string? ProviderLogoUrl { get; set; } = null!;
    public bool Verified { get; set; } = false;

    /// <summary>
    ///     Total money in wallet including amounts escrowed
    /// </summary>
    [Redacted]
    public double Balance { get; set; } = 0;

    /// <summary>
    ///     Balance available to use for further transactions
    /// </summary>
    [Redacted]
    public double AvailableBalance { get; set; } = 0;
}
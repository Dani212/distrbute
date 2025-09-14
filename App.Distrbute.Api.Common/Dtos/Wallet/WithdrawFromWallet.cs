using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos.Wallet;

public class WithdrawFromWallet
{
    [Required] public double Amount { get; set; }
    public string TenantId { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Dtos;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class BaseWallet : BaseModel
{
    [ForeignKey]
    [Required] public Email Email { get; set; } = null!;

    /// <summary>
    ///     Savings Product this wallet belongs to on ledger
    /// </summary>
    [Required]
    public int ProductId { get; set; }

    /// <summary>
    ///     Ledger account id
    /// </summary>
    [Required]
    public int AccountId { get; set; }

    [Required] public string? AccountName { get; set; }
}

public static class BaseWalletExtensions
{
    public static WalletResDto ToResponseDto(this BaseWallet wallet, double availableBalance, double balance)
    {
        var response = new WalletResDto();
        response.Id = wallet.Id;
        response.AccountName = wallet.AccountName;
        response.AvailableBalance = availableBalance;
        response.Balance = balance;

        return response;
    }

    public static Depository ToDepository(this BaseWallet wallet)
    {
        var depo = new Depository();
        depo.Id = wallet.Id;
        depo.WalletAccountId = wallet.AccountId;
        depo.WalletProductId = wallet.ProductId;
        depo.WalletAccountName = wallet.AccountName;
        return depo;
    }
}
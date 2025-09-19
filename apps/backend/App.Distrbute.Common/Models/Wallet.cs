using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Dtos;
using Paystack.Sdk.Enums;
using Persistence.Sdk.Models;
using Redact.Sdk.Attributes;

namespace App.Distrbute.Common.Models;

[Redactable]
public class Wallet : BaseWallet
{
    [ForeignKey]
    public Brand? Brand { get; set; }
    
    [ForeignKey]
    public Distributor? Distributor { get; set; }

    /// <summary>
    ///     Account number from selected provider
    /// </summary>
    [Required, Redacted]
    public string AccountNumber { get; set; } = null!;

    [Required] public PaymentChannel? Type { get; set; }
    [Required] public PaymentChannelProvider? Provider { get; set; }
    public string? ProviderLogoUrl { get; set; } 
    public string? AuthorizationCode { get; set; }
    public string? RecipientCode { get; set; }
    public bool Verified { get; set; } = false;
    public bool Active { get; set; } = true;
}

public static class WalletExtensions
{
    public static WalletResDto ToResponseDto(this Wallet wallet, double availableBalance, double balance)
    {
        var response = new WalletResDto();
        response.Id = wallet.Id;
        response.AccountNumber = wallet.AccountNumber;
        response.AccountName = wallet.AccountName;
        response.Provider = wallet.Provider;
        response.Type = wallet.Type;
        response.ProviderLogoUrl = wallet.ProviderLogoUrl;
        response.AvailableBalance = availableBalance;
        response.Balance = balance;
        response.Verified = wallet.Verified;

        return response;
    }

    public static Depository ToDepository(this Wallet wallet)
    {
        var depo = new Depository();
        depo.Id = wallet.Id;
        depo.WalletAccountId = wallet.AccountId;
        depo.WalletProductId = wallet.ProductId;
        depo.WalletClientId = wallet.ClientId;
        depo.WalletAccountName = wallet.AccountName;
        depo.WalletAccountNumber = wallet.AccountNumber;
        depo.WalletProvider = wallet.Provider!.Value;
        depo.WalletProviderLogoUrl = wallet.ProviderLogoUrl;
        depo.WalletType = wallet.Type!.Value;

        return depo;
    }
}
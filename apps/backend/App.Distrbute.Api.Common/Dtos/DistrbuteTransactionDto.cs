using App.Distrbute.Common.Enums;
using Paystack.Sdk.Enums;

namespace App.Distrbute.Api.Common.Dtos;

public class DistrbuteTransactionDto
{
    public string Id { get; set; } = null!;
    public string? AccountName { get; set; }
    public string? BrandId { get; set; }
    public string? DistributorId { get; set; }
    public string? BrandName { get; set; }
    public string? DistributorName { get; set; }
    public string Currency { get; set; } = "GHS";
    public string Description { get; set; } = null!;
    public string IntegrationChannel { get; set; } = null!;
    public DepositoryDto? Source { get; set; }
    public DepositoryDto? Destination { get; set; }
    public TransactionType TransactionType { get; set; }
    public string ClientReference { get; set; } = null!;
    public double? Amount { get; set; }
    public double? Charges { get; set; }
    public double? AmountAfterCharges { get; set; }
    public PaymentProcessor PaymentProcessor { get; set; }
    public string? PaymentProcessorClientReference { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public DateTime TransactionDate { get; set; }
}

public class DepositoryDto
{
    public string Id { get; set; } = null!;
    public string WalletAccountNumber { get; set; } = null!;
    public string? WalletAccountName { get; set; }
    public PaymentChannel? WalletType { get; set; }
    public PaymentChannelProvider? WalletProvider { get; set; }
}
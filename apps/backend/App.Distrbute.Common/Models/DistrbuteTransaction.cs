using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Distrbute.Common.Enums;
using Paystack.Sdk.Enums;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class DistrbuteTransaction : BaseModel
{
    [Persistence.Sdk.Models.ForeignKey]
    public Brand? Brand { get; set; }
    
    [Persistence.Sdk.Models.ForeignKey]
    public Distributor? Distributor { get; set; }
    
    public int? LedgerActionId { get; set; }
    
    [Required] 
    public string Currency { get; set; } = "GHS";
    
    [Required]
    public string Description { get; set; } = null!;
    
    [Required] 
    public string IntegrationChannel { get; set; } = null!;
    
    [Required, Column(TypeName = "jsonb")] 
    public Depository Source { get; set; }
    [Column(TypeName = "jsonb")] 
    public Depository? Destination { get; set; }

    /// <summary>
    ///     topup, escrow, payment, withdrawal
    /// </summary>
    [Required]
    public TransactionType TransactionType { get; set; }

    [Required] public string ClientReference { get; set; } = null!;

    public double? Amount { get; set; }
    public double? Charges { get; set; }
    public double? AmountAfterCharges { get; set; }
    public double? AmountDueDistrbute { get; set; }

    public PaymentProcessor? PaymentProcessor { get; set; }
    public string? PaymentProcessorClientReference { get; set; }
    public string? PaymentProcessorDescription { get; set; }

    /// <summary>
    ///     Paid, UnPaid
    /// </summary>
    [Required]
    public TransactionStatus TransactionStatus { get; set; } = TransactionStatus.Pending;

    [Required] public DateTime? TransactionDate { get; set; }
    public DateTime? SettledDate { get; set; }
    [Column(TypeName = "jsonb")] public List<Step> Steps { get; set; } = [];
}

public class Depository
{
    public string Id { get; set; } = null!;
    public int WalletAccountId { get; set; }
    public int WalletProductId { get; set; }
    public int WalletClientId { get; set; }
    public string WalletAccountNumber { get; set; } = null!;
    public string WalletAuthorizationCode { get; set; } = null!;
    public string WalletRecipientCode { get; set; } = null!;
    public string? WalletAccountName { get; set; }
    public PaymentChannel WalletType { get; set; }
    public PaymentChannelProvider WalletProvider { get; set; }
    public string? WalletProviderLogoUrl { get; set; }
}

public class Step
{
    public TransactionProcessingStep Description { get; set; }
    public DateTime At { get; set; } = DateTime.UtcNow;
}

public static class DistrbuteTransactionExtendsion
{
    public static DistrbuteTransaction AddStep(this DistrbuteTransaction transaction, TransactionProcessingStep step)
    {
        if (step == TransactionProcessingStep.Default)
            throw new ArgumentOutOfRangeException(nameof(step), step, $"Step {step} is undefined");

        // validate correct ordering
        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep != null && lastStep.Description > step)
            throw new InvalidOperationException(
                $"Last transaction step {lastStep.Description} is greater than step {step}");

        var newStep = new Step();
        newStep.Description = step;

        transaction.Steps.Add(newStep);

        return transaction;
    }

    public static DistrbuteTransaction RemoveLastStep(this DistrbuteTransaction transaction)
    {
        if (transaction.Steps.Count > 0) transaction.Steps.RemoveAt(transaction.Steps.Count - 1);

        return transaction;
    }
}
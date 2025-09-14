using App.Distrbute.Common.Enums;
using Paystack.Sdk.Enums;
using Persistence.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Dtos;

public class WalletPageRequest : PageRequest
{
    public string TenantId { get; set; } = null!;
    public PaymentChannel? Type { get; set; }
    public PaymentChannelProvider? Provider { get; set; }
}

public class TransactionPageRequest : PageRequest
{
    public TransactionType? Type { get; set; }
    public TransactionStatus? Status { get; set; }
}
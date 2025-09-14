using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Ledgr.Sdk.Dtos;
using Ledgr.Sdk.Dtos.Wallet.Create;
using Ledgr.Sdk.Dtos.Wallet.Deposit;
using Paystack.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Dtos.Wallet;

public class CheckStatusResponse
{
    public double? Amount { get; set; }
    public TransactionStatus? TransactionStatus { get; set; }
    public string ClientReference { get; set; } = null!;
    public int? AccountId { get; set; }
    public int? ProductId { get; set; }

    public bool IsPending => TransactionStatus is Distrbute.Common.Enums.TransactionStatus.Pending
        or Distrbute.Common.Enums.TransactionStatus.Challenged;
}

// initialize transaction
public class TransactionProcessingContext
{
    public string ClientReference { get; set; }
}

public class RetrievedTransaction : TransactionProcessingContext
{
    public DistrbuteTransaction PlatformTransaction { get; set; }
}

public class VerifiedTransaction : RetrievedTransaction
{
    public PaystackTransaction PaystackTransaction { get; set; }
}

// prepare transaction wallet

public class CreateLedgerAccount : VerifiedTransaction
{
    public CreateWalletRequest LedgerReq { get; set; }
}

public class CreatedLedgerAccount : CreateLedgerAccount
{
    public BaseFineractResponseDto LedgerRes { get; set; }
}

public class CreateTransferRecipientReq : CreatedLedgerAccount
{
    public CreateRecipient TransferRecipientReq { get; set; }
}

public class CreatedTransferRecipient : CreateTransferRecipientReq
{
    public TransferRecipient TransferRecipientRes { get; set; }
}

public class CreateWalletReq : CreatedTransferRecipient
{
    public Distrbute.Common.Models.Wallet WalletReq { get; set; }
}

public class VerifiedTransactionWithOptionalWallet : VerifiedTransaction
{
    public Distrbute.Common.Models.Wallet? Wallet { get; set; }
}

public class VerifiedTransactionWithResolvedWallet : VerifiedTransaction
{
    public Distrbute.Common.Models.Wallet Wallet { get; set; }
}

// deposit
public class DepositLedgerAccountReq : VerifiedTransactionWithResolvedWallet
{
    public DepositRequest DepositReq { get; set; }
}

public class DepositedLedgerAccount : VerifiedTransactionWithResolvedWallet
{
    public BaseFineractResponseDto? DepositRes { get; set; }
}
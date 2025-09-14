using App.Distrbute.Api.Common.Dtos.Wallet;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IDepositToWalletService
{
    // one
    Task<RetrievedTransaction> RetrieveTransactionAsync(TransactionProcessingContext context);

    // two
    Task<VerifiedTransaction> VerifyPaystackTransactionAsync(RetrievedTransaction context);

    // three
    VerifiedTransaction InitializeTransactionAsync(VerifiedTransaction context);

    // four
    Task<T> SaveTransactionAsync<T>(T context) where T : VerifiedTransaction;

    // five
    Task<VerifiedTransactionWithOptionalWallet> RetrieveTransactionWallet(VerifiedTransaction context);

    // five (tie)
    CreateLedgerAccount CreateLedgerAccountRequest(VerifiedTransaction context);

    Task<CreatedLedgerAccount> SaveLedgerAccountAsync(CreateLedgerAccount context);

    CreateTransferRecipientReq CreatePaystackRecipientRequest(CreatedLedgerAccount context);

    Task<CreatedTransferRecipient> SavePaystackRecipientAsync(CreateTransferRecipientReq context);

    CreateWalletReq CreateNewWalletRequest(CreatedTransferRecipient context);

    Task<VerifiedTransactionWithResolvedWallet> SaveNewWalletAsync(CreateWalletReq context);


    // six
    VerifiedTransactionWithResolvedWallet CreateDepositLockRequest(
        VerifiedTransactionWithResolvedWallet context);

    // seven
    Task<DepositLedgerAccountReq> CreateLedgerAccountDepositRequest(VerifiedTransactionWithResolvedWallet context);

    // eight
    Task<DepositedLedgerAccount> ExecuteDepositAsync(DepositLedgerAccountReq context);

    // nine
    DepositedLedgerAccount DetermineFinalTransactionState(DepositedLedgerAccount context);

    // ten
    CheckStatusResponse CheckStatus<T>(T context) where T : RetrievedTransaction;
}
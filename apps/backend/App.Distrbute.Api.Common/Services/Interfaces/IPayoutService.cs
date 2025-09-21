using App.Distrbute.Api.Common.Dtos.Wallet;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IPayoutService
{
    // Core pipeline methods
    Task<RetrievedPost> RetrievePostAsync(PayoutProcessingContext context);
    Task<InitializedPost> InitializePostForProcessingAsync(RetrievedPost context);
    Task<PostWithTransaction> RetrieveOrCreateTransactionAsync(PostWithCampaign context);
    Task<T> SaveTransactionAsync<T>(T context) where T : PostWithTransaction;
    Task<PostWithResolvedWallets> ResolveWalletsAsync(PostWithTransaction context);
    PostWithResolvedWallets CreateDepositLockRequest(PostWithResolvedWallets context);
    PostWithTransferRequest CreateTransferRequest(PostWithResolvedWallets context);
    Task<ExecutedTransfer> ExecuteTransferAsync(PostWithTransferRequest context);
    ExecutedTransfer DetermineFinalTransactionState(ExecutedTransfer context);
    Task<CompletedPayout> SendPaymentNotificationAsync(ExecutedTransfer context);
    PayoutTaskResponse CheckStatus<T>(T context) where T : InitializedPost;
}

public class PayoutTaskResponse
{
    public bool Successful { get; set; }
    public bool Retry { get; set; }
    public int MaxRetries { get; set; }
    public string Description { get; set; } = null!;
}
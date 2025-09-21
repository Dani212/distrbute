using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Ledgr.Sdk.Dtos;
using Ledgr.Sdk.Dtos.Wallet.Transfer;
using Ledgr.Sdk.Exceptions;
using Ledgr.Sdk.Services.Interfaces;
using Mapster;
using Messaging.Sdk.Mail.Core;
using Microsoft.Extensions.Options;
using Persistence.Sdk.Core.Interfaces;
using SendGrid.Helpers.Mail;
using Utility.Sdk.Exceptions;

namespace App.Distrbute.Api.Common.Services.Providers;

// Context progression for payout processing

/// <summary>
/// Idempotent payout processing service following pipeline architecture
/// Each step can reconstruct its state and be safely retried
/// No rollbacks needed - failed operations leave recoverable state
/// </summary>
public class PayoutService : IPayoutService
{
    private readonly IDbRepository _dbRepository;
    private readonly IEmailClient _emailClient;
    private readonly MailTemplateConfig _mailTemplateConfig;
    private readonly IWalletSdk _walletSdk;

    public PayoutService(IDbRepository dbRepository,
        IWalletSdk walletSdk,
        IEmailClient emailClient,
        IOptions<MailTemplateConfig> mailTemplateConfig)
    {
        _dbRepository = dbRepository;
        _walletSdk = walletSdk;
        _emailClient = emailClient;
        _mailTemplateConfig = mailTemplateConfig.Value;
    }

    // step 1
    public async Task<RetrievedPost> RetrievePostAsync(PayoutProcessingContext context)
    {
        var postId = context.PostId;

        var post = await _dbRepository
            .GetAsync<Post>(q => q
                .Where(e => e.Id == postId)
                .IncludeWith(e => e.CampaignInvite, e => e.Campaign)
                .Where(e => e.CampaignInvite != null)
                .IncludeWith(e => e.DistributorSocialAccount, e => e.Distributor, e => e.Email)
                .Include(e => e.DistrbuteTransaction)
                .Where(e => e.DistrbuteTransaction == null || e.DistrbuteTransaction.TransactionStatus == TransactionStatus.Pending || e.DistrbuteTransaction.TransactionStatus == TransactionStatus.Challenged)
                .Where(e => 
                    e.PostApprovalStatus == PostApprovalStatus.Approved &&
                    (e.PostPayoutStatus == PostPayoutStatus.Pending || e.PostPayoutStatus == PostPayoutStatus.Challenged))
            );

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<RetrievedPost>();
        resp.PlatformPost = post;

        return resp;
    }
    
    // step 2
    public async Task<InitializedPost> InitializePostForProcessingAsync(RetrievedPost context)
    {
        var post = context.PlatformPost;

        // Set status to InProgress to claim this payout (idempotent key)
        if (post.PostPayoutStatus == PostPayoutStatus.Pending)
        {
            post.PostPayoutStatus = PostPayoutStatus.InProgress;
            await _dbRepository.UpdateAsync(post);
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<InitializedPost>();
        resp.PlatformPost = post;

        return resp;
    }

    // step 3
    public async Task<PostWithTransaction> RetrieveOrCreateTransactionAsync(PostWithCampaign context)
    {
        var post = context.PlatformPost;
        var campaign = context.Campaign;

        // Try to find existing transaction for this payout
        var existingTransaction = post.DistrbuteTransaction;

        DistrbuteTransaction effectiveTransaction;
        if (existingTransaction == null)
        {
            // Create new transaction
            var amount = post.CampaignInvite!.Bid ;
            var fundingTransaction = campaign.FundingTransaction;
            
            effectiveTransaction = new DistrbuteTransaction();
            
            // we are moving the money from the brands wallet 
            effectiveTransaction.Source = fundingTransaction.Source;
            effectiveTransaction.Brand = post.Brand;
            effectiveTransaction.Distributor = post.DistributorSocialAccount!.Distributor;
            effectiveTransaction.Description = $"Funds transfer of GHS {amount:N2} initiated by platform";
            effectiveTransaction.IntegrationChannel = CommonConstants.IntegrationChannel.Platform;
            effectiveTransaction.TransactionType = TransactionType.Payment;
            effectiveTransaction.ClientReference = Guid.NewGuid().ToString();
            effectiveTransaction.Amount = amount;
            effectiveTransaction.Charges = 0;
            effectiveTransaction.AmountAfterCharges = amount;
            effectiveTransaction.AmountDueDistrbute = 0;
            effectiveTransaction.PaymentProcessor = PaymentProcessor.Distrbute;
            effectiveTransaction.PaymentProcessorClientReference = effectiveTransaction.ClientReference;
            effectiveTransaction.PaymentProcessorDescription = effectiveTransaction.Description;
            effectiveTransaction.TransactionStatus = TransactionStatus.InProgress;
            effectiveTransaction.TransactionDate = DateTime.UtcNow;
            
            effectiveTransaction.AddStep(TransactionProcessingStep.TransactionRecordCreated);
            
            effectiveTransaction = await _dbRepository.AddAsync(effectiveTransaction);
            post.DistrbuteTransaction = effectiveTransaction;
            
            await _dbRepository.UpdateAsync(post);
        }
        else
        {
            effectiveTransaction = existingTransaction;
            
            // Ensure transaction is in progress state
            effectiveTransaction.TransactionStatus = TransactionStatus.InProgress;
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<PostWithTransaction>();
        resp.Transaction = effectiveTransaction;

        return resp;
    }

    // step 4
    public async Task<T> SaveTransactionAsync<T>(T context) where T : PostWithTransaction
    {
        var transaction = context.Transaction;
        var updatedTransaction = await _dbRepository.UpdateAsync(transaction);
        context.Transaction = updatedTransaction;
        return context;
    }

    // step 5
    public async Task<PostWithResolvedWallets> ResolveWalletsAsync(PostWithTransaction context)
    {
        var post = context.PlatformPost;
        var transaction = context.Transaction;

        // Record this step if we just created the transaction
        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description == TransactionProcessingStep.TransactionRecordCreated)
            AddTransactionStep(context, lastStep.Description, TransactionProcessingStep.RetrievingWalletFromDatabase);

        // If we're now resolving wallets
        lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description == TransactionProcessingStep.RetrievingWalletFromDatabase)
        {
            // Retrieve destination wallet (distributor suspense wallet)
            var destinationWallet = await _dbRepository.GetAsync<SuspenseWallet>(w => w
                .Include(e => e.Distributor)
                .Where(e => e.Distributor.Id == post.DistributorSocialAccount!.Distributor.Id));

            var destination = destinationWallet.ToDepository();
            transaction.Destination = destination;

            AddTransactionStep(context, lastStep.Description, TransactionProcessingStep.RetrievedWalletFromDatabase);
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<PostWithResolvedWallets>();
        resp.SourceWallet = transaction.Source!;
        resp.DestinationWallet = transaction.Destination!;

        return resp;
    }
    
    // step 6
    public PostWithResolvedWallets CreateDepositLockRequest(PostWithResolvedWallets context)
    {
        var transaction = context.Transaction;

        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.RetrievedWalletFromDatabase)
            transaction.TransactionStatus = TransactionStatus.LockedForDeposit; // Idempotent key

        return context;
    }

    // step 6
    public PostWithTransferRequest CreateTransferRequest(PostWithResolvedWallets context)
    {
        var transaction = context.Transaction;
        var source = context.SourceWallet;
        var destination = context.DestinationWallet;

        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description == TransactionProcessingStep.RetrievedWalletFromDatabase)
            AddTransactionStep(context, lastStep.Description, TransactionProcessingStep.DepositingToWallet);
        

        var transferRequest = new TransferRequest();
        transferRequest.FromAccountId = source.WalletAccountId;
        transferRequest.FromClientId = source.WalletClientId;
        
        transferRequest.ToAccountId = destination.WalletAccountId;
        transferRequest.ToClientId = destination.WalletClientId;
        
        transferRequest.TransferAmount = transaction.AmountAfterCharges!.Value;
        transferRequest.TransferDate = transaction.TransactionDate!.Value;
        transferRequest.TransferDescription = transaction.Description;

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<PostWithTransferRequest>();
        resp.TransferReq = transferRequest;

        return resp;
    }

    // step 7
    public async Task<ExecutedTransfer> ExecuteTransferAsync(PostWithTransferRequest context)
    {
        var transaction = context.Transaction;
        var post = context.PlatformPost;
        var transferRequest = context.TransferReq;

        var lastStep = transaction.Steps.LastOrDefault();
        
        // Only execute if we're in the transferring state and haven't completed yet
        var shouldExecute = lastStep?.Description is TransactionProcessingStep.DepositingToWallet &&
                            transaction.TransactionStatus == TransactionStatus.LockedForDeposit &&
                            transaction.LedgerActionId == null;

        BaseFineractResponseDto transferResponse;
        if (shouldExecute)
        {
            try
            {
                transferResponse = await _walletSdk.TransferAsync(transferRequest);
                transaction.LedgerActionId = transferResponse.ActionId;
                
                // Update post payout amount
                post.PostPayoutStatus = PostPayoutStatus.Paid;

                AddTransactionStep(context, lastStep!.Description, TransactionProcessingStep.DepositedToWallet);
            }
            catch (LedgerException)
            {
                // Don't add the step - leave in depositing state for retry
                transaction.TransactionStatus = TransactionStatus.Challenged;
                throw;
            }
        }
        else
        {
            // Transfer was already executed, reconstruct response
            transferResponse = new BaseFineractResponseDto
            {
                ActionId = transaction.LedgerActionId!.Value
            };
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<ExecutedTransfer>();
        resp.TransferResponse = transferResponse;

        return resp;
    }

    // step 8
    public ExecutedTransfer DetermineFinalTransactionState(ExecutedTransfer context)
    {
        var transaction = context.Transaction;

        var lastStep = transaction.Steps.LastOrDefault();

        // Already successful transaction
        if (lastStep?.Description == TransactionProcessingStep.Successful)
        {
            transaction.TransactionStatus = TransactionStatus.Successful;
        }
        // Transaction that just became successful
        else if (lastStep?.Description == TransactionProcessingStep.DepositedToWallet)
        {
            AddTransactionStep(context, lastStep.Description, TransactionProcessingStep.Successful);
            transaction.TransactionStatus = TransactionStatus.Successful;
        }
        // If something went wrong
        else
        {
            transaction.TransactionStatus = TransactionStatus.Challenged;
        }

        return context;
    }

    // step 9
    public async Task<CompletedPayout> SendPaymentNotificationAsync(ExecutedTransfer context)
    {
        var post = context.PlatformPost;
        var transaction = context.Transaction;
        
        bool emailSent = false;
        
        // Only send email if transaction is successful and email hasn't been sent
        if (transaction.TransactionStatus == TransactionStatus.Successful)
        {
            try
            {
                await SendPaymentEmailInternal(post);
                emailSent = true;
            }
            catch (Exception)
            {
                // Email failure doesn't affect transaction success
                emailSent = false;
            }
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<CompletedPayout>();
        resp.EmailSent = emailSent;

        return resp;
    }

    // step 10
    public PayoutTaskResponse CheckStatus<T>(T context) where T : InitializedPost
    {
        var post = context.PlatformPost;
        var transaction = (context as PostWithTransaction)?.Transaction;

        var resp = new PayoutTaskResponse();
        
        if (transaction?.TransactionStatus == TransactionStatus.Successful)
        {
            resp.Successful = true;
            resp.Description = $"Successfully transferred {transaction.Amount} to distributor {post.DistributorSocialAccount!.Distributor.Id}";
            resp.Retry = false;
            resp.MaxRetries = 0;
        }
        else if (transaction?.TransactionStatus == TransactionStatus.Challenged)
        {
            resp.Successful = false;
            resp.Description = $"Transfer challenged, ready for retry";
            resp.Retry = true;
            resp.MaxRetries = 3;
        }
        else
        {
            resp.Successful = false;
            resp.Description = $"Payout processing in progress";
            resp.Retry = false;
            resp.MaxRetries = 0;
        }

        return resp;
    }

    private async Task SendPaymentEmailInternal(Post existingPost)
    {
        var templateData = new Dictionary<string, string>();
        var distributor = existingPost.DistributorSocialAccount!.Distributor;
        
        templateData.Add("name", distributor.Name);
        templateData.Add("amount", $"₵{existingPost.DistrbuteTransaction!.AmountAfterCharges:N}");
        templateData.Add("year", DateTime.UtcNow.Year.ToString());

        var personalization = new Personalization();
        personalization.Subject = "📣 You've Been Paid For An Ad On Distrbute!";
        templateData.Add("subject", personalization.Subject);
        personalization.Tos = new List<EmailAddress> { new(distributor.Email.Address) };
        personalization.TemplateData = templateData;

        var mail = new BulkEmail();
        mail.Subject = personalization.Subject;
        mail.TemplateId = _mailTemplateConfig.AdPaymentTemplateId;
        mail.Personalizations = [personalization];

        await _emailClient.SendBulkAsync(mail);
    }

    private void AddTransactionStep<T>(T context, TransactionProcessingStep previousStep, TransactionProcessingStep newStep) 
        where T : PostWithTransaction
    {
        var transaction = context.Transaction;

        if (previousStep == TransactionProcessingStep.Default) 
            throw new BadRequest("Step argument out of range");

        // Validate precedence
        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep == null || lastStep.Description != previousStep)
            throw new BadRequest($"Cannot set transaction progress, Invalid last step: {lastStep?.Description}");

        // Add steps
        if (transaction.Steps.Any(s => s.Description == newStep))
            throw new BadRequest($"Cannot set transaction progress, step: {newStep} already exists");

        transaction.AddStep(newStep);
    }
}
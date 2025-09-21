using Akka.Actor;
using Akka.Hosting;
using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Models;
using Mapster;
using ObjectStorage.Sdk.Dtos;
using ObjectStorage.Sdk.Services.Interfaces;
using ObjectStorage.Sdk.Services.Providers;
using Pipeline.Sdk.Actors;
using Pipeline.Sdk.Actors.ActorMessages;
using Pipeline.Sdk.Core;
using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Services.Providers;

public class PipelineProvider : IPipelineProvider
{
    private readonly IDepositToWalletService _depositService;
    private readonly IPayoutService _payoutService;
    private readonly IFileUploader _fileUploader;
    private readonly IMediaProcessor _mediaProcessor;
    private readonly IRequiredActor<RunPipelineActor> _pipelineActor;
    private readonly IPipelineFactory _pipelineFactory;
    private readonly IPostMetricService _postMetricService;
    private readonly IPostValuationService _postValuationService;
    private readonly IPostValuationWriter _postValuationWriter;
    private readonly ISocialAccountValuationService _socialAccountValuationService;
    private readonly IUploadedFileFinalizer _uploadedFileFinalizer;

    public PipelineProvider(
        IPipelineFactory pipelineFactory,
        IRequiredActor<RunPipelineActor> pipelineActor,
        IMediaProcessor processor,
        IFileUploader fileUploader,
        IUploadedFileFinalizer uploadedFileFinalizer,
        IPostMetricService postMetricService,
        IPostValuationService postValuationService,
        IPostValuationWriter postValuationWriter,
        ISocialAccountValuationService socialAccountValuationService,
        IDepositToWalletService depositService,
        IPayoutService payoutService
        )
    {
        _pipelineFactory = pipelineFactory;
        _pipelineActor = pipelineActor;
        _mediaProcessor = processor;
        _fileUploader = fileUploader;
        _uploadedFileFinalizer = uploadedFileFinalizer;
        _postMetricService = postMetricService;
        _postValuationService = postValuationService;
        _postValuationWriter = postValuationWriter;
        _socialAccountValuationService = socialAccountValuationService;
        _depositService = depositService;
        _payoutService = payoutService;
    }

    public async Task ExecuteMediaProcessingPipeline(Email principal, MediaProcessingReq req, string prefix)
    {
        var pipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<MediaProcessingReq, ProcessedFile>("Process media to different formats",
                _mediaProcessor.ProcessMediaAsync)
            .AddStage<ProcessedFile, List<UploadReq>>("Transform the paths to uploads reqs", processedFile =>
            {
                var uploads = new List<UploadReq>();

                foreach (var path in processedFile.ProcessedFilePaths)
                {
                    var upload = new UploadReq();
                    upload.OriginalFilename = processedFile.OriginalFilename;
                    upload.Path = path;
                    upload.PathPrefix = prefix;
                    upload.OwnerId = principal.Address;
                    upload.OriginalFileCreationDate = processedFile.OriginalFileCreationDate;

                    uploads.Add(upload);
                }

                return uploads;
            })
            .AddStage<List<UploadReq>, List<UploadedFile>>("Upload the files",
                reqs => _fileUploader.UploadFiles<DocumentFile>(reqs))
            .AddStage<List<UploadedFile>, List<string>>("Clean up local copy of temp files",
                _uploadedFileFinalizer.Finalize)
            .Build();

        var runnerMessage = new PipelineInitMessage(req, pipeline);

        var actor = await _pipelineActor.GetAsync();
        actor.Tell(runnerMessage);
    }

    public async Task ExecutePostTrackingPipeline(TrackOnePostReq req)
    {
        var pipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<TrackOnePostReq, TrackedPost>("Gather post metrics", _postMetricService.GatherAsync)
            .AddStage<TrackedPost, ValuedPost>("Calculate value of post",
                _postValuationService.Value)
            .AddStage<ValuedPost, SocialProfile>("Write value of post",
                _postValuationWriter.WriteAsync)
            .AddStage<SocialProfile, bool>("Value social account", _socialAccountValuationService.ValueAsync)
            .Build();
        
        var runnerMessage = new PipelineInitMessage(req, pipeline);
        
        var actor = await _pipelineActor.GetAsync();
        actor.Tell(runnerMessage);
    }

    public async Task ExecuteInitSocialAccountValuePipeline<T>(T socialAccount) where T : SocialAccountBase
    {
        var pipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<TrackPostBase, TrackedPosts>("Gather posts metrics", _postMetricService.GatherManyAsync)
            .AddStage<TrackedPosts, ValuedPosts>("Calculate value of posts",
                _postValuationService.ValueMany)
            .AddStage<ValuedPosts, SocialProfile>("Write value of posts",
                _postValuationWriter.WriteManyAsync)
            .AddStage<SocialProfile, bool>("Value social account", _socialAccountValuationService.ValueAsync)
            .Build();
        
        var trackPostBase = new TrackPostBase();
        if (socialAccount is DistributorSocialAccount distributorSocialAccount)
        {
            trackPostBase.DistributorSocialAccount = distributorSocialAccount;
        }
        if (socialAccount is BrandSocialAccount brandSocialAccount)
        {
            trackPostBase.BrandSocialAccount = brandSocialAccount;
        }
        
        var socialProfile = socialAccount.AsSocialProfile();
        trackPostBase.SocialProfile = socialProfile;
        
        var runnerMessage = new PipelineInitMessage(trackPostBase, pipeline);
        
        var actor = await _pipelineActor.GetAsync();
        actor.Tell(runnerMessage);
    }

    public Pipeline.Sdk.Core.Pipeline DepositProcessingPipeline()
    {
        var initializationPipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<TransactionProcessingContext, RetrievedTransaction>(
                "Load platform transaction from storage (using client reference)",
                _depositService.RetrieveTransactionAsync
            )
            .AddStage<RetrievedTransaction, VerifiedTransaction>(
                "Cross-check transaction with Paystack (verify authenticity, amount, status)",
                _depositService.VerifyPaystackTransactionAsync
            )
            .AddStage<VerifiedTransaction>(
                "Initialize platform-side transaction metadata (prepare ledger + wallet references)",
                _depositService.InitializeTransactionAsync
            )
            .AddStage<VerifiedTransaction>(
                "Persist initialized transaction state for durability",
                _depositService.SaveTransactionAsync
            )
            .Build();
        
        var walletCreationPipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<VerifiedTransaction, CreateLedgerAccount>(
                "Construct request to open a new ledger account (internal representation of funds)",
                _depositService.CreateLedgerAccountRequest
            )
            .AddStage<CreateLedgerAccount, CreatedLedgerAccount>(
                "Persist the newly created ledger account into our database",
                _depositService.SaveLedgerAccountAsync
            )
            .AddStage<CreatedLedgerAccount>(
                "Link ledger account details to the pending transaction",
                _depositService.SaveTransactionAsync
            )
            .AddStage<CreatedLedgerAccount, CreateTransferRecipientReq>(
                "Construct request to register recipient with Paystack (needed for withdrawals)",
                _depositService.CreatePaystackRecipientRequest
            )
            .AddStage<CreateTransferRecipientReq, CreatedTransferRecipient>(
                "Persist Paystack recipient details in our database",
                _depositService.SavePaystackRecipientAsync
            )
            .AddStage<CreatedTransferRecipient>(
                "Attach recipient details to the transaction record",
                _depositService.SaveTransactionAsync
            )
            .AddStage<CreatedTransferRecipient, CreateWalletReq>(
                "Construct request to create new platform wallet (user-facing account)",
                _depositService.CreateNewWalletRequest
            )
            .AddStage<CreateWalletReq, VerifiedTransactionWithResolvedWallet>(
                "Persist new wallet in our system, return resolved wallet binding",
                _depositService.SaveNewWalletAsync
            )
            .Build();
        
        Func<VerifiedTransaction, Task<VerifiedTransactionWithResolvedWallet>> transactionWalletResolver =
            async verifiedTransaction =>
            {
                var context = await _depositService
                    .RetrieveTransactionWallet(verifiedTransaction);
                var existingWallet = context.Wallet;
        
                if (existingWallet == null)
                {
                    var resolvedWallet = await walletCreationPipeline
                        .ExecuteReturningAsync<VerifiedTransactionWithResolvedWallet>(verifiedTransaction);
        
                    return resolvedWallet;
                }
        
                var res = context.Adapt<VerifiedTransactionWithResolvedWallet>();
        
                return res;
            };
        
        var processingPipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<TransactionProcessingContext, VerifiedTransaction>(
                "Run transaction initialization pipeline (retrieve + verify transaction)",
                initializationPipeline.ExecuteReturningAsync<VerifiedTransaction>
            )
            .AddStage(
                "Resolve wallet: use existing one or trigger wallet creation pipeline",
                transactionWalletResolver
            )
            .AddStage<VerifiedTransactionWithResolvedWallet>(
                "Persist resolved wallet details against transaction (ensures linkage)",
                _depositService.SaveTransactionAsync
            )
            .AddStage<VerifiedTransactionWithResolvedWallet>(
                "Create optimistic ledger entry (deposit lock to prevent race conditions)",
                _depositService.CreateDepositLockRequest
            )
            .AddStage<VerifiedTransactionWithResolvedWallet>(
                "Persist optimistic ledger state on transaction (early visibility)",
                _depositService.SaveTransactionAsync
            )
            .AddStage<VerifiedTransactionWithResolvedWallet, DepositLedgerAccountReq>(
                "Construct final ledger deposit payload (amount, account, metadata)",
                _depositService.CreateLedgerAccountDepositRequest
            )
            .AddStage<DepositLedgerAccountReq, DepositedLedgerAccount>(
                "Execute actual deposit into ledger (money movement step — idempotent critical)",
                _depositService.ExecuteDepositAsync
            )
            .AddStage<VerifiedTransactionWithResolvedWallet>(
                "Persist deposit details on wallet + transaction",
                _depositService.SaveTransactionAsync
            )
            .AddStage<DepositedLedgerAccount>(
                "Determine final transaction state (success/failure based on ledger result)",
                _depositService.DetermineFinalTransactionState
            )
            .AddStage<DepositedLedgerAccount>(
                "Persist final transaction state (commit ledger + wallet linkage)",
                _depositService.SaveTransactionAsync
            )
            .Build();
        
        
        return processingPipeline;
    }

    public async Task<CheckStatusResponse> ExecuteDepositProcessingPipeline(string clientReference)
    {
        var processingContext = new TransactionProcessingContext();
        processingContext.ClientReference = clientReference;
        
        var transactionStatusPipeline = TransactionStatusPipeline();
        var checkStatus = await transactionStatusPipeline
            .ExecuteReturningAsync<CheckStatusResponse>(processingContext);
        
        var transactionPending = checkStatus.IsPending;
        if (transactionPending)
        {
            var pipeline = DepositProcessingPipeline();
        
            var runnerMessage = new PipelineInitMessage(processingContext, pipeline);
        
            // deposit processing pipeline should ALWAYS run in an actor to ensure there is no risk of race conditions
            var actor = await _pipelineActor.GetAsync();
            actor.Tell(runnerMessage);
        }
        
        return checkStatus;
    }

    private Pipeline.Sdk.Core.Pipeline TransactionStatusPipeline()
    {
        var pipeline = _pipelineFactory.CreateBuilder()
            .AddStage<TransactionProcessingContext, RetrievedTransaction>(
                "Load transaction from platform storage (via client reference)",
                _depositService.RetrieveTransactionAsync
            )
            .AddStage<RetrievedTransaction, CheckStatusResponse>(
                "Check if transaction already processed (prevents re-execution)",
                _depositService.CheckStatus
            )
            .Build();
        
        return pipeline;
    }
    
    
    // DISTRIBUTOR PAYOUTS
    public Pipeline.Sdk.Core.Pipeline PayoutProcessingPipeline()
    {
        var initializationPipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<PayoutProcessingContext, RetrievedPost>(
                "Load post from platform storage (using post ID)",
                _payoutService.RetrievePostAsync
            )
            .AddStage<RetrievedPost, InitializedPost>(
                "Initialize post for payout processing (claim and lock)",
                _payoutService.InitializePostForProcessingAsync
            )
            .AddStage<PostWithCampaign, PostWithTransaction>(
                "Retrieve or create transaction record for this payout",
                _payoutService. RetrieveOrCreateTransactionAsync
            )
            .AddStage<PostWithTransaction>(
                "Persist transaction state for durability",
                _payoutService. SaveTransactionAsync
            )
            .Build();

        var walletResolutionPipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<PostWithTransaction, PostWithResolvedWallets>(
                "Resolve destination (suspense) wallet for distributor",
                _payoutService.ResolveWalletsAsync
            )
            .AddStage<PostWithResolvedWallets>(
                "Persist wallet details on transaction",
                _payoutService.SaveTransactionAsync
            )
            .AddStage<PostWithResolvedWallets>(
                "Create transfer lock (prevent race conditions)",
                _payoutService.CreateDepositLockRequest
            )
            .AddStage<PostWithResolvedWallets>(
                "Persist transfer lock state (critical save point)",
                _payoutService.SaveTransactionAsync
            )
            .Build();

        var transferExecutionPipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<PostWithResolvedWallets, PostWithTransferRequest>(
                "Construct ledger transfer request payload",
                _payoutService.CreateTransferRequest
            )
            .AddStage<PostWithTransferRequest>(
                "Persist transfer request details",
                _payoutService.SaveTransactionAsync
            )
            .AddStage<PostWithTransferRequest, ExecutedTransfer>(
                "Execute actual funds transfer via ledger (idempotent critical step)",
                _payoutService.ExecuteTransferAsync
            )
            .AddStage<ExecutedTransfer>(
                "Persist transfer results on transaction and post",
                _payoutService.SaveTransactionAsync
            )
            .AddStage<ExecutedTransfer>(
                "Determine final transaction state based on transfer result",
                _payoutService.DetermineFinalTransactionState
            )
            .AddStage<ExecutedTransfer>(
                "Persist final transaction state",
                _payoutService.SaveTransactionAsync
            )
            .AddStage<ExecutedTransfer, CompletedPayout>(
                "Send payment notification email to distributor",
                _payoutService.SendPaymentNotificationAsync
            )
            .Build();

        var mainPipeline = _pipelineFactory
            .CreateBuilder()
            .AddStage<PayoutProcessingContext, PostWithTransaction>(
                "Run initialization pipeline (post retrieval + campaign validation + transaction setup)",
                initializationPipeline.ExecuteReturningAsync<PostWithTransaction>
            )
            .AddStage<PostWithTransaction, PostWithResolvedWallets>(
                "Run wallet resolution pipeline (destination wallet + deposit lock)",
                walletResolutionPipeline.ExecuteReturningAsync<PostWithResolvedWallets>
            )
            .AddStage<PostWithResolvedWallets, CompletedPayout>(
                "Run transfer execution pipeline (ledger transfer + finalization)",
                transferExecutionPipeline.ExecuteReturningAsync<CompletedPayout>
            )
            .Build();

        return mainPipeline;
    }

    public async Task<PayoutTaskResponse> ExecutePayoutProcessingPipeline(string postId)
    {
        var processingContext = new PayoutProcessingContext();
        processingContext.PostId = postId;

        var statusPipeline = PayoutStatusPipeline();
        var checkStatus = await statusPipeline
            .ExecuteReturningAsync<PayoutTaskResponse>(processingContext);

        var needsProcessing = !checkStatus.Successful && checkStatus.Retry;
        if (needsProcessing)
        {
            var pipeline = PayoutProcessingPipeline();

            var runnerMessage = new PipelineInitMessage(processingContext, pipeline);

            // payout processing pipeline should ALWAYS run in an actor to ensure no race conditions
            var actor = await _pipelineActor.GetAsync();
            actor.Tell(runnerMessage);
        }

        return checkStatus;
    }

    private Pipeline.Sdk.Core.Pipeline PayoutStatusPipeline()
    {
        var pipeline = _pipelineFactory.CreateBuilder()
            .AddStage<PayoutProcessingContext, RetrievedPost>(
                "Load post from platform storage (check processing eligibility)",
                _payoutService.RetrievePostAsync
            )
            .AddStage<RetrievedPost, InitializedPost>(
                "Initialize post processing state",
                _payoutService. InitializePostForProcessingAsync
            )
            .AddStage<InitializedPost, PayoutTaskResponse>(
                "Check if payout already processed (prevents re-execution)",
                _payoutService.CheckStatus
            )
            .Build();

        return pipeline;
    }
}
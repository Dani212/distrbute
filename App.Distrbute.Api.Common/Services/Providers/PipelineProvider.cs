using Akka.Actor;
using Akka.Hosting;
using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Models;
using ObjectStorage.Sdk.Dtos;
using ObjectStorage.Sdk.Services.Interfaces;
using ObjectStorage.Sdk.Services.Providers;
using Pipeline.Sdk.Actors;
using Pipeline.Sdk.Actors.ActorMessages;
using Pipeline.Sdk.Core;

namespace App.Distrbute.Api.Common.Services.Providers;

public class PipelineProvider : IPipelineProvider
{
    // private readonly IDepositToWalletService _depositService;
    private readonly IFileUploader _fileUploader;
    private readonly IMediaProcessor _mediaProcessor;
    private readonly IRequiredActor<RunPipelineActor> _pipelineActor;
    private readonly IPipelineFactory _pipelineFactory;
    // private readonly IPostEngagementTrackingService _postEngagementTrackingService;
    // private readonly ISocialAccountValuationService _socialAccountValuationService;
    // private readonly ISocialAccountValuationWriter _socialAccountValuationWriter;
    private readonly IUploadedFileFinalizer _uploadedFileFinalizer;

    public PipelineProvider(
        IPipelineFactory pipelineFactory,
        IRequiredActor<RunPipelineActor> pipelineActor,
        IMediaProcessor processor,
        IFileUploader fileUploader,
        IUploadedFileFinalizer uploadedFileFinalizer
        // IPostEngagementTrackingService postEngagementTrackingService,
        // ISocialAccountValuationService socialAccountValuationService,
        // ISocialAccountValuationWriter socialAccountValuationWriter,
        // IDepositToWalletService depositService
        )
    {
        _pipelineFactory = pipelineFactory;
        _pipelineActor = pipelineActor;
        _mediaProcessor = processor;
        _fileUploader = fileUploader;
        _uploadedFileFinalizer = uploadedFileFinalizer;
        // _postEngagementTrackingService = postEngagementTrackingService;
        // _socialAccountValuationService = socialAccountValuationService;
        // _socialAccountValuationWriter = socialAccountValuationWriter;
        // _depositService = depositService;
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

    public async Task ExecutePostTrackingPipeline(string postId)
    {
        throw new NotImplementedException();
        
        // var pipeline = _pipelineFactory
        //     .CreateBuilder()
        //     .AddStage<string, TrackedPost>("Gather post metrics", _postEngagementTrackingService.TrackAsync)
        //     .AddStage<TrackedPost, ValuedPost>("Calculate social account added value from post",
        //         _socialAccountValuationService.Valuate)
        //     .AddStage<ValuedPost, PostValuation>("Write new social account value",
        //         _socialAccountValuationWriter.WriteAsync)
        //     .Build();
        //
        // var runnerMessage = new PipelineInitMessage(postId, pipeline);
        //
        // var actor = await _pipelineActor.GetAsync();
        // actor.Tell(runnerMessage);
    }

    public async Task ExecuteInitSocialAccountValuePipeline(string socialAccountId)
    {
        throw new NotImplementedException();
        
        // var pipeline = _pipelineFactory
        //     .CreateBuilder()
        //     .AddStage<string, TrackedPosts>("Gather posts metrics", _postEngagementTrackingService.TrackManyAsync)
        //     .AddStage<TrackedPosts, ValuedPosts>("Calculate social account value from posts",
        //         _socialAccountValuationService.Valuate)
        //     .AddStage<ValuedPosts, List<PostValuation>>("Write new social account values",
        //         _socialAccountValuationWriter.WriteAsync)
        //     .Build();
        //
        // var runnerMessage = new PipelineInitMessage(socialAccountId, pipeline);
        //
        // var actor = await _pipelineActor.GetAsync();
        // actor.Tell(runnerMessage);
    }

    public Pipeline.Sdk.Core.Pipeline DepositProcessingPipeline()
    {
        throw new NotImplementedException();
        
        // var initializationPipeline = _pipelineFactory
        //     .CreateBuilder()
        //     .AddStage<TransactionProcessingContext, RetrievedTransaction>(
        //         "Load platform transaction from storage (using client reference)",
        //         _depositService.RetrieveTransactionAsync
        //     )
        //     .AddStage<RetrievedTransaction, VerifiedTransaction>(
        //         "Cross-check transaction with Paystack (verify authenticity, amount, status)",
        //         _depositService.VerifyPaystackTransactionAsync
        //     )
        //     .AddStage<VerifiedTransaction>(
        //         "Initialize platform-side transaction metadata (prepare ledger + wallet references)",
        //         _depositService.InitializeTransactionAsync
        //     )
        //     .AddStage<VerifiedTransaction>(
        //         "Persist initialized transaction state for durability",
        //         _depositService.SaveTransactionAsync
        //     )
        //     .Build();
        //
        // var walletCreationPipeline = _pipelineFactory
        //     .CreateBuilder()
        //     .AddStage<VerifiedTransaction, CreateLedgerAccount>(
        //         "Construct request to open a new ledger account (internal representation of funds)",
        //         _depositService.CreateLedgerAccountRequest
        //     )
        //     .AddStage<CreateLedgerAccount, CreatedLedgerAccount>(
        //         "Persist the newly created ledger account into our database",
        //         _depositService.SaveLedgerAccountAsync
        //     )
        //     .AddStage<CreatedLedgerAccount>(
        //         "Link ledger account details to the pending transaction",
        //         _depositService.SaveTransactionAsync
        //     )
        //     .AddStage<CreatedLedgerAccount, CreateTransferRecipientReq>(
        //         "Construct request to register recipient with Paystack (needed for withdrawals)",
        //         _depositService.CreatePaystackRecipientRequest
        //     )
        //     .AddStage<CreateTransferRecipientReq, CreatedTransferRecipient>(
        //         "Persist Paystack recipient details in our database",
        //         _depositService.SavePaystackRecipientAsync
        //     )
        //     .AddStage<CreatedTransferRecipient>(
        //         "Attach recipient details to the transaction record",
        //         _depositService.SaveTransactionAsync
        //     )
        //     .AddStage<CreatedTransferRecipient, CreateWalletReq>(
        //         "Construct request to create new platform wallet (user-facing account)",
        //         _depositService.CreateNewWalletRequest
        //     )
        //     .AddStage<CreateWalletReq, VerifiedTransactionWithResolvedWallet>(
        //         "Persist new wallet in our system, return resolved wallet binding",
        //         _depositService.SaveNewWalletAsync
        //     )
        //     .Build();
        //
        // Func<VerifiedTransaction, Task<VerifiedTransactionWithResolvedWallet>> transactionWalletResolver =
        //     async verifiedTransaction =>
        //     {
        //         var context = await _depositService
        //             .RetrieveTransactionWallet(verifiedTransaction);
        //         var existingWallet = context.Wallet;
        //
        //         if (existingWallet == null)
        //         {
        //             var resolvedWallet = await walletCreationPipeline
        //                 .ExecuteReturningAsync<VerifiedTransactionWithResolvedWallet>(verifiedTransaction);
        //
        //             return resolvedWallet;
        //         }
        //
        //         var res = context.Adapt<VerifiedTransactionWithResolvedWallet>();
        //
        //         return res;
        //     };
        //
        // var processingPipeline = _pipelineFactory
        //     .CreateBuilder()
        //     .AddStage<TransactionProcessingContext, VerifiedTransaction>(
        //         "Run transaction initialization pipeline (retrieve + verify transaction)",
        //         initializationPipeline.ExecuteReturningAsync<VerifiedTransaction>
        //     )
        //     .AddStage(
        //         "Resolve wallet: use existing one or trigger wallet creation pipeline",
        //         transactionWalletResolver
        //     )
        //     .AddStage<VerifiedTransactionWithResolvedWallet>(
        //         "Persist resolved wallet details against transaction (ensures linkage)",
        //         _depositService.SaveTransactionAsync
        //     )
        //     .AddStage<VerifiedTransactionWithResolvedWallet>(
        //         "Create optimistic ledger entry (deposit lock to prevent race conditions)",
        //         _depositService.CreateDepositLockRequest
        //     )
        //     .AddStage<VerifiedTransactionWithResolvedWallet>(
        //         "Persist optimistic ledger state on transaction (early visibility)",
        //         _depositService.SaveTransactionAsync
        //     )
        //     .AddStage<VerifiedTransactionWithResolvedWallet, DepositLedgerAccountReq>(
        //         "Construct final ledger deposit payload (amount, account, metadata)",
        //         _depositService.CreateLedgerAccountDepositRequest
        //     )
        //     .AddStage<DepositLedgerAccountReq, DepositedLedgerAccount>(
        //         "Execute actual deposit into ledger (money movement step — idempotent critical)",
        //         _depositService.ExecuteDepositAsync
        //     )
        //     .AddStage<VerifiedTransactionWithResolvedWallet>(
        //         "Persist deposit details on wallet + transaction",
        //         _depositService.SaveTransactionAsync
        //     )
        //     .AddStage<DepositedLedgerAccount>(
        //         "Determine final transaction state (success/failure based on ledger result)",
        //         _depositService.DetermineFinalTransactionState
        //     )
        //     .AddStage<DepositedLedgerAccount>(
        //         "Persist final transaction state (commit ledger + wallet linkage)",
        //         _depositService.SaveTransactionAsync
        //     )
        //     .Build();
        //
        //
        // return processingPipeline;
    }

    public async Task<CheckStatusResponse> ExecuteDepositProcessingPipeline(string clientReference)
    {
        throw new NotImplementedException();
        
        // var processingContext = new TransactionProcessingContext();
        // processingContext.ClientReference = clientReference;
        //
        // var transactionStatusPipeline = TransactionStatusPipeline();
        // var checkStatus = await transactionStatusPipeline
        //     .ExecuteReturningAsync<CheckStatusResponse>(processingContext);
        //
        // var transactionPending = checkStatus.IsPending;
        // if (transactionPending)
        // {
        //     var pipeline = DepositProcessingPipeline();
        //
        //     var runnerMessage = new PipelineInitMessage(processingContext, pipeline);
        //
        //     // deposit processing pipeline should ALWAYS run in an actor to ensure there is no risk of race conditions
        //     var actor = await _pipelineActor.GetAsync();
        //     actor.Tell(runnerMessage);
        // }
        //
        // return checkStatus;
    }

    private Pipeline.Sdk.Core.Pipeline TransactionStatusPipeline()
    {
        throw new NotImplementedException();
        //
        // var pipeline = _pipelineFactory.CreateBuilder()
        //     .AddStage<TransactionProcessingContext, RetrievedTransaction>(
        //         "Load transaction from platform storage (via client reference)",
        //         _depositService.RetrieveTransactionAsync
        //     )
        //     .AddStage<RetrievedTransaction, CheckStatusResponse>(
        //         "Check if transaction already processed (prevents re-execution)",
        //         _depositService.CheckStatus
        //     )
        //     .Build();
        //
        // return pipeline;
    }
}
using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Ledgr.Sdk.Dtos;
using Ledgr.Sdk.Dtos.Wallet.Create;
using Ledgr.Sdk.Dtos.Wallet.Deposit;
using Ledgr.Sdk.Exceptions;
using Ledgr.Sdk.Services.Interfaces;
using Mapster;
using Paystack.Sdk.Dtos;
using Paystack.Sdk.Enums;
using Paystack.Sdk.Services.Interfaces;
using Persistence.Sdk.Core.Interfaces;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Services.Providers;

/// <summary>
///     General transaction processing guidelines:
///     - Each step is idempotent and can reconstruct its results.
///     - Side effects must occur exactly once.
///     - Processing should happen within an actor to eliminate race conditions.
///     - If you must adapt an existing context into another as a response, ALWAYS do that after all your functional logic
///     to avoid invisible updates (Updates made to old context after you already adapted a new context)
/// </summary>
public class DepositToWalletService : IDepositToWalletService
{
    private readonly IDbRepository _dbRepository;
    private readonly IPayStackSdk _payStackSdk;
    private readonly IWalletSdk _walletSdk;

    public DepositToWalletService(IDbRepository dbRepository,
        IWalletSdk walletSdk,
        IPayStackSdk payStackSdk)
    {
        _dbRepository = dbRepository;
        _walletSdk = walletSdk;
        _payStackSdk = payStackSdk;
    }

    // step one
    public async Task<RetrievedTransaction> RetrieveTransactionAsync(TransactionProcessingContext context)
    {
        var clientReference = context.ClientReference;

        var transaction = await _dbRepository
            .GetAsync<DistrbuteTransaction>(b => b
                .IncludeWith(e => e.Brand, p => p.Email)
                .IncludeWith(e => e.Distributor, p => p.Email)
                .Where(e => e.ClientReference == clientReference));

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<RetrievedTransaction>();
        resp.PlatformTransaction = transaction;

        return resp;
    }

    // step two
    public async Task<VerifiedTransaction> VerifyPaystackTransactionAsync(RetrievedTransaction context)
    {
        var clientReference = context.ClientReference;
        var transaction = context.PlatformTransaction;

        PaystackTransactionResponse verifiedPaystackTransactionResponse;
        if (transaction.TransactionType is TransactionType.NewWallet or TransactionType.ActivateCampaignNewWallet)
            verifiedPaystackTransactionResponse = await _payStackSdk.VerifyTransaction(clientReference);
        else
            verifiedPaystackTransactionResponse = await _payStackSdk.VerifyChargeTransaction(clientReference);

        if (!verifiedPaystackTransactionResponse.WasSuccessful)
            throw new BadRequest("Paystack transaction could not be verified.");

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<VerifiedTransaction>();
        resp.PaystackTransaction = verifiedPaystackTransactionResponse.Data!;

        return resp;
    }

    // step three
    public VerifiedTransaction InitializeTransactionAsync(VerifiedTransaction context)
    {
        // if this is not the first time processing, update states on the transaction
        var transaction = context.PlatformTransaction;
        var verifiedPaystackTransaction = context.PaystackTransaction;

        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.TransactionRecordCreated)
        {
            transaction.Amount = verifiedPaystackTransaction.EffectiveAmount;
            transaction.AmountAfterCharges = verifiedPaystackTransaction.EffectiveAmount;
            transaction.PaymentProcessorClientReference = $"{verifiedPaystackTransaction.Id}";
            transaction.PaymentProcessorDescription = verifiedPaystackTransaction.GatewayResponse;
            transaction.PaymentProcessor = PaymentProcessor.PayStack;
            transaction.Source = new Depository();

            var paystackAuthInfo = verifiedPaystackTransaction.Authorization;
            transaction.Source.WalletAccountNumber = GetAccountNumber(paystackAuthInfo);
            transaction.Source.WalletProductId = transaction.LedgerProductId!.Value;
            transaction.Source.WalletType = paystackAuthInfo.Channel;
            transaction.Source.WalletProvider = paystackAuthInfo.Brand;
            transaction.Source.WalletAuthorizationCode = paystackAuthInfo.AuthorizationCode;

            transaction.SettledDate =
                verifiedPaystackTransaction.PaidAt ?? verifiedPaystackTransaction.TransactionDate;

            // record this step and set transaction status to in progress to avoid contention
            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.VerifiedPaymentFromPaymentProcessor);

            // set transaction status to in progress to avoid contention
            transaction.TransactionStatus = TransactionStatus.InProgress;
        }
        else
        {
            // set transaction status to in progress to avoid contention
            transaction.TransactionStatus = TransactionStatus.InProgress;
        }

        return context;
    }

    // step 4
    public async Task<T> SaveTransactionAsync<T>(T context) where T : VerifiedTransaction
    {
        var transaction = context.PlatformTransaction;

        var updatedTransaction = await _dbRepository.UpdateAsync(transaction);
        context.PlatformTransaction = updatedTransaction;

        return context;
    }

    // step 5
    public async Task<VerifiedTransactionWithOptionalWallet> RetrieveTransactionWallet(VerifiedTransaction context)
    {
        var transaction = context.PlatformTransaction;
        var paystackAuthInfo = context.PaystackTransaction.Authorization;

        // record this stage if we just now verified payment
        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.VerifiedPaymentFromPaymentProcessor)
            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.RetrievingWalletFromDatabase);

        Wallet? existingWallet;

        // if we are now trying to retrieve the wallet 
        lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.RetrievingWalletFromDatabase)
        {
            var email = GetTransactionEmail(transaction);
            string actioningEntityId = transaction.Brand?.Id ?? transaction.Distributor!.Id;

            existingWallet = await _dbRepository.GetAsync<Wallet>(b => b
                .Include(e => e.Email)
                .Where(e => e.Email.Address == email.Address &&
                            (transaction.Brand == null ||
                             transaction.Brand.Id == actioningEntityId) && // brand topping up
                            (transaction.Distributor == null ||
                             transaction.Distributor.Id == actioningEntityId) && // distributor topping up
                            e.AccountNumber.Equals(transaction.Source!.WalletAccountNumber) &&
                            e.Active))
                .IgnoreAndDefault<NotFound, Wallet>();

            if (existingWallet != null)
            {
                transaction.Source!.Id = existingWallet.Id;
                transaction.Source.WalletAccountId = existingWallet.AccountId;
                transaction.LedgerAccountId = transaction.Source.WalletAccountId;
                transaction.Source.WalletRecipientCode = existingWallet.RecipientCode!;
                transaction.Source.WalletProviderLogoUrl = existingWallet.ProviderLogoUrl;

                AddTransactionStep(context,
                    lastStep.Description,
                    TransactionProcessingStep.RetrievedWalletFromDatabase);
            }
        }
        // if we've already retrieve the wallet so reconstruct
        else
        {
            existingWallet = new Wallet();
            var email = GetTransactionEmail(transaction);
            existingWallet.Email = email;
            existingWallet.Brand = transaction.Brand;
            existingWallet.Distributor = transaction.Distributor;
            existingWallet.ProductId = transaction.LedgerProductId!.Value;

            existingWallet.AccountId = transaction.Source!.WalletAccountId;
            existingWallet.AccountNumber = GetAccountNumber(paystackAuthInfo);
            var accountName = GetTransactionAccountNumber(transaction);
            existingWallet.AccountName = accountName;
            existingWallet.Type = paystackAuthInfo.Channel;
            existingWallet.Provider = paystackAuthInfo.Brand;
            existingWallet.AuthorizationCode = paystackAuthInfo.AuthorizationCode;
            existingWallet.RecipientCode = transaction.Source.WalletRecipientCode;
        }

        // ALWAYS do this adaption after all your functional logic
        var transactionWithExistingWallet = context.Adapt<VerifiedTransactionWithOptionalWallet>();
        transactionWithExistingWallet.Wallet = existingWallet;

        return transactionWithExistingWallet;
    }

    private static string? GetTransactionAccountNumber(DistrbuteTransaction transaction)
    {
        var accountName = transaction.Brand?.Name ?? transaction.Distributor?.Name; // depending on who made the trans
        return accountName;
    }

    private static Email GetTransactionEmail(DistrbuteTransaction transaction)
    {
        var email = transaction.Brand?.Email ?? transaction.Distributor?.Email;
        return email!;
    }

    // step 5 (TIE)
    // ledger account
    public CreateLedgerAccount CreateLedgerAccountRequest(VerifiedTransaction context)
    {
        var transaction = context.PlatformTransaction;

        // if we couldn't retrieve wallet from DB, record this stage
        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.RetrievingWalletFromDatabase)
            AddTransactionStep(context,
                TransactionProcessingStep.RetrievingWalletFromDatabase,
                TransactionProcessingStep.CreatingNewWalletOnLedger);

        // create a wallet on ledger
        var payload = new CreateWalletRequest();
        payload.ClientId = transaction.LedgerClientId!.Value;
        payload.SavingsProductId = transaction.LedgerProductId!.Value;

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<CreateLedgerAccount>();
        resp.LedgerReq = payload;

        return resp;
    }

    public async Task<CreatedLedgerAccount> SaveLedgerAccountAsync(CreateLedgerAccount context)
    {
        var transaction = context.PlatformTransaction;
        var payload = context.LedgerReq;

        BaseFineractResponseDto value;

        var lastStep = transaction.Steps.LastOrDefault();
        // if we're now creating the wallet
        if (lastStep?.Description is TransactionProcessingStep.CreatingNewWalletOnLedger)
        {
            value = await _walletSdk
                .CreateAsync(payload)
                .CatchAndThrowAsOrReturn<LedgerException, FailedDependency, BaseFineractResponseDto>
                    ("We're having trouble saving your wallet information. Please try again in a few minutes.");

            transaction.Source!.WalletAccountId = value.SavingsId;
            transaction.LedgerAccountId = value.SavingsId;

            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.CreatedNewWalletOnLedger);
        }
        // if it's a retried or repeat transaction so reconstruct
        else
        {
            value = new BaseFineractResponseDto();
            value.SavingsId = transaction.Source!.WalletAccountId;
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<CreatedLedgerAccount>();
        resp.LedgerRes = value;

        return resp;
    }

    // transfer recipient
    public CreateTransferRecipientReq CreatePaystackRecipientRequest(CreatedLedgerAccount context)
    {
        var transaction = context.PlatformTransaction;
        var paystackTransaction = context.PaystackTransaction;

        // if the last step was creating the wallet on ledger
        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.CreatedNewWalletOnLedger)
            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.CreatingNewRecipientOnPaymentProcessor);

        var payload = new CreateRecipient();
        payload.Type = TransferRecipientType.Authorization;
        var email = GetTransactionEmail(transaction);
        payload.Email = email.Address;
        var accountName = GetTransactionAccountNumber(transaction);
        payload.Name = accountName;
        payload.AuthorizationCode = paystackTransaction.Authorization.AuthorizationCode;

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<CreateTransferRecipientReq>();
        resp.TransferRecipientReq = payload;

        return resp;
    }

    public async Task<CreatedTransferRecipient> SavePaystackRecipientAsync(CreateTransferRecipientReq context)
    {
        var transaction = context.PlatformTransaction;
        var transferRecipientReq = context.TransferRecipientReq;

        TransferRecipient value;

        var lastStep = transaction.Steps.LastOrDefault();
        // first time
        if (lastStep?.Description is TransactionProcessingStep.CreatingNewRecipientOnPaymentProcessor)
        {
            var transferRecipientResponse = await _payStackSdk.CreateTransferRecipient(transferRecipientReq);
            value = transferRecipientResponse.Data;

            transaction.Source!.WalletRecipientCode = value.RecipientCode;

            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.CreatedNewRecipientOnPaymentProcessor);
        }
        // retried or repeat transaction so reconstruct
        else
        {
            value = new TransferRecipient();
            value.RecipientCode = transaction.Source!.WalletRecipientCode;
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<CreatedTransferRecipient>();
        resp.TransferRecipientRes = value;

        return resp;
    }

    // wallet
    public CreateWalletReq CreateNewWalletRequest(CreatedTransferRecipient context)
    {
        var transaction = context.PlatformTransaction;
        var paystackAuthInfo = context.PaystackTransaction.Authorization;

        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.CreatedNewRecipientOnPaymentProcessor)
            AddTransactionStep(context,
                TransactionProcessingStep.CreatedNewRecipientOnPaymentProcessor,
                TransactionProcessingStep.CreatingNewWalletInDb);

        var wallet = new Wallet();
        var email = GetTransactionEmail(transaction);
        wallet.Email = email;
        wallet.Brand = transaction.Brand;
        wallet.Distributor = transaction.Distributor;
        wallet.ProductId = transaction.LedgerProductId!.Value;

        wallet.AccountId = context.LedgerRes.SavingsId;
        wallet.AccountNumber = GetAccountNumber(paystackAuthInfo);
        var accountName = GetTransactionAccountNumber(transaction);
        wallet.AccountName = accountName;
        wallet.Type = paystackAuthInfo.Channel;
        wallet.Provider = paystackAuthInfo.Brand;
        wallet.AuthorizationCode = paystackAuthInfo.AuthorizationCode;
        wallet.RecipientCode = context.TransferRecipientRes.RecipientCode;
        wallet.Verified = true;
        wallet.Active = true;

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<CreateWalletReq>();
        resp.WalletReq = wallet;

        return resp;
    }

    public async Task<VerifiedTransactionWithResolvedWallet> SaveNewWalletAsync(CreateWalletReq context)
    {
        var transaction = context.PlatformTransaction;
        var req = context.WalletReq;
        var paystackAuthInfo = context.PaystackTransaction.Authorization;
        Wallet savedWallet;

        var lastStep = transaction.Steps.LastOrDefault();
        // first time
        if (lastStep?.Description is TransactionProcessingStep.CreatingNewWalletInDb)
        {
            savedWallet = await _dbRepository.AddAsync(req);

            transaction.Source!.Id = req.Id;
            transaction.Source.WalletProviderLogoUrl = req.ProviderLogoUrl;

            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.CreatedNewWalletInDb);
        }
        // we've already retrieve the wallet once before so reconstruct
        else
        {
            savedWallet = new Wallet();
            savedWallet.Id = transaction.Source!.Id;
            var email = GetTransactionEmail(transaction);
            savedWallet.Email = email;
            savedWallet.Brand = transaction.Brand;
            savedWallet.Distributor = transaction.Distributor;
            savedWallet.ProductId = transaction.LedgerProductId!.Value;

            savedWallet.AccountId = transaction.Source!.WalletAccountId;
            savedWallet.AccountNumber = GetAccountNumber(paystackAuthInfo);
            var accountName = GetTransactionAccountNumber(transaction);
            savedWallet.AccountName = accountName;
            savedWallet.Type = paystackAuthInfo.Channel;
            savedWallet.Provider = paystackAuthInfo.Brand;
            savedWallet.AuthorizationCode = paystackAuthInfo.AuthorizationCode;
            savedWallet.RecipientCode = transaction.Source.WalletRecipientCode;
            savedWallet.ProviderLogoUrl = transaction.Source.WalletProviderLogoUrl;
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<VerifiedTransactionWithResolvedWallet>();
        resp.Wallet = savedWallet;

        return resp;
    }

    // step 6
    public VerifiedTransactionWithResolvedWallet CreateDepositLockRequest(VerifiedTransactionWithResolvedWallet context)
    {
        var transaction = context.PlatformTransaction;

        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.CreatedNewWalletInDb
            or TransactionProcessingStep.RetrievedWalletFromDatabase)
            transaction.TransactionStatus = TransactionStatus.LockedForDeposit; // Idempotent key

        return context;
    }

    // step 7
    public async Task<DepositLedgerAccountReq> CreateLedgerAccountDepositRequest(
        VerifiedTransactionWithResolvedWallet context)
    {
        var clientReference = context.ClientReference;
        var accountId = context.Wallet.AccountId;
        var transaction = context.PlatformTransaction;

        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep?.Description is TransactionProcessingStep.RetrievedWalletFromDatabase or
            TransactionProcessingStep.CreatedNewWalletInDb)
            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.DepositingToWallet);

        var depositReq = new DepositRequest();
        depositReq.AccountId = accountId;
        depositReq.ReceiptNumber = clientReference;
        depositReq.TransactionAmount = transaction.AmountAfterCharges.GetValueOrDefault(0);
        depositReq.TransactionDate = DateTime.UtcNow;

        // check if it's a distributor, if it is, deposit to their suspense wallet
        if (transaction.TransactionType == TransactionType.NewWallet && transaction.Distributor != null)
        {
            var email = GetTransactionEmail(transaction);
            
            var suspenseWallet = await _dbRepository
                .GetAsync<SuspenseWallet>(q => q
                    .Include(e => e.Email)
                    .Where(e => e.Email.Address == email.Address)
                    .Include(e => e.Distributor)
                    .Where(e => e.Distributor.Id == transaction.Distributor.Id));

            depositReq.AccountId = suspenseWallet.AccountId;
        }

        // ALWAYS do this adaption after all your functional logic
        var resp = context.Adapt<DepositLedgerAccountReq>();
        resp.DepositReq = depositReq;

        return resp;
    }

    // step 8
    public async Task<DepositedLedgerAccount> ExecuteDepositAsync(DepositLedgerAccountReq context)
    {
        var depositReq = context.DepositReq;
        var transaction = context.PlatformTransaction;

        var lastStep = transaction.Steps.LastOrDefault();
        var shouldExecute = lastStep?.Description is TransactionProcessingStep.DepositingToWallet &&
                            transaction.TransactionStatus == TransactionStatus.LockedForDeposit;
        if (shouldExecute)
            try
            {
                var depositResp = await _walletSdk.DepositAsync(depositReq);

                transaction.LedgerActionId = depositResp.ActionId;

                AddTransactionStep(context,
                    lastStep!.Description,
                    TransactionProcessingStep.DepositedToWallet);

                // ALWAYS do this adaption after all your functional logic
                var resp = context.Adapt<DepositedLedgerAccount>();
                resp.DepositRes = depositResp;

                return resp;
            }
            catch (Exception)
            {
            }

        // ALWAYS do this adaption after all your functional logic
        var default_ = context.Adapt<DepositedLedgerAccount>();
        return default_;
    }

    // step 9
    public DepositedLedgerAccount DetermineFinalTransactionState(DepositedLedgerAccount context)
    {
        var transaction = context.PlatformTransaction;

        var lastStep = transaction.Steps.LastOrDefault();

        // already successful transaction
        if (lastStep?.Description is TransactionProcessingStep.Successful)
        {
            transaction.TransactionStatus = TransactionStatus.Successful;
        }
        // transaction that just became successful 
        else if (lastStep?.Description is TransactionProcessingStep.DepositedToWallet)
        {
            AddTransactionStep(context,
                lastStep.Description,
                TransactionProcessingStep.Successful);

            transaction.TransactionStatus = TransactionStatus.Successful;
        }
        // if something went wrong
        else
        {
            transaction.TransactionStatus = TransactionStatus.Challenged;
        }

        return context;
    }

    // step 10
    public CheckStatusResponse CheckStatus<T>(T context) where T : RetrievedTransaction
    {
        var transaction = context.PlatformTransaction;

        var resp = new CheckStatusResponse();
        resp.Amount = transaction.Amount.GetValueOrDefault(0);
        resp.TransactionStatus = transaction.TransactionStatus;
        resp.ClientReference = transaction.ClientReference;
        resp.AccountId = transaction.LedgerAccountId;
        resp.ProductId = transaction.LedgerProductId;

        return resp;
    }

    private void AddTransactionStep<T>(
        T context,
        TransactionProcessingStep previousStep,
        TransactionProcessingStep newStep) where T : VerifiedTransaction
    {
        var transaction = context.PlatformTransaction;

        if (previousStep == TransactionProcessingStep.Default) throw new BadRequest("Step argument out of range");

        // validate precedence
        var lastStep = transaction.Steps.LastOrDefault();
        if (lastStep == null || lastStep.Description != previousStep)
            throw new BadRequest(
                $"Cannot set transaction progress, Invalid last step: {lastStep?.Description}");

        // add steps
        if (transaction.Steps.Any(s => s.Description == newStep))
            throw new BadRequest($"Cannot set transaction progress, step: {newStep} already exists");

        transaction.AddStep(newStep);
    }

    private static string GetAccountNumber(PaystackAuthorization auth)
    {
        var def = $"{auth.Bin}{auth.Last4}";
        if (PaymentChannel.mobile_money == auth.Channel)
        {
            var mobileNum = auth.MobileMoneyNumber;
            var effectiveMomo = string.IsNullOrEmpty(mobileNum) ? def : mobileNum;
            return effectiveMomo;
        }

        return def;
    }
}
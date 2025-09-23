using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Dtos;
using App.Distrbute.Api.Common.Dtos.Distributor;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Ledgr.Sdk.Dtos;
using Ledgr.Sdk.Dtos.Wallet.Balance;
using Ledgr.Sdk.Dtos.Wallet.Create;
using Ledgr.Sdk.Exceptions;
using Ledgr.Sdk.Services.Interfaces;
using Mapster;
using Microsoft.Extensions.Options;
using ObjectStorage.Sdk.Dtos;
using ObjectStorage.Sdk.Services.Interfaces;
using Paystack.Sdk.Dtos;
using Paystack.Sdk.Options;
using Paystack.Sdk.Services.Interfaces;
using Persistence.Sdk.Core.Interfaces;
using Persistence.Sdk.Dtos;
using Persistence.Sdk.Models;
using Utility.Sdk.Dtos;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Distributor.Api.Services.Providers;

public class DistributorService : IDistributorService
{
    private readonly IDbRepository _dbRepository;
    private readonly IMediaProcessor _mediaProcessor;
    private readonly IObjectStorageSdk _objectStorage;
    private readonly PayStackConfig _paystackConfig;
    private readonly IPayStackSdk _payStackSdk;
    private readonly IPipelineProvider _pipelineProvider;
    private readonly SavingsProductConfig _savingsProductConfig;
    private readonly IWalletSdk _walletSdk;

    public DistributorService(IDbRepository dbRepository,
        IWalletSdk walletSdk,
        IOptions<SavingsProductConfig> savingsProductConfig,
        IPayStackSdk payStackSdk,
        IOptions<PayStackConfig> paystackConfig,
        IObjectStorageSdk objectStorage,
        IMediaProcessor mediaProcessor,
        IPipelineProvider pipelineProvider)
    {
        _dbRepository = dbRepository;
        _walletSdk = walletSdk;
        _savingsProductConfig = savingsProductConfig.Value;
        _payStackSdk = payStackSdk;
        _paystackConfig = paystackConfig.Value;
        _objectStorage = objectStorage;
        _mediaProcessor = mediaProcessor;
        _pipelineProvider = pipelineProvider;
    }

    public async Task<IApiResponse<DistributorDto>> CreateAsync(Email principal, CreateDistributorDto dto)
    {
        // block multiple distributors
        await _dbRepository
            .ExistsAsync<Common.Models.Distributor>(q => q
                .Include(e => e.Email)
                .Where(e => e.Email.Address == principal.Address))
            .ThrowIfTrue<BadRequest>("Distributor already exists for this email");

        // create hold wallet on ledger
        var ledgerReq = new CreateWalletRequest();
        ledgerReq.ClientId = principal.LedgerClientId;
        ledgerReq.SavingsProductId = _savingsProductConfig.SuspenseSavingsProduct;
        var ledgerResponse = await _walletSdk
            .CreateAsync(ledgerReq)
            .CatchAndThrowAsOrReturn<LedgerException, FailedDependency, BaseFineractResponseDto>
                ("We're having trouble creating a suspense wallet for you. Please try again in a few minutes.");

        var distributor = dto.Adapt(new Common.Models.Distributor());
        distributor.Id = Guid.NewGuid().ToString();
        distributor.Email = principal;
        distributor.OpenToCollaboration = true;

        if (distributor.ProfilePicture != null)
        {
            var attachment = await GetAttachment(principal, distributor.ProfilePicture.Id);
            distributor.ProfilePicture = attachment;
        }

        // Create wallet in db
        var holdWallet = new SuspenseWallet();
        holdWallet.Id = Guid.NewGuid().ToString();
        holdWallet.Distributor = distributor;
        holdWallet.ProductId = ledgerReq.SavingsProductId;
        holdWallet.AccountId = ledgerResponse.SavingsId;
        holdWallet.ClientId = principal.LedgerClientId;
        holdWallet.AccountName = dto.Name;

        var entities = new List<BaseModel> { holdWallet, distributor };
        var savedEntities = await _dbRepository.AddManyAsync(entities);

        var savedDistributor =
            (Common.Models.Distributor)savedEntities.Find(e => e.Id.Equals(distributor.Id))!;
        
        if (distributor.ProfilePicture != null)
        {
            await ExecuteMediaProcessingPipeline(principal, distributor.ProfilePicture);
        }

        var response = savedDistributor.Adapt(new DistributorDto());
        response.Email = savedDistributor.Email.Address;

        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<DistributorDto>> UpdateAsync(Email principal, CreateDistributorDto dto)
    {
        var existingDistributor = await GetDistributor(principal);

        var profilePictureChanged = dto.ProfilePicture?.Id != null &&
                                    !dto.ProfilePicture.Id.Equals(existingDistributor.ProfilePicture?.Id);

        var update = dto.Adapt(existingDistributor);

        if (profilePictureChanged)
        {
            var attachment = await GetAttachment(principal, update.ProfilePicture!.Id);
            update.ProfilePicture = attachment;
        }

        var updated = await _dbRepository.UpdateAsync(update);

        var response = updated.Adapt(new DistributorDto());
        response.Email = updated.Email.Address;
        
        if (profilePictureChanged && updated.ProfilePicture != null)
        {
            await ExecuteMediaProcessingPipeline(principal, updated.ProfilePicture);
        }

        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<DistributorDto>> GetAsync(Email principal)
    {
        var existingDistributor = await GetDistributor(principal);

        var response = existingDistributor.Adapt(new DistributorDto());
        response.Email = existingDistributor.Email.Address;

        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<WalletResDto>> GetEarned(Email principal)
    {
        var existingDistributor = await GetDistributor(principal);

        var suspenseAccount = await _dbRepository.GetAsync<SuspenseWallet>(q => q
            .Include(e => e.Distributor)
            .Where(e =>  e.Distributor.Id == existingDistributor.Id));

        var ledgerBalance = await _walletSdk
            .GetBalanceAsync(suspenseAccount.AccountId)
            .CatchAndThrowAsOrReturn<LedgerException, FailedDependency, WalletBalanceDto>
                ("We're having trouble processing request on your wallet. Please try again in a few minutes.");

        var availableBalance = ledgerBalance.AvailableBalance;
        var balance = ledgerBalance.AccountBalance;
        var response = suspenseAccount.ToResponseDto(availableBalance, balance);

        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<PagedResult<DistrbuteTransactionDto>>> AllTransactionAsync(Email principal,
        TransactionPageRequest filter)
    {
        // Fetch all from PG
        var existingDistributor = await GetDistributor(principal);

        var paged = await _dbRepository.GetAllAsync<DistrbuteTransaction>(
                q => q
                         .Where(e => e.Distributor != null)
                         .IncludeWith(e => e.Distributor, d => d!.Email)
                         .Where(e => e.Distributor!.Email.Address == principal.Address && e.Distributor.Id == existingDistributor.Id && (filter.Type == null) || (filter.Type.Equals(e.TransactionType) && filter.Status == null) || filter.Status.Equals(e.TransactionStatus))
                         .Page(filter));

        var response = new PagedResult<DistrbuteTransactionDto>
        {
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = paged.TotalCount,
            Data = paged.Data.ConvertAll(c =>
            {
                var dto = c.Adapt<DistrbuteTransactionDto>();
                dto.BrandId = c.Brand?.Id;
                dto.BrandName = c.Brand?.Name;
                dto.DistributorId = c.Distributor?.Id;
                dto.DistributorName = c.Distributor?.Name;
                dto.AccountName = dto.BrandName ?? dto.DistributorName;
                
                return dto;
            })
        };

        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<InitializeTransactionDto>> LinkWalletAsync(Email principal, string? callbackUrl)
    {
        var existingDistributor = await GetDistributor(principal);

        var amount = 1;

        // Call paystack to initialize transaction
        var clientReference = Guid.NewGuid().ToString();
        var initializeTransactionReq = new InitializeTransaction();
        initializeTransactionReq.Email = principal.Address;
        initializeTransactionReq.Reference = clientReference;
        initializeTransactionReq.Amount = amount * 100; // paystack needs extra zeros
        initializeTransactionReq.Currency = _paystackConfig.Currency;
        initializeTransactionReq.CallbackUrl = callbackUrl;
        initializeTransactionReq.Channels = _paystackConfig.Channels;
        var initializeTransactionResponse = await _payStackSdk
            .InitializeTransaction(initializeTransactionReq)
            .CatchAndThrowAsOrReturn<LedgerException, FailedDependency, InitializeTransactionResponse>
                ("We're having trouble processing request on your wallet. Please try again in a few minutes.");

        // Create transaction record only and return, actual wallet topup will occur after payment callback
        var transaction = new DistrbuteTransaction();
        var integrationChannel = CommonConstants.IntegrationChannel.DistributorPortal;
        var savingsProductId = _savingsProductConfig.DistributorSavingsProduct;
        var source = new Depository();
        source.WalletProductId = savingsProductId;
        source.WalletClientId = principal.LedgerClientId;
        transaction.Source = source;
        transaction.Distributor = existingDistributor;
        transaction.Description = $"Funds topup of GHS {amount:N2} initiated by Distributor for linking wallet";
        transaction.IntegrationChannel = integrationChannel;
        transaction.TransactionType = TransactionType.NewWallet;
        transaction.ClientReference = clientReference;
        transaction.Amount = amount;
        transaction.AmountAfterCharges = amount; // the charge is to the customer by PayStack and so doesn't concern us
        transaction.Charges = 0;
        transaction.AmountDueDistrbute = 0;
        transaction.PaymentProcessor = PaymentProcessor.PayStack;
        transaction.PaymentProcessorClientReference = initializeTransactionResponse.Data.AccessCode;
        transaction.PaymentProcessorDescription = initializeTransactionResponse.Data.AuthorizationUrl;
        transaction.TransactionStatus = TransactionStatus.Pending;
        transaction.TransactionDate = DateTime.UtcNow;
        transaction.AddStep(TransactionProcessingStep.TransactionRecordCreated);

        await _dbRepository.AddAsync(transaction);

        var paystackResp = initializeTransactionResponse.Data;
        var response = paystackResp.Adapt<InitializeTransactionDto>();

        return response.ToOkApiResponse();
    }

    private async Task<DocumentFile> GetAttachment(Email principal, string id)
    {
        try
        {
            var metadata = await _objectStorage.GetObjectMetadataAsync(id, principal.Address);
            var uploadDate = metadata.UploadedAt;
            var options = CommonConstants.DistrbuteMediaProcessingOptions();
            var otherFormats = _mediaProcessor.WhatWouldBeGenerated(options, metadata.Filename);
            var thumbnailUrl = _objectStorage.GetObjectUrlFromFilename(
                metadata.Filename,
                otherFormats.ThumbnailFilename,
                CommonConstants.PROFILE_PICTURE_STORAGE_PREFIX,
                uploadDate);
            var otherFormatUrls = otherFormats.OtherFormatsFilenames.ToDictionary(
                k => k.Key,
                v => _objectStorage.GetObjectUrlFromFilename
                (metadata.Filename,
                    v.Value,
                    CommonConstants.PROFILE_PICTURE_STORAGE_PREFIX,
                    uploadDate));

            var attachment = new DocumentFile();
            attachment.Id = id;
            attachment.Size = metadata.Size;
            attachment.FileType = metadata.FileType;
            attachment.UploadedAt = uploadDate;
            attachment.Filename = metadata.Filename;
            attachment.Thumbnail = thumbnailUrl;
            attachment.Url = metadata.Url;
            attachment.SizeReadable = metadata.SizeReadable;
            attachment.OtherFormats = otherFormatUrls;

            return attachment;
        }
        catch (FileNotFoundException)
        {
            throw new BadRequest(
                $"We couldn’t find any uploaded file matching the key: {id}. Please make sure you’ve uploaded your file and try again."
            );
        }
        catch (Exception ex)
        {
            throw new FailedDependency(
                "Something went wrong while retrieving your file. Please try again in a moment.", ex
            );
        }
    }

    private async Task ExecuteMediaProcessingPipeline(Email principal, DocumentFile attachment)
    {
        var options = CommonConstants.DistrbuteMediaProcessingOptions();
        options.InputPath = attachment.Url;
        options.CreationDate = attachment.UploadedAt;

        await _pipelineProvider.ExecuteMediaProcessingPipeline(principal, options,
            CommonConstants.PROFILE_PICTURE_STORAGE_PREFIX);
    }

    private async Task<Common.Models.Distributor> GetDistributor(Email principal)
    {
        var existingDistributor = await _dbRepository
            .GetAsync<Common.Models.Distributor>(q => q
                .Include(e => e.Email)
                .Where(e => e.Email.Address == principal.Address));

        return existingDistributor;
    }
}
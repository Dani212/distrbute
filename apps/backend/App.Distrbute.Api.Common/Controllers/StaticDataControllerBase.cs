using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Paystack.Sdk.Enums;
using Persistence.Sdk.Core.Interfaces;
using Socials.Sdk.Enums;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Controllers;

public class StaticDataControllerBase : CustomControllerBase
{
    private readonly IDbRepository _dbRepository;

    public StaticDataControllerBase(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }
    
    [HttpGet("niches")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<NicheDto>>))]
    public async Task<IActionResult> AllNiches()
    {
        User.GetAccount(); // ensure authenticated 

        var niches = await _dbRepository.GetManyAsync<BrandNiche>(q => q
            .Where(e => true));
        
        var adapted = niches.Select(e => e.Adapt<NicheDto>()).ToList();

        var response = adapted.ToOkApiResponse();
        return ToActionResult(response);
    }
    
    [HttpGet("distributor-niches")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<NicheDto>>))]
    public async Task<IActionResult> AllDistributorNiches()
    {
        User.GetAccount(); // ensure authenticated 
        
        var niches = await _dbRepository.GetManyAsync<DistributorNiche>(q => q
            .Where(e => true));
        
        var adapted = niches.Select(e => e.Adapt<NicheDto>()).ToList();

        var response = adapted.ToOkApiResponse();
        return ToActionResult(response);
    }

    [HttpGet("platforms")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<Platform>>))]
    public async Task<IActionResult> AllPlatform()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(Platform))
            .Cast<Platform>()
            .Where(t => t != Platform.Default)
            .ToList().ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("campaign-statuses")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<CampaignStatus>>))]
    public async Task<IActionResult> AllCampaignStatuses()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(CampaignStatus))
            .Cast<CampaignStatus>()
            .Where(t => t != CampaignStatus.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("campaign-types")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<CampaignType>>))]
    public async Task<IActionResult> AllCampaignTypes()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(CampaignType))
            .Cast<CampaignType>()
            .Where(t => t != CampaignType.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("campaign-sub-types")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<CampaignSubType>>))]
    public async Task<IActionResult> AllCampaignSubTypes()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(CampaignSubType))
            .Cast<CampaignSubType>()
            .Where(t => t != CampaignSubType.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("content-types")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<ContentType>>))]
    public async Task<IActionResult> AllContentTypes()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(ContentType))
            .Cast<ContentType>()
            .Where(t => t != ContentType.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("transaction-statuses")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<TransactionStatus>>))]
    public async Task<IActionResult> AllTransactionStatus()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(TransactionStatus))
            .Cast<TransactionStatus>()
            .Where(t => t != TransactionStatus.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("transaction-types")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<TransactionType>>))]
    public async Task<IActionResult> AllTransactionTypes()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(TransactionType))
            .Cast<TransactionType>()
            .Where(t => t != TransactionType.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("payment-processors")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<PaymentProcessor>>))]
    public async Task<IActionResult> AllPaymentProcessors()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(PaymentProcessor))
            .Cast<PaymentProcessor>()
            .Where(t => t != PaymentProcessor.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("wallet-types")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<PaymentChannel>>))]
    public async Task<IActionResult> AllWalletTypes()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(PaymentChannel))
            .Cast<PaymentChannel>()
            .Where(t => t != PaymentChannel.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("wallet-providers")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<PaymentChannelProvider>>))]
    public async Task<IActionResult> AllWalletProviders()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(PaymentChannelProvider))
            .Cast<PaymentChannelProvider>()
            .Where(t => t != PaymentChannelProvider.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }

    [HttpGet("timeframes")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<Timeframe>>))]
    public async Task<IActionResult> AllTimeframes()
    {
        User.GetAccount(); // ensure authenticated 
        
        var response = Enum.GetValues(typeof(Timeframe))
            .Cast<Timeframe>()
            .Where(t => t != Timeframe.Default)
            .ToList()
            .ToOkApiResponse();

        await Task.CompletedTask;
        return ToActionResult(response);
    }
}
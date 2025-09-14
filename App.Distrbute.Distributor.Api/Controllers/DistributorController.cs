using System.Threading.Tasks;
using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Api.Common.Dtos;
using App.Distrbute.Api.Common.Dtos.Distributor;
using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Common;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObjectStorage.Sdk.Services.Interfaces;
using Paystack.Sdk.Dtos;
using Persistence.Sdk.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
[Route("v{version:apiVersion}")]
[Authorize(AuthenticationSchemes = CommonConstants.AuthScheme.BEARER)]
public class DistributorController : BaseBrandDistributorController
{
    private readonly IDistributorService _distributorService;

    public DistributorController(IDistributorService distributorService, IObjectStorageService fileService) :
        base(fileService)
    {
        _distributorService = distributorService;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create resource with provided request"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<DistributorDto>))]
    public async Task<IActionResult> Create([FromBody] CreateDistributorDto dto)
    {
        var principal = User.GetAccount();
        var response = await _distributorService.CreateAsync(principal, dto);
        return ToActionResult(response);
    }

    [HttpPut]
    [SwaggerOperation(
        Summary = "Update existing resource by it's ID"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<DistributorDto>))]
    public async Task<IActionResult> Update([FromBody] CreateDistributorDto dto)
    {
        var principal = User.GetAccount();
        var response = await _distributorService.UpdateAsync(principal, dto);
        return ToActionResult(response);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get existing resource by it's ID"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<DistributorDto>))]
    public async Task<IActionResult> Get()
    {
        var principal = User.GetAccount();
        var response = await _distributorService.GetAsync(principal);
        return ToActionResult(response);
    }

    [HttpGet("earned")]
    [SwaggerOperation(
        Summary = "Get a distributors's total earned balance"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<WalletResDto>))]
    public async Task<IActionResult> GetDistributorEarnings()
    {
        var principal = User.GetAccount();
        var response = await _distributorService.GetEarned(principal);
        return ToActionResult(response);
    }

    [HttpGet("transactions")]
    [SwaggerOperation(
        Summary = "Get all instances of transaction of brand that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PagedResult<DistrbuteTransactionDto>>))]
    public async Task<IActionResult> All([FromQuery] TransactionPageRequest filter)
    {
        var principal = User.GetAccount();
        var response = await _distributorService.AllTransactionAsync(principal, filter);

        return ToActionResult(response);
    }

    [HttpPost("link-wallet")]
    [SwaggerOperation(
        Summary = "Link a new wallet"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<TransactionAuthorizationData>))]
    public async Task<IActionResult> InitiateDepositAsync([FromQuery] string? callbackUrl)
    {
        var principal = User.GetAccount();
        var response = await _distributorService.LinkWalletAsync(principal, callbackUrl);
        return ToActionResult(response);
    }
}
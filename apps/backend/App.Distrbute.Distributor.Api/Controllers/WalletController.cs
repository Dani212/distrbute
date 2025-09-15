using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Api.Common.Dtos;
using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common;
using App.Distrbute.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Sdk.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;
using WalletPageRequest = App.Distrbute.Api.Common.Dtos.WalletPageRequest;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = CommonConstants.AuthScheme.BEARER)]
public class WalletController : CustomControllerBase
{
    private readonly IPipelineProvider _pipelineProvider;
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService, IPipelineProvider pipelineProvider)
    {
        _walletService = walletService;
        _pipelineProvider = pipelineProvider;
    }

    [HttpGet("check-status")]
    [SwaggerOperation(
        Summary = "Check the status of an existing transaction"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<CheckStatusResponse>))]
    public async Task<IActionResult> CheckStatusAsync([FromQuery] string clientReference)
    {
        var principal = User.GetAccount();
        var response = await _pipelineProvider.ExecuteDepositProcessingPipeline(clientReference);
        return ToActionResult(response.ToOkApiResponse());
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get existing wallet by it's account number"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<WalletResDto>))]
    public async Task<IActionResult> Get([FromRoute] [Required] string id)
    {
        var principal = User.GetAccount();
        var req = new WalletQueryRequest();
        req.TenantId = principal.Id;
        var response = await _walletService.GetAsync<Common.Models.Distributor>(principal, id, req);
        return ToActionResult(response);
    }

    [HttpPost("{id}/withdraw")]
    [SwaggerOperation(
        Summary = "Withdraw an amount from a wallet"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<WalletResDto>))]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawFromWallet dto, [FromRoute] [Required] string id)
    {
        var principal = User.GetAccount();
        return ToActionResult(new WalletResDto().ToOkApiResponse());
    }

    [HttpGet("all")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PagedResult<WalletResDto>>))]
    public async Task<IActionResult> All([FromQuery] WalletPageRequest filter)
    {
        var principal = User.GetAccount();
        var response = await _walletService.AllAsync<Common.Models.Distributor>(principal, filter);

        return ToActionResult(response);
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Common;
using App.Distrbute.Distributor.Api.Dtos;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Sdk.Dtos;
using Socials.Sdk;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;
using Utility.Sdk.Exceptions;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = CommonConstants.AuthScheme.BEARER)]
public class SocialsController : CustomControllerBase
{
    private readonly IDistributorService _distributorService;
    private readonly ISocialAccountService _socialAccountService;

    public SocialsController(IDistributorService distributorService,
        ISocialAccountService socialAccountService)
    {
        _distributorService = distributorService;
        _socialAccountService = socialAccountService;
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get a social account"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<SocialAccountDto>))]
    public async Task<IActionResult> GetAsync([FromRoute] string id)
    {
        var principal = User.GetAccount();
        var response = await _socialAccountService.GetAsync(principal, id);

        return ToActionResult(response);
    }

    [HttpPut("{id}/preferences")]
    [SwaggerOperation(
        Summary = "Update ad preferences on a social account"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<SocialAccountDto>))]
    public async Task<IActionResult> UpdatePreferences([FromRoute] string id,
        [FromBody] SocialAccountPreferencesReq req)
    {
        var principal = User.GetAccount();
        var response = await _socialAccountService.UpdatePreferences(principal, id, req);

        return ToActionResult(response);
    }

    [HttpGet("connect/{provider}")]
    public async Task<IActionResult> ConnectSocial(string provider)
    {
        if (!OAuthConstants.Providers.All.Contains(provider))
            throw new BadRequest("Invalid provider specified for oauth social account connection.");

        var principal = User.GetAccount();

        var distributor = await _distributorService.GetAsync(principal);

        var userId = distributor.Data!.Id;
        var properties = new AuthenticationProperties
        {
            Parameters =
            {
                [OAuthConstants.UserIdKey] = userId
            }
        };

        // Add state to the authentication properties
        properties.Items[OAuthConstants.UserIdKey] = userId;

        return Challenge(properties, provider);
    }

    [HttpGet("connected")]
    [SwaggerOperation(
        Summary = "Get all connected social accounts"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PagedResult<SocialAccountDto>>))]
    public async Task<IActionResult> All([FromQuery] SocialAccountsPageRequest page)
    {
        var principal = User.GetAccount();
        var response = await _socialAccountService.GetConnectedAccounts(principal, page);

        return ToActionResult(response);
    }

    [HttpGet("summary")]
    [SwaggerOperation(
        Summary = "Get connected social accounts summary"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<ConnectedPlatformsSummary>>))]
    public async Task<IActionResult> Summary()
    {
        var principal = User.GetAccount();
        var response = await _socialAccountService.GetConnectedAccountsSummary(principal);

        return ToActionResult(response);
    }
}
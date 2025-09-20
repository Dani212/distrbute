using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Common;
using App.Distrbute.Distributor.Api.Dtos;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Sdk.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = CommonConstants.AuthScheme.BEARER)]
public class AdController : CustomControllerBase
{
    private readonly IPostService _postService;

    public AdController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create resource with provided request"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PostDto>))]
    public async Task<IActionResult> Create([FromBody] CreatePostReq dto)
    {
        var principal = User.GetAccount();
        var response = await _postService.CreateAsync(principal, dto);
        return ToActionResult(response);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get existing resource by it's ID"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PostDto>))]
    public async Task<IActionResult> Get([FromRoute] [Required] string id, [FromQuery] PostsQueryRequest query)
    {
        var principal = User.GetAccount();
        var response = await _postService.GetAsync(principal, id, query);
        return ToActionResult(response);
    }

    [HttpGet("all")]
    [SwaggerOperation(
        Summary = "Get all instances of existing resource that matches filter"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PagedResult<PostDto>>))]
    public async Task<IActionResult> All([FromQuery] PostPageRequest filter)
    {
        var principal = User.GetAccount();
        var response = await _postService.AllAsync(principal, filter);

        return ToActionResult(response);
    }

    [HttpGet("summary")]
    [SwaggerOperation(
        Summary = "Get campaigns summary"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<CampaignSummary>))]
    public async Task<IActionResult> SummaryAsync()
    {
        var principal = User.GetAccount();
        var response = await _postService.SummaryAsync(principal);

        return ToActionResult(response);
    }

    [HttpGet("timeseries")]
    [SwaggerOperation(
        Summary = "Get posts timeseries"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<CampaignSummary>))]
    public async Task<IActionResult> TimeseriesAsync([FromQuery] PostTimeseriesQueryRequest filter)
    {
        var principal = User.GetAccount();
        var response = await _postService.TimeseriesAsync(principal, filter);

        return ToActionResult(response);
    }
}
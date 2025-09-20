using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Common;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Sdk.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = CommonConstants.AuthScheme.BASIC)]
public class BackOfficeController : BaseBackOfficeController
{
    private readonly IDistributorNicheService _nicheService;

    public BackOfficeController(IDistributorNicheService nicheService)
    {
        _nicheService = nicheService;
    }

    [HttpPost("distributor/niche")]
    [SwaggerOperation(
        Summary = "Create a new niche"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<NicheDto>))]
    public async Task<IActionResult> Create([FromBody] NicheDto dto)
    {
        var response = await _nicheService.CreateAsync(dto);
        return ToActionResult(response);
    }

    [HttpPost("distributor/niche/bulk")]
    [SwaggerOperation(
        Summary = "Create multiple niches at once"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<List<NicheDto>>))]
    public async Task<IActionResult> CreateMany([FromBody] List<NicheDto> dtos)
    {
        var response = await _nicheService.CreateManyAsync(dtos);
        return ToActionResult(response);
    }

    [HttpPut("distributor/niche/{name}")]
    [SwaggerOperation(
        Summary = "Update an existing niche by name"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<NicheDto>))]
    public async Task<IActionResult> Update([FromRoute][Required] string name, [FromBody] NicheDto dto)
    {
        var response = await _nicheService.UpdateAsync(name, dto);
        return ToActionResult(response);
    }

    [HttpDelete("distributor/niche/{name}")]
    [SwaggerOperation(
        Summary = "Delete a niche by name"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<bool>))]
    public async Task<IActionResult> Delete([FromRoute][Required] string name)
    {
        var response = await _nicheService.DeleteAsync(name);
        return ToActionResult(response);
    }

    [HttpGet("distributor/niches")]
    [SwaggerOperation(
        Summary = "Get all niches"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PagedResult<NicheDto>>))]
    public async Task<IActionResult> All()
    {
        var response = await _nicheService.AllAsync();
        return ToActionResult(response);
    }
}
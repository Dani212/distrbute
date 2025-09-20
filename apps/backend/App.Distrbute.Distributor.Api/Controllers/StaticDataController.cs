using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Sdk.Core.Interfaces;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = CommonConstants.AuthScheme.BEARER)]
public class StaticDataController : StaticDataControllerBase
{
    public StaticDataController(IDbRepository dbRepository) : base(dbRepository)
    {
    }
}
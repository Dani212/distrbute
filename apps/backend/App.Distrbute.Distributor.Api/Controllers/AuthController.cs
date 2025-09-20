using App.Distrbute.Api.Common.Controllers;
using App.Distrbute.Api.Common.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace App.Distrbute.Distributor.Api.Controllers;

[ApiController]
public class AuthController : AuthControllerBase
{
    public AuthController(IAuthenticationService authService) : base(authService)
    {
    }

    protected override string TokenPath()
    {
        return "/api/distributor";
    }
}
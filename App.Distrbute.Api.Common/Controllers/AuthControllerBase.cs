using App.Distrbute.Api.Common.Dtos.auth;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Controllers;

public abstract class AuthControllerBase : CustomControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthControllerBase(IAuthenticationService authService)
    {
        _authService = authService;
    }

    [HttpPost("send-otp")]
    [SwaggerOperation(
        Summary = "Send an OTP to provided address"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<GeneratedOtp>))]
    public async Task<IActionResult> SendOtp([FromBody] LoginRequest request)
    {
        var response = await _authService.SendOtpAsync(request);

        return ToActionResult(response);
    }

    [HttpPost("verify-otp")]
    [SwaggerOperation(
        Summary = "Verify an OTP code"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<VerifiedOtpResponse>))]
    public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationRequest otp)
    {
        var response = await _authService.VerifyOtpAsync(otp);

        var token = response.Data!.Token;
        var expirationSeconds = response.Data.ExpirationMillis / 1000;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            MaxAge = TimeSpan.FromSeconds(expirationSeconds),
            Path = TokenPath()
        };

        var bearerToken = $"Bearer {token}";
        Response.Cookies.Append(CommonConstants.AUTHORIZATION_COOKIE, bearerToken, cookieOptions);

        return ToActionResult(response);
    }

    public abstract string TokenPath();
}
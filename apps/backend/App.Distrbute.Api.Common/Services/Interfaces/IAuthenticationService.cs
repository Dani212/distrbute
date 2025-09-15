using App.Distrbute.Api.Common.Dtos.auth;
using Utility.Sdk.Dtos;
using LoginRequest = App.Distrbute.Api.Common.Dtos.auth.LoginRequest;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IAuthenticationService
{
    Task<IApiResponse<GeneratedOtp>> SendOtpAsync(LoginRequest request);
    Task<IApiResponse<VerifiedOtpResponse>> VerifyOtpAsync(OtpVerificationRequest otp);
}
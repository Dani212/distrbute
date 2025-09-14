using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos.auth;

public class OtpVerificationRequest
{
    [Required] public string RequestId { get; set; } = null!;
    [Required] public string VerificationId { get; set; } = null!;
    [Required] public string OtpPrefix { get; set; } = null!;
    [Required] public string OtpCode { get; set; } = null!;
    [Required] public string Email { get; set; } = null!;
}
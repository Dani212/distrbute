using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos.auth;

public class LoginRequest
{
    public string? Name { get; set; }
    [Required] public string Email { get; set; } = null!;
}
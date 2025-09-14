namespace App.Distrbute.Api.Common.Dtos;

public class InitializeTransactionDto
{
    public string AuthorizationUrl { get; set; } = null!;
    public string AccessCode { get; set; } = null!;
    public string Reference { get; set; } = null!;
}
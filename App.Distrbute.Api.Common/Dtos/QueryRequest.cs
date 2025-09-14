using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Dtos;

public class WalletQueryRequest
{
    [Required] public string TenantId { get; set; } = null!;
}
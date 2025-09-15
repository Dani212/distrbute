using System.ComponentModel.DataAnnotations;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class Email : BaseModel
{
    [Required] public string Name { get; set; }
    [Required] [EmailAddress] public string Address { get; set; } = null!;
    [Required] public int LedgerClientId { get; set; }
}
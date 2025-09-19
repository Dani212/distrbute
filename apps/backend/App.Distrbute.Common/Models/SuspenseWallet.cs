using System.ComponentModel.DataAnnotations;
using Persistence.Sdk.Models;
using Redact.Sdk.Attributes;

namespace App.Distrbute.Common.Models;

[Redactable]
public class SuspenseWallet : BaseWallet
{
    [Required, ForeignKey]
    public Distributor Distributor { get; set; } = null!;
}
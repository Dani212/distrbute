using System.ComponentModel.DataAnnotations;
using Redact.Sdk.Attributes;

namespace App.Distrbute.Common.Models;

[Redactable]
public class SuspenseWallet : BaseWallet
{
    [Required] public Distributor Distributor { get; set; } = null!;
}
using System.ComponentModel.DataAnnotations;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class BrandResource : BaseModel
{
    [Required] public Brand Brand { get; set; } = null!;
}
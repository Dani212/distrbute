using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;

namespace App.Distrbute.Common.Models;

public class Brand : BaseBrandDistributor
{
    [Required] public List<BrandNiche> Niches { get; set; } = [];
    [Required] public string Bio { get; set; } = null!;
}
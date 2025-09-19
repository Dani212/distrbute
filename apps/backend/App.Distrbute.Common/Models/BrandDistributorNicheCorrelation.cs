using System.ComponentModel.DataAnnotations;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class BrandDistributorNicheCorrelation : BaseModel
{
    [Required, ForeignKey]
    public BrandNiche BrandNiche { get; set; }
    
    [Required, ForeignKey]
    public DistributorNiche DistributorNiche { get; set; }
    
    public int Weight { get; set; }
}
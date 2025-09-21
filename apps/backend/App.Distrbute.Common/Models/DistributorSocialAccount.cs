using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Socials.Sdk.Enums;

namespace App.Distrbute.Common.Models;

public class DistributorSocialAccount : SocialAccountBase
{
    [Required, Persistence.Sdk.Models.ForeignKey]
    public Distributor Distributor { get; set; } = null!;
    
    public long StoryPaidViews { get; set; }
    public long ReelPaidViews { get; set; }
    public long ShortPaidViews { get; set; }
    public long PostPaidViews { get; set; }


    [Required, Persistence.Sdk.Models.ForeignKey]
    public List<DistributorNiche> Niches { get; set; } = new();

    [Required, Persistence.Sdk.Models.ForeignKey]
    public List<BrandNiche> ExcludeFromNiche { get; set; } = new();

    [Required, Column(TypeName = "jsonb")]
    public List<ContentType> ExcludeFromContent { get; set; } = new();
    public DateTime? AudienceLastUpdated { get; set; }
}
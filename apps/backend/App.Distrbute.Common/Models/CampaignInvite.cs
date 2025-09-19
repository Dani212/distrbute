using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class CampaignInvite : BrandResource
{
    [Required, ForeignKey] 
    public DistributorSocialAccount DistributorSocialAccount { get; set; } = null!;
    
    [Required, ForeignKey] 
    public Campaign Campaign { get; set; } = null!;

    public double Ask { get; set; }
    public double Bid { get; set; }
    public long Reach { get; set; }
    
    [Required] 
    public CampaignInviteStatus? Status { get; set; } = CampaignInviteStatus.Pending;
    public DateTime? Expiry { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    
    public bool PostSubmitted { get; set; }
}
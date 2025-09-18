using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Distrbute.Common.Enums;
using Socials.Sdk.Dtos;
using Socials.Sdk.Enums;

namespace App.Distrbute.Common.Models;

public class Post : BrandResource
{
    [Required] 
    public string ExternalPostId { get; set; } = null!;
    
    // a distributor post
    [Persistence.Sdk.Models.ForeignKey] 
    public DistributorSocialAccount? DistributorSocialAccount { get; set; }
    
    // a brand post
    [Persistence.Sdk.Models.ForeignKey] 
    public BrandSocialAccount? BrandSocialAccount { get; set; }
    
    [Persistence.Sdk.Models.ForeignKey] 
    public PostMetric? PostValuation { get; set;}
    
    // part of a campaign
    [Persistence.Sdk.Models.ForeignKey] 
    public CampaignInvite? CampaignInvite { get; set; }
    
    [Persistence.Sdk.Models.ForeignKey] 
    public DistrbuteTransaction? DistrbuteTransaction { get; set; }
    
    public PostStatus? PostStatus { get; set; }
    
    public PostApprovalStatus? PostApprovalStatus { get; set; }
    
    public PostPayoutStatus? PostPayoutStatus { get; set; }
    
    [Column(TypeName = "jsonb")]
    public Embedding? Embedding { get; set; }

    [Required] 
    public string Link { get; set; }
    
    [Required] 
    public ContentType? ContentType { get; set; }
    
    [Required] 
    public DateTime? PostedAt { get; set; }
}
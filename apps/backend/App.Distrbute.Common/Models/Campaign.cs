using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Distrbute.Common.Enums;
using Socials.Sdk.Enums;

namespace App.Distrbute.Common.Models;

public class Campaign : BrandResource
{
    [Required] 
    public CampaignType? Type { get; set; }
    public CampaignSubType? SubType { get; set; }

    public double Budget { get; set; }
    public double Reach { get; set; }

    [Required] 
    public CampaignStatus? Status { get; set; } = CampaignStatus.Pending;
    
    [Required, Column(TypeName = "jsonb")]
    public List<DistributorNiche> TargetedNiches { get; set; } = [];

    [Required, Column(TypeName = "jsonb")]
    public List<PlatformSplit> TargetedPlatforms { get; set; } = new();
    
    [Column(TypeName = "jsonb")] 
    public ContentDocumentFile? Attachment { get; set; }

    [Column(TypeName = "jsonb")] 
    public UGCDocumentFile? RulesOfEngagement { get; set; }
    
    [Required]
    public ContentType? ContentType { get; set; }

    // Funding details
    [Required, Persistence.Sdk.Models.ForeignKey]
    public DistrbuteTransaction FundingTransaction { get; set; } = null!;
}

public class PlatformSplit
{
    public Platform? Platform { get; set; }

    [Range(0, 100, ErrorMessage = "Split must be between 0 and 100.")]
    public double Split { get; set; }
}
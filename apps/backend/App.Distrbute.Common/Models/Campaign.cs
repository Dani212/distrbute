using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Distrbute.Common.Enums;
using Socials.Sdk.Enums;

namespace App.Distrbute.Common.Models;

public class Campaign : BrandResource
{
    [Required] public CampaignType? Type { get; set; }
    public CampaignSubType? SubType { get; set; }
    
    [Required]
    [Column(TypeName = "jsonb")]
    public List<DistributorNiche> TargetedNiches { get; set; } = [];

    [Required]
    [Column(TypeName = "jsonb")]
    public List<PlatformSplit> TargetedPlatforms { get; set; } = new();

    [Column(TypeName = "jsonb")] public ContentDocumentFile? Attachment { get; set; }

    [Column(TypeName = "jsonb")] public UGCDocumentFile? RulesOfEngagement { get; set; }

    public double Budget { get; set; }
    public double Reach { get; set; }

    [Required] public CampaignStatus? Status { get; set; } = CampaignStatus.Pending;

    // Funding details
    [Required] public DistrbuteTransaction? FundingTransaction { get; set; }
}

public class PlatformSplit
{
    public Platform? Platform { get; set; }

    [Range(0, 100, ErrorMessage = "Split must be between 0 and 100.")]
    public double Split { get; set; }
}

public static class CampaignExtensions
{
    public static ContentType GetContentType(this Campaign campaign)
    {
        return campaign.Type switch
        {
            CampaignType.Broadcast => campaign.Attachment!.ContentType!.Value,
            CampaignType.UGC => campaign.RulesOfEngagement!.ContentType!.Value,

            _ => throw new ArgumentOutOfRangeException(nameof(campaign.Type), campaign.Type, null)
        };
    }
}
using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Socials.Sdk.Enums;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Dtos.Campaign;

public class CampaignDto
{
    public string Id { get; set; } = null!;
    public string? BrandName { get; set; }
    public string? BrandBio { get; set; }
    public string? BrandEmail { get; set; }
    public CampaignStatus? Status { get; set; }
    public string BrandId { get; set; } = null!;
    public CampaignType? Type { get; set; }
    public CampaignSubType? SubType { get; set; }
    public List<AgeGroup> TargetedAgeGroups { get; set; } = new();
    public List<AudiencePersona> TargetedAudience { get; set; } = new();
    public List<PlatformSplit> TargetedPlatforms { get; set; } = new();
    public List<AudienceGender> TargetedGenders { get; set; } = new();
    public List<AudienceLocation> TargetedLocations { get; set; } = new();
    public ContentDocumentFile? Attachment { get; set; } = new();
    public UGCDocumentFile? RulesOfEngagement { get; set; }

    [Range(10, double.MaxValue, ErrorMessage = "Budget must be at least 10.")]
    public double? Budget { get; set; }

    [Range(100, double.MaxValue, ErrorMessage = "Reach must be at least 100.")]
    public double? Reach { get; set; }
}

public class CreateCampaignDto
{
    [Required] public string BrandId { get; set; } = null!;
    [Required] public string QuoteId { get; set; } = null!;
}

public class CampaignStatistics : CampaignDto
{
    public double AmountSpent { get; set; }
    public Dictionary<Platform, PlatformBreakdown> PlatformBreakdowns { get; set; } = new();
}

public class PlatformBreakdown
{
    public double Views { get; set; }
    public long Comments { get; set; }
    public double Likes { get; set; }
    public double EngagementRate { get; set; }
}

public class CampaignEstimateReq
{
    [Required] public string BrandId { get; set; } = null!;

    [Required]
    [AttributeExtensions.DisallowedEnumValues((int)CampaignType.Default)]
    public CampaignType? Type { get; set; }

    [Required]
    [AttributeExtensions.DisallowedEnumValues((int)CampaignSubType.Default)]
    public CampaignSubType? SubType { get; set; }

    [Required]
    [AttributeExtensions.ListRange(1, 100, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AgeGroup.Default)]
    public List<AgeGroup> TargetedAgeGroups { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(1, 100, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AudiencePersona.Default)]
    public List<AudiencePersona> TargetedAudience { get; set; } = new();

    [Required]
    [Distrbute.Common.Extensions.AttributeExtensions.ValidatePlatformSplit]
    public List<PlatformSplit> TargetedPlatforms { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(1, 2, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AudienceGender.Default)]
    public List<AudienceGender> TargetedGenders { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(1, 100, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AudienceLocation.Default)]
    public List<AudienceLocation> TargetedLocations { get; set; } = new();

    [AttributeExtensions.ConditionalRequired(nameof(Type), CampaignType.Broadcast)]
    public ContentDocumentFile? Attachment { get; set; }

    [AttributeExtensions.ConditionalRequired(nameof(Type), CampaignType.UGC)]
    public UGCDocumentFile? RulesOfEngagement { get; set; }

    [AttributeExtensions.ConditionalRequired(nameof(SubType), CampaignSubType.Budget)]
    [Range(10, double.MaxValue, ErrorMessage = "Budget must be at least 10.")]
    public double? Budget { get; set; }

    [AttributeExtensions.ConditionalRequired(nameof(SubType), CampaignSubType.Reach)]
    [Range(100, double.MaxValue, ErrorMessage = "Reach must be at least 100.")]
    public double? Reach { get; set; }
}

public class CampaignEstimateResponse : CampaignEstimateReq
{
    public string Currency { get; set; } = "GHS";
    public string BrandEmail { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string BrandBio { get; set; } = null!;
    public Estimate? BestMatch { get; set; }
    public Estimate? Balanced { get; set; }
    public Estimate? BestReach { get; set; }
}

public class CampaignEstimateResponseInternal : CampaignEstimateResponse
{
    public EstimateInternal? BestMatchInternal { get; set; }
    public EstimateInternal? BalancedInternal { get; set; }
    public EstimateInternal? BestReachInternal { get; set; }
}

public class Estimate
{
    public string QuoteId { get; set; } = null!;
    public double Reach { get; set; }
    public double Budget { get; set; }
    public int MatchedDistributorsCount { get; set; }
    public List<MatchedDistributor> MatchedDistributorsPreview { get; set; } = null!;

    public static Estimate? operator +(Estimate? _this, Estimate? _that)
    {
        if (_this == null) return _that;
        if (_that == null) return _this;

        var added = new Estimate();
        added.QuoteId = _this.QuoteId;
        added.Reach = Math.Round(_this.Reach + _that.Reach);
        added.Budget = Math.Round(_this.Budget + _that.Budget, 2);
        added.MatchedDistributorsCount = _this.MatchedDistributorsCount + _that.MatchedDistributorsCount;
        added.MatchedDistributorsPreview = _this.MatchedDistributorsPreview.Concat(_that.MatchedDistributorsPreview)
            .Take(5)
            .ToList();

        return added;
    }
}

public static class CampaignEstimateReqExtensions
{
    public static ContentType GetContentType(this CampaignEstimateReq campaign)
    {
        return campaign.Type switch
        {
            CampaignType.Broadcast => campaign.Attachment!.ContentType!.Value,
            CampaignType.UGC => campaign.RulesOfEngagement!.ContentType!.Value,

            _ => throw new ArgumentOutOfRangeException(nameof(campaign.Type), campaign.Type, null)
        };
    }
}

public class EstimateInternal : Estimate
{
    public double Ask { get; set; }
    public double Bid { get; set; }
    public List<MatchedDistributorInternal> MatchedDistributors { get; set; } = null!;

    public static EstimateInternal? operator +(EstimateInternal? _this, EstimateInternal? _that)
    {
        if (_this == null) return _that;
        if (_that == null) return _this;

        var added = new EstimateInternal();
        added.QuoteId = _this.QuoteId;

        added.Ask = Math.Round(_this.Ask + _that.Ask, 2);
        added.Bid = Math.Round(_this.Bid + _that.Bid, 2);

        added.Reach = Math.Round(_this.Reach + _that.Reach);
        added.Budget = Math.Round(_this.Budget + _that.Budget, 2);

        added.MatchedDistributorsCount = _this.MatchedDistributorsCount + _that.MatchedDistributorsCount;
        added.MatchedDistributorsPreview = _this.MatchedDistributorsPreview.Concat(_that.MatchedDistributorsPreview)
            .Take(5)
            .ToList();
        added.MatchedDistributors = _this.MatchedDistributors.Concat(_that.MatchedDistributors).ToList();

        return added;
    }
}

public class MatchedDistributor
{
    public string DistributorId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Platform? Platform { get; set; }
    public string Handle { get; set; } = null!;
    public string ProfileLink { get; set; } = null!;
    public DocumentFileDto ProfilePicture { get; set; } = null!;
}

public class MatchedDistributorInternal : MatchedDistributor
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public double Ask { get; set; }
    public double Bid { get; set; }
    public long Reach { get; set; }
}

public class CampaignEstimateCache : EstimateInternal
{
    [Required] public string BrandEmail { get; set; } = null!;
    [Required] public string BrandId { get; set; } = null!;
    [Required] public string BrandName { get; set; } = null!;
    [Required] public string BrandBio { get; set; } = null!;

    [Required]
    [AttributeExtensions.DisallowedEnumValues((int)CampaignType.Default)]
    public CampaignType? Type { get; set; }

    [AttributeExtensions.ConditionalRequired(nameof(Type), CampaignType.Broadcast)]
    [AttributeExtensions.DisallowedEnumValues((int)CampaignSubType.Default)]
    public CampaignSubType? SubType { get; set; }

    [Required]
    [AttributeExtensions.ListRange(1, 100, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AgeGroup.Default)]
    public List<AgeGroup> TargetedAgeGroups { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(1, 100, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AudiencePersona.Default)]
    public List<AudiencePersona> TargetedAudience { get; set; } = new();

    [Required]
    [Distrbute.Common.Extensions.AttributeExtensions.ValidatePlatformSplit]
    public List<PlatformSplit> TargetedPlatforms { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(1, 2, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AudienceGender.Default)]
    public List<AudienceGender> TargetedGenders { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(1, 100, true)]
    [AttributeExtensions.DisallowedEnumValues((int)AudienceLocation.Default)]
    public List<AudienceLocation> TargetedLocations { get; set; } = new();

    [AttributeExtensions.ConditionalRequired(nameof(Type), CampaignType.Broadcast)]
    public ContentDocumentFile? Attachment { get; set; }

    [AttributeExtensions.ConditionalRequired(nameof(Type), CampaignType.UGC)]
    public UGCDocumentFile? RulesOfEngagement { get; set; }

    public string Currency { get; set; } = null!;
}
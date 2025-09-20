using App.Distrbute.Common.Enums;
using Socials.Sdk.Enums;

namespace App.Distrbute.Api.Common.Options;

public class PriceConfig
{
    public double Base { get; set; }

    public Dictionary<Platform, double> PlatformWeights { get; set; } = new Dictionary<Platform, double>();

    public Dictionary<ContentType, double> ContentTypeWeights { get; set; } = new Dictionary<ContentType, double>();

    public Dictionary<CampaignType, double> EngagementWeights { get; set; } = new Dictionary<CampaignType, double>();

    public double Margin { get; set; }

    public Dictionary<Platform, string> Disclaimers { get; set; } = new Dictionary<Platform, string>();
}

public static class PriceConfigExtensions
{
    public static (double Ask, double Bid) CalculatePrice(
        this PriceConfig config,
        Platform? platform,
        ContentType? contentType,
        CampaignType? campaignType,
        long reach)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config), "Price configuration is required.");
        
        if (platform == Platform.Default)
            throw new InvalidOperationException("Platform cannot be Default.");

        if (contentType == ContentType.Default)
            throw new InvalidOperationException("Content type cannot be Default.");

        if (campaignType == CampaignType.Default)
            throw new InvalidOperationException("Engagement type cannot be Default.");

        if (reach <= 0)
            throw new ArgumentOutOfRangeException(nameof(reach), "Reach must be greater than zero.");

        if (!config.PlatformWeights.ContainsKey(platform!.Value))
            throw new InvalidOperationException($"No platform weight defined for {platform}.");

        if (!config.ContentTypeWeights.ContainsKey(contentType!.Value))
            throw new InvalidOperationException($"No content type weight defined for {contentType}.");

        if (!config.EngagementWeights.ContainsKey(campaignType!.Value))
            throw new InvalidOperationException($"No engagement weight defined for {campaignType}.");

        if (!config.Disclaimers.ContainsKey(platform.Value))
            throw new InvalidOperationException($"No disclaimer defined for {platform}.");

        var basePrice = config.Base;
        if (basePrice <= 0)
            throw new InvalidOperationException("Base price must be greater than zero.");

        var margin = config.Margin;
        if (margin <= 0 || margin >= 1)
            throw new InvalidOperationException("Margin must be a value between 0 and 1.");

        var platformWeight = config.PlatformWeights[platform.Value];
        var contentWeight = config.ContentTypeWeights[contentType.Value];
        var engagementWeight = config.EngagementWeights[campaignType.Value];

        var askPerReach = basePrice * platformWeight * contentWeight * engagementWeight;
        if (askPerReach <= 0)
            throw new InvalidOperationException("Calculated ask price per reach must be greater than zero.");

        var bidPerReach = askPerReach * (1 - margin);

        var totalAsk = askPerReach * reach;
        var totalBid = bidPerReach * reach;

        return (totalAsk, totalBid);
    }
    
    public static string GetDisclaimer(
        this PriceConfig config,
        Platform? platform)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config), "Price configuration is required.");
        
        if (platform == Platform.Default)
            throw new InvalidOperationException("Platform cannot be Default.");

        if (!config.Disclaimers.ContainsKey(platform!.Value))
            throw new InvalidOperationException($"No disclaimer defined for {platform}.");

        return config.Disclaimers[platform.Value];
    }
}
using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Models;
using Mapster;
using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Services.Providers;

public class PostValuationService : IPostValuationService
{
    private readonly ValuationConfig _config;

    public PostValuationService()
    {
        _config = new ValuationConfig();
    }

    public ValuedPost Value(TrackedPost trackedPost)
    {
        var socialPost = trackedPost.SocialPost;
        var now = DateTime.UtcNow;
        var postedAt = socialPost.CreatedAt ?? now;

        // Engagement categories
        var highIntentEngagement = socialPost.SavedCount;
        var mediumIntentEngagement = socialPost.CommentCount + socialPost.QuoteCount;
        var lowIntentEngagement = socialPost.LikeCount + socialPost.ShareCount;
        var awarenessEngagement = socialPost.ViewCount;
        var totalIntentEngagement = highIntentEngagement + mediumIntentEngagement + lowIntentEngagement;

        // Hybrid denominator (followers + sqrt(views)) to avoid undervaluing viral reach
        var followers = Math.Max(1, trackedPost.SocialProfile.FollowersCount);
        var hybridDenominator = followers + Math.Sqrt(awarenessEngagement);

        // Weighted engagement score
        var weightedEngagementScore = (
            highIntentEngagement * _config.HighIntentWeight +
            mediumIntentEngagement * _config.MediumIntentWeight +
            lowIntentEngagement * _config.LowIntentWeight +
            awarenessEngagement * _config.AwarenessWeight
        ) / hybridDenominator;

        // Engagement depth ratio
        var totalWeightedEngagement = highIntentEngagement * _config.HighIntentWeight +
                                      mediumIntentEngagement * _config.MediumIntentWeight +
                                      lowIntentEngagement * _config.LowIntentWeight +
                                      awarenessEngagement * _config.AwarenessWeight;

        var maxPossibleWeight = (totalIntentEngagement + awarenessEngagement) * _config.HighIntentWeight;
        var engagementDepthRatio = maxPossibleWeight > 0 ? totalWeightedEngagement / maxPossibleWeight : 0;

        // Engagement rate & authenticity
        var engagementRate = (double)totalIntentEngagement / followers;
        var authenticityScore = CalculateAuthenticityScore(engagementRate);

        // Recency factor
        var daysSincePosted = (now - postedAt).TotalDays;

        // Base conversion potential
        var baseConversionPotential = weightedEngagementScore * _config.ScalingFactor;

        // Quality multiplier
        var qualityMultiplier =
            (1.0 + engagementDepthRatio * 0.5) *
            authenticityScore;

        // Conversion score with bounds
        var conversionScore = Math.Min(_config.MaxConversionRate,
            Math.Max(_config.MinConversionRate,
                baseConversionPotential * qualityMultiplier));

        // Estimated conversions with diminishing returns
        var estimatedConversions = awarenessEngagement * (1 - Math.Exp(-_config.ConversionCurveK * conversionScore));

        // Build valuation
        var valuation = new PostValuation
        {
            FollowersCount = followers,
            ViewCount = socialPost.ViewCount,
            LikeCount = socialPost.LikeCount,
            CommentCount = socialPost.CommentCount,
            SavedCount = socialPost.SavedCount,
            ShareCount = socialPost.ShareCount,
            QuoteCount = socialPost.QuoteCount,

            ConversionScore = Math.Round(conversionScore, 4),
            WeightedEngagementScore = Math.Round(weightedEngagementScore, 4),
            EngagementDepthRatio = Math.Round(engagementDepthRatio, 3),
            AuthenticityScore = Math.Round(authenticityScore, 3),

            EstimatedConversions = (long) Math.Round(estimatedConversions),
            EngagementRate = Math.Round(engagementRate * 100, 2)
        };

        var valuedPost = trackedPost.Adapt<ValuedPost>();
        valuedPost.Valuation = valuation;

        return valuedPost;
    }

    public ValuedPosts ValueMany(TrackedPosts trackedPosts)
    {
        var valuations = new Dictionary<SocialPost, PostValuation>();

        foreach (var socialPost in trackedPosts.SocialPosts)
        {
            var trackedPost = trackedPosts.Adapt<TrackedPost>();
            trackedPost.SocialPost = socialPost;

            var valuation = Value(trackedPost);
            
            valuations[socialPost] = valuation.Valuation;
        }

        var valuedPost = trackedPosts.Adapt<ValuedPosts>();
        valuedPost.SocialPostsWithValuations = valuations;

        return valuedPost;
    }

    private double CalculateAuthenticityScore(double engagementRate)
    {
        if (engagementRate < _config.MinEngagementRate) return 0.6; // softer penalty
        if (engagementRate > _config.MaxEngagementRate) return 0.5; // softer penalty

        if (engagementRate >= 0.01 && engagementRate <= 0.08)
            return 1.0;

        return Math.Max(0.6, 1.0 - Math.Abs(engagementRate - 0.045) / 0.045);
    }
}

public class ValuationConfig
{
    public double HighIntentWeight { get; set; } = 8.0;
    public double MediumIntentWeight { get; set; } = 4.0;
    public double LowIntentWeight { get; set; } = 1.0;
    public double AwarenessWeight { get; set; } = 0.002;

    public double MinEngagementRate { get; set; } = 0.001;
    public double MaxEngagementRate { get; set; } = 0.15;

    public double ScalingFactor { get; set; } = 25.0;
    public double MaxConversionRate { get; set; } = 0.25;
    public double MinConversionRate { get; set; } = 0.0001;

    // Controls diminishing returns curve
    public double ConversionCurveK { get; set; } = 5.0;
}
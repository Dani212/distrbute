using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class PostValuation : BaseModel
{
    public long FollowersCount { get; set; }
    public long ViewCount { get; set; }
    public long LikeCount { get; set; }
    public long CommentCount { get; set; }
    public long SavedCount { get; set; }
    public long ShareCount { get; set; }
    public long QuoteCount { get; set; }
    public double ConversionScore { get; set; }
    public double WeightedEngagementScore { get; set; }
    public double EngagementDepthRatio { get; set; }
    public double AuthenticityScore { get; set; }
    public long EstimatedConversions { get; set; }
    public double EngagementRate { get; set; }
}
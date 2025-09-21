using App.Distrbute.Common.Models;
using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Dtos.Post;

public class ValuedPost : TrackedPost
{
    public PostValuation Valuation { get; set; }
}

public class ValuedPosts : TrackedPosts
{
    public IDictionary<SocialPost, PostValuation> SocialPostsWithValuations { get; set; }
}
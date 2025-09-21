using App.Distrbute.Common.Models;
using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Dtos.Post;

public class TrackPostBase
{
    public BrandSocialAccount? BrandSocialAccount { get; set; }
    
    public DistributorSocialAccount? DistributorSocialAccount { get; set; }
    public SocialProfile SocialProfile { get; set; }
}

public class TrackOnePostReq : TrackPostBase
{
    public string Link { get; set; }
}

public class TrackedPost : TrackPostBase
{
    public SocialPost SocialPost { get; set; }
}

public class TrackedPosts : TrackPostBase
{
    public List<SocialPost> SocialPosts { get; set; }
}
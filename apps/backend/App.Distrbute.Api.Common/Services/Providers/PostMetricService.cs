using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Services.Interfaces;
using Mapster;
using Socials.Sdk.Dtos;
using Socials.Sdk.Enums;
using Socials.Sdk.Services.Interfaces;

namespace App.Distrbute.Api.Common.Services.Providers;

public class PostMetricService : IPostMetricService
{
    private readonly IInstagramSdk _instagramSdk;
    private readonly ITiktokSdk _tiktokSdk;
    private readonly ITwitterSdk _twitterSdk;

    public PostMetricService(
        ITwitterSdk twitterSdk,
        IInstagramSdk instagramSdk,
        ITiktokSdk tiktokSdk)
    {
        _twitterSdk = twitterSdk;
        _instagramSdk = instagramSdk;
        _tiktokSdk = tiktokSdk;
    }

    public async Task<TrackedPost> GatherAsync(TrackOnePostReq req)
    {
        var link = req.Link;
        var socialProfile = req.SocialProfile;
        var socialPost = new SocialPost();
        var platform = socialProfile.Platform!.Value;
        
        switch (platform)
        {
            case Platform.Instagram:
            {
                // get latest profile info
                socialProfile = await _instagramSdk.GetUser(socialProfile);
                socialPost = await _instagramSdk.GetPost(socialProfile, link);

                break;
            }
            case Platform.TikTok:
            {
                // get latest profile info
                socialProfile = await _tiktokSdk.GetUser(socialProfile);
                socialPost = await _tiktokSdk.GetPost(socialProfile, link);

                break;
            }
            case Platform.Twitter:
            {
                socialProfile = await _twitterSdk.GetUser(socialProfile);
                socialPost = await _twitterSdk.GetPost(socialProfile, link);

                break;
            }
        }

        var trackedPost = req.Adapt<TrackedPost>();
        trackedPost.SocialPost = socialPost;

        return trackedPost;
    }

    public async Task<TrackedPosts> GatherManyAsync(TrackPostBase req)
    {
        var socialPosts = new List<SocialPost>();
        var socialProfile = req.SocialProfile;
        var platform = socialProfile.Platform!.Value;
        switch (platform)
        {
            case Platform.Instagram:
            {
                socialPosts = await _instagramSdk.GetPosts(socialProfile);

                break;
            }
            case Platform.TikTok:
            {
                socialPosts = await _tiktokSdk.GetPosts(socialProfile);

                break;
            }
            case Platform.Twitter:
            {
                socialPosts = await _twitterSdk.GetPosts(socialProfile);

                break;
            }
        }

        var trackedPost = req.Adapt<TrackedPosts>();
        trackedPost.SocialPosts = socialPosts;

        return trackedPost;
    }
}
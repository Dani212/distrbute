using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Models;
using Logged.Sdk.Core;
using Persistence.Sdk.Core.Interfaces;
using Socials.Sdk.Dtos;
using Socials.Sdk.Enums;

namespace App.Distrbute.Api.Common.Services.Providers;

public class SocialAccountValuationService : ISocialAccountValuationService
{
    private readonly IDbRepository _dbRepository;
    private readonly ICoolLogger<SocialAccountValuationService> _logger;

    public SocialAccountValuationService(
        IDbRepository dbRepository,
        ICoolLogger<SocialAccountValuationService> logger)
    {
        _dbRepository = dbRepository;
        _logger = logger;
    }

    public async Task<bool> ValueAsync(SocialProfile socialProfile)
    {
        var posts = await _dbRepository
            .GetManyAsync<Post>(q => q
                .Include(e => e.DistributorSocialAccount)
                .Where(q => q.DistributorSocialAccount != null)
                .Where(e => e.DistributorSocialAccount!.ExternalId == socialProfile.ExternalId)
                .Include(e => e.PostValuation)
                .Where( e => e.PostValuation != null));

        if (!posts.Any())
        {
            _logger.LogInformation("No distributor posts were found for {ExternalId}, returning", socialProfile.ExternalId);
            return true;
        }

        var storyConversions = posts
            .Where(p => p.ContentType == ContentType.Story)
            .Select(p => p.PostValuation!.EstimatedConversions)
            .DefaultIfEmpty(0)
            .Average();

        var shortConversions = posts
            .Where(p => p.ContentType == ContentType.Short)
            .Select(p => p.PostValuation!.EstimatedConversions)
            .DefaultIfEmpty(0)
            .Average();

        var reelConversions = posts
            .Where(p => p.ContentType == ContentType.Reel)
            .Select(p => p.PostValuation!.EstimatedConversions)
            .DefaultIfEmpty(0)
            .Average();

        var postConversions = posts
            .Where(p => p.ContentType == ContentType.Post)
            .Select(p => p.PostValuation!.EstimatedConversions)
            .DefaultIfEmpty(0)
            .Average();

        var socialAccount = posts.First().DistributorSocialAccount!;
        socialAccount.StoryPaidViews = (long) Math.Round(storyConversions);
        socialAccount.ShortPaidViews = (long) Math.Round(shortConversions);
        socialAccount.ReelPaidViews = (long) Math.Round(reelConversions);
        socialAccount.PostPaidViews = (long) Math.Round(postConversions);
        
        socialAccount.PostsLastSynced = DateTime.UtcNow;
        
        await _dbRepository.UpdateAsync(socialAccount);
        
        return true;
    }
}
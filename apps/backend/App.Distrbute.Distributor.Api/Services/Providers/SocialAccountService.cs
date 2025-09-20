using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using App.Distrbute.Distributor.Api.Dtos;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Mapster;
using Microsoft.Extensions.Options;
using Persistence.Sdk.Core.Interfaces;
using Persistence.Sdk.Dtos;
using Rest.Sdk.Core;
using Socials.Sdk.Dtos;
using Socials.Sdk.Enums;
using Socials.Sdk.Services.Interfaces;
using Utility.Sdk.Dtos;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Distributor.Api.Services.Providers;

public class SocialAccountService : ISocialAccountService
{
    private readonly IDbRepository _dbRepository;
    private readonly IPipelineProvider _pipelineProvider;
    private readonly PriceConfig  _priceConfig;
    private readonly IInstagramSdk _instagramSdk;
    private readonly ITiktokSdk _tiktokSdk;
    private readonly ITwitterSdk _twitterSdk;
    private const int TWENTY_FOUR_HOURS = 24;

    public SocialAccountService(
        IDbRepository dbRepository,
        IPipelineProvider pipelineProvider,
        IOptions<PriceConfig> priceConfig,
        ITwitterSdk twitterSdk,
        ITiktokSdk tiktokSdk,
        IInstagramSdk instagramSdk)
    {
        _dbRepository = dbRepository;
        _pipelineProvider = pipelineProvider;
        _priceConfig = priceConfig.Value;
        _instagramSdk = instagramSdk;
        _tiktokSdk = tiktokSdk;
        _twitterSdk = twitterSdk;
    }

    public async Task<string?> HandleConnectedSocialProfile(SocialProfile profile)
    {
        var existingAccount = await _dbRepository.GetAsync<DistributorSocialAccount>(q => q
            .Where(e => e.Platform == profile.Platform)
            .Include(e => e.Distributor)
            .Where(e => e.Distributor.Id == profile.UserId)
            .Where(e => profile.ExternalId == e.ExternalId || // same id
                        profile.Handle == e.Handle)) // or same handle
            .IgnoreAndDefault<NotFound, DistributorSocialAccount>();

        var now  = DateTime.UtcNow;
        if (existingAccount != null)
        {
            SetSocialAccountDetails(existingAccount, profile);
            
            var postsLastUpdated = existingAccount.PostsLastSynced;
            if (postsLastUpdated < now.AddHours(-TWENTY_FOUR_HOURS))  // 24 hours passed
            {
                // this involves loading the posts + valuation
                // happens in background
                await _pipelineProvider.ExecuteInitSocialAccountValuePipeline(existingAccount.Id);
            }
            
            existingAccount.ProfileLastSynced = now;

            var updated = await _dbRepository
                .UpdateAsync(existingAccount)
                .IgnoreAndDefault<FailedDependency, DistributorSocialAccount>();

            return updated?.Id;
        }
        
        var existingDistributor = await _dbRepository.GetAsync<Common.Models.Distributor>(q => q
            .Where(e => e.Id.Equals(profile.UserId)));

        var newAccount = new DistributorSocialAccount();
        newAccount.Id = Guid.NewGuid().ToString();
        newAccount.Distributor = existingDistributor;
        SetSocialAccountDetails(newAccount, profile);
            
        newAccount.PostsLastSynced = now;
        newAccount.ProfileLastSynced = now;
        
        var added = await _dbRepository
            .AddAsync(newAccount)
            .IgnoreAndDefault<FailedDependency, DistributorSocialAccount>();

        if (added != null)
        {
            // start first ever tracking of posts
            // this involves loading the posts + valuation
            // happens in background
            await _pipelineProvider.ExecuteInitSocialAccountValuePipeline(newAccount.Id);
        }

        return added?.Id;
    }

    public async Task HandleRefreshedTokenContext(OAuthTokenContext ctx)
    {
        var existingAccount = await _dbRepository.GetAsync<DistributorSocialAccount>(q => q
            .Where(e => e.Id.Equals(ctx.OAuthEntityId)));

        existingAccount.AccessToken = ctx.AccessToken;
        existingAccount.RefreshToken = ctx.RefreshToken;
        existingAccount.AccessTokenExpiry = ctx.AccessTokenExpiry;
        existingAccount.RefreshTokenExpiry = ctx.RefreshTokenExpiry;
        existingAccount.OAuthTokenKind = ctx.OAuthTokenKind;

        await _dbRepository.UpdateAsync(existingAccount);
    }

    public async Task<IApiResponse<SocialAccountDto>> GetAsync(Email principal, string id)
    {
        var existingAccount = await _dbRepository.GetAsync<DistributorSocialAccount>(q => q
            .IncludeWith(e => e.Distributor, d => d.Email)
            .Where(e => principal.Address == e.Distributor.Email.Address && e.Id.Equals(id)));

        var now = DateTime.UtcNow;
        var profileLastSynced = existingAccount.ProfileLastSynced;
        if (profileLastSynced < now.AddHours(-TWENTY_FOUR_HOURS)) // 24 hours passed
        {
            try
            {
                var platform = existingAccount.Platform!.Value;
                var socialProfile = existingAccount.AsSocialProfile();
                var updatedSocialProfile = platform switch
                {
                    Platform.Instagram  => await _instagramSdk.GetUser(socialProfile),
                    Platform.Twitter => await _twitterSdk.GetUser(socialProfile),
                    Platform.TikTok  => await _tiktokSdk.GetUser(socialProfile),
                    _ => null
                };

                if (updatedSocialProfile != null)
                {
                    existingAccount.FollowersCount = updatedSocialProfile.FollowersCount;
                    existingAccount.FollowingCount = updatedSocialProfile.FollowingCount;
                    existingAccount.DisplayName = updatedSocialProfile.DisplayName;
                    existingAccount.ProfileImageUrl = updatedSocialProfile.ProfileImageUrl;

                    existingAccount.ProfileLastSynced = now;
                
                    await _dbRepository.UpdateAsync(existingAccount);
                }
            }catch{}
        }
        
        var postsLastUpdated = existingAccount.PostsLastSynced;
        if (postsLastUpdated < now.AddHours(-TWENTY_FOUR_HOURS))  // 24 hours passed
        {
            // this involves loading the posts + valuation
            // happens in background
            await _pipelineProvider.ExecuteInitSocialAccountValuePipeline(existingAccount.Id);
        }
        
        
        var resp = existingAccount.Adapt<SocialAccountDto>();
        var payouts = GetPayouts(existingAccount);
        resp.Payouts = payouts;

        return resp.ToOkApiResponse();
    }

    public async Task<IApiResponse<List<ConnectedPlatformsSummary>>> GetConnectedAccountsSummary(Email principal)
    {
        var results = await _dbRepository.AggregateAsync<DistributorSocialAccount, Platform, ConnectedPlatformsSummary>(q => q
                .IncludeWith(e => e.Distributor, d => d.Email)
                .Where(e => principal.Address == e.Distributor.Email.Address),
            groupBy:e => e.Platform!.Value,
            selector: g => new ConnectedPlatformsSummary
            {
                Platform = g.Key,
                AccountCount = g.Count(),
                FollowersCount = g.Sum(sa => sa.FollowersCount),
                FollowingCount = g.Sum(sa => sa.FollowingCount)
            });

        return results.ToOkApiResponse();
    }

    public async Task<IApiResponse<SocialAccountDto>> UpdatePreferences(Email principal, string id,
        SocialAccountPreferencesReq req)
    {
        var exisitingAccount = await _dbRepository.GetAsync<DistributorSocialAccount>(q => q
            .IncludeWith(e => e.Distributor, d => d.Email)
            .Where(e => principal.Address == e.Distributor.Email.Address && e.Id.Equals(id)));

        // allow updating audience at 6 month intervals
        var now = DateTime.UtcNow;
        var sixMonthsPassed = exisitingAccount.AudienceLastUpdated == null ||
                              exisitingAccount.AudienceLastUpdated.Value.AddMonths(6) <= now;
        if (sixMonthsPassed)
        {
            var nichesUpdated = !exisitingAccount.Niches.Select(n => n.Name).HasTheSameElementsAs(req.Niches);
            if (nichesUpdated)
            {
                var requestedNiches =
                    await _dbRepository.GetManyAsync<DistributorNiche>(q => q.Where(e => req.Niches.Contains(e.Name)));
                exisitingAccount.Niches = requestedNiches;
                exisitingAccount.AudienceLastUpdated = now;
            }
        }

        exisitingAccount.IsDisabled = req.IsDisabled;
        exisitingAccount.ExcludeFromContent = req.ExcludeFromContent;
        
        var requestedExcludeNiches =
            await _dbRepository.GetManyAsync<BrandNiche>(q => q.Where(e => req.ExcludeFromNiches.Contains(e.Name)));
        
        exisitingAccount.ExcludeFromNiche = requestedExcludeNiches;

        var updated = await _dbRepository.UpdateAsync(exisitingAccount);

        var resp = updated.Adapt<SocialAccountDto>();
        var payouts = GetPayouts(updated);
        resp.Payouts = payouts;

        return resp.ToOkApiResponse();
    }

    public async Task<IApiResponse<PagedResult<SocialAccountDto>>> GetConnectedAccounts(Email principal,
        SocialAccountsPageRequest page)
    {
        var paged = await _dbRepository.GetAllAsync<DistributorSocialAccount>(q => q
                .IncludeWith(e => e.Distributor, d => d.Email)
                .Where(e => principal.Address == e.Distributor.Email.Address &&  (page.Platform == null || page.Platform == e.Platform))
            .Page(page));

        var response = new PagedResult<SocialAccountDto>
        {
            Page = page.Page,
            PageSize = page.PageSize,
            TotalCount = paged.TotalCount,
            Data = paged.Data.ConvertAll(e =>
            {
                var resp = e.Adapt<SocialAccountDto>();
                var payouts = GetPayouts(e);
                resp.Payouts = payouts;
                
                return resp;
            })
        };

        return response.ToOkApiResponse();
    }

    private static void SetSocialAccountDetails(DistributorSocialAccount account, SocialProfile profile)
    {
        account.Handle = profile.Handle;
        account.DisplayName = profile.DisplayName;
        account.ProfileImageUrl = profile.ProfileImageUrl;
        account.ExternalId = profile.ExternalId;
        account.Platform = profile.Platform;
        account.ProfileLink = profile.Platform.ToProfileLink(account.Handle);
        account.FollowersCount = profile.FollowersCount;
        account.FollowingCount = profile.FollowingCount;
        
        // these are already encrypted by socials.sdk and never stored in plain text
        account.AccessToken = profile.AccessToken;
        account.RefreshToken = profile.RefreshToken;
            
        account.AccessTokenExpiry = profile.AccessTokenExpiry;
        account.RefreshTokenExpiry = profile.RefreshTokenExpiry;
    }

    private IDictionary<CampaignType, IDictionary<ContentType, double>> GetPayouts(DistributorSocialAccount socialAccount)
    {
        if (socialAccount == null)
            throw new ArgumentNullException(nameof(socialAccount), "Distributor social account is required.");

        var result = new Dictionary<CampaignType, IDictionary<ContentType, double>>();

        foreach (CampaignType campaignType in Enum.GetValues(typeof(CampaignType)))
        {
            if (campaignType == CampaignType.Default)
                continue;

            var contentGrid = new Dictionary<ContentType, double>();

            foreach (ContentType contentType in Enum.GetValues(typeof(ContentType)))
            {
                if (contentType == ContentType.Default)
                    continue;

                // Determine reach from distributor's account
                long reach = contentType switch
                {
                    ContentType.Post  => socialAccount.PostPaidViews,
                    ContentType.Story => socialAccount.StoryPaidViews,
                    ContentType.Reel  => socialAccount.ReelPaidViews,
                    ContentType.Short => socialAccount.ShortPaidViews,
                    _ => throw new InvalidOperationException($"Unsupported content type: {contentType}")
                };

                var (_, bid) = _priceConfig.CalculatePrice(
                    socialAccount.Platform,
                    contentType,
                    campaignType,
                    reach);

                contentGrid[contentType] = bid;
            }

            if (contentGrid.Any())
                result[campaignType] = contentGrid;
        }

        return result;
    }
}
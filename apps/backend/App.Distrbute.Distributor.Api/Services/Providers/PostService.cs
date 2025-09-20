using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Extensions;
using App.Distrbute.Common.Models;
using App.Distrbute.Distributor.Api.Dtos;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Mapster;
using Messaging.Sdk.Mail.Core;
using Microsoft.Extensions.Options;
using Persistence.Sdk.Core.Interfaces;
using Persistence.Sdk.Core.Providers;
using Persistence.Sdk.Dtos;
using Scheduler.Sdk.Exceptions;
using SendGrid.Helpers.Mail;
using Socials.Sdk.Dtos;
using Socials.Sdk.Enums;
using Socials.Sdk.Extensions;
using Socials.Sdk.Services.Interfaces;
using Utility.Sdk.Dtos;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Distributor.Api.Services.Providers;

public class PostService : IPostService
{
    private readonly IDbRepository _dbRepository;
    private readonly IEmailClient _emailClient;
    private readonly IInstagramSdk _instagramSdk;
    private readonly MailTemplateConfig _mailTemplateConfig;
    private readonly IJobSchedulingService _schedulingService;
    private readonly ITiktokSdk _tiktokSdk;
    private readonly ITwitterSdk _twitterSdk;

    public PostService(IDbRepository dbRepository,
        IEmailClient emailClient,
        IOptions<MailTemplateConfig> mailTemplateConfig,
        IJobSchedulingService schedulingService,
        ITwitterSdk twitterSdk,
        ITiktokSdk tiktokSdk,
        IInstagramSdk instagramSdk)
    {
        _dbRepository = dbRepository;
        _emailClient = emailClient;
        _mailTemplateConfig = mailTemplateConfig.Value;
        _schedulingService = schedulingService;
        _twitterSdk = twitterSdk;
        _tiktokSdk = tiktokSdk;
        _instagramSdk = instagramSdk;
    }

    public async Task<IApiResponse<PostDto>> CreateAsync(Email principal, CreatePostReq req)
    {
        var invite = await GetInvite(principal, req.InviteId);

        // Prevent creating ads for ended invites
        if (invite.EndedAt != null)
            throw new BadRequest("This invite has already ended. You can no longer create an ad for it.");

        // Ensure invite was accepted
        if (invite.AcceptedAt == null)
            throw new BadRequest(
                $"You need to accept the invite before creating an ad. Current status: {invite.Status}");

        // Prevent duplicates
        await _dbRepository
            .ExistsAsync<Post>(q => q
                .Include(e => e.CampaignInvite)
                .Where(e => e.CampaignInvite != null)
                .Where(e => e.CampaignInvite!.Id == invite.Id))
            .ThrowIfTrue<BadRequest>(
                "An ad has already been created for this invite. You can update the existing one instead.");

        var link = req.Link.UrlClean();
        var platform = invite.DistributorSocialAccount.Platform!.Value;
        var contentType = invite.Campaign.ContentType!.Value;
        var isValidUrl = platform.IsValidUrl(link, contentType);
        if (!isValidUrl)
        {
            var recognizedUrls = platform.RecognizedUrls(contentType);
            throw new BadRequest(
                $"It looks like the URL: {link} you entered isn’t valid. For a {contentType} on {platform.GetDisplayName()}, the share link will usually start with one of the following: [{string.Join(", ", recognizedUrls)}]. Please double-check and try again."
            );
        }

        var socialAccount = invite.DistributorSocialAccount;
        var socialProfile = socialAccount.AsSocialProfile();
        var socialPost = await RetrievePost(socialProfile, link);
        
        var post = new Post();
        post.Id = Guid.NewGuid().ToString();
        post.DistributorSocialAccount = invite.DistributorSocialAccount;
        post.Brand = invite.Campaign.Brand;
        post.CampaignInvite = invite;
        post.PostStatus = PostStatus.Live;
        post.PostApprovalStatus = PostApprovalStatus.Pending;
        post.PostPayoutStatus = PostPayoutStatus.Pending;
        post.Embedding = socialPost.Embedding;
        post.Link = link;
        post.ContentType = invite.Campaign.ContentType;
        post.PostedAt = socialPost.CreatedAt;

        // schedule auto approval
        // throws
        await _schedulingService.SchedulePostAutoApprovalAsync(post)
            .CatchAndThrowAs<ScheduleException, FailedDependency>(
                "An error occured while processsing your post, let's try that again later");

        // schedule post tracking
        // throws
        await _schedulingService
            .SchedulePostTrackingAsync(post)
            .CatchAndThrowAs<ScheduleException, FailedDependency>(
                "An error occured while processsing your post, let's try that again later");

        Post saved;

        await using var optimisticLock = await _dbRepository.OptimisticTransactionAsync();
        try
        {
            saved = await _dbRepository.AddAsync(post);

            invite.PostSubmitted = true;
            await _dbRepository.UpdateAsync(invite);

            await optimisticLock.CommitAsync();
        }
        catch (Exception)
        {
            await optimisticLock.RollbackAsync();
            throw;
        }

        // send email
        await SendPostSubmittedEmail(post);

        var resp = GetResponseDto(saved);

        return resp.ToOkApiResponse();
    }

    public async Task<IApiResponse<PostDto>> GetAsync(Email principal, string id, PostsQueryRequest query)
    {
        var post = await _dbRepository.GetAsync<Post>(q => q
            .Where(e => e.Id == id)
            .IncludeWith(e => e.DistributorSocialAccount, e => e.Distributor, e => e.Email)
            .Where(e => e.DistributorSocialAccount!.Distributor.Email.Address == principal.Address)
            .Include(e => e.CampaignInvite)
            .Include(e => e.DistrbuteTransaction)
            .Include(e => e.Brand));

        var response = new PostDto
        {
            Id = post.Id,
            CampaignInviteId = post.CampaignInvite!.Id,
            BrandId = post.Brand.Id,
            BrandName = post.Brand.Name,
            BrandBio = post.Brand.Bio,
            Platform = post.DistributorSocialAccount!.Platform!.Value,
            PostStatus = post.PostStatus,
            PostApprovalStatus = post.PostApprovalStatus,
            PostPayoutStatus = post.PostPayoutStatus,
            Link = post.Link,
            Likes = post.PostValuation?.LikeCount ?? 0,
            Views = post.PostValuation?.ViewCount ?? 0,
            EngagementRate = post.PostValuation?.EngagementRate ?? 0,
            Comments = post.PostValuation?.CommentCount ?? 0,
            Embedding = post.Embedding,
            Payout = post.CampaignInvite.Bid,
            PaidOut = post.DistrbuteTransaction?.AmountAfterCharges ?? 0,
            CreatedAt = post.CreatedAt!.Value
        };

        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<PagedResult<PostDto>>> AllAsync(Email principal, PostPageRequest page)
    {
        QueryBuilder<Post> query = QueryBuilder<Post>.Base()
            .IncludeWith(e => e.DistributorSocialAccount, e => e.Distributor, e => e.Email)
            .Where(e => e.DistributorSocialAccount!.Distributor.Email.Address == principal.Address)
            .Include(e => e.CampaignInvite)
            .Include(e => e.Brand)
            .Include(e => e.DistrbuteTransaction)
            .Where(e => (page.InviteId == null || e.CampaignInvite!.Id == page.InviteId) &&
                        (page.Platform == null ||
                         page.Platform == e.DistributorSocialAccount!.Platform) && // filter by platform
                        (page.Status == null || page.Status == e.PostStatus) &&
                        (page.ApprovalStatus == null || page.ApprovalStatus.Equals(e.PostApprovalStatus)) &&
                        (page.PayoutStatus == null || page.PayoutStatus.Equals(e.PostPayoutStatus)))
            .Page(page);
        
        // apply ordering
        query = SortDirection.ASC == page.SortDir ? query.OrderBy(c => c.CreatedAt) :  query.OrderByDescending(c => c.CreatedAt);
        
        var paged = await _dbRepository.GetAllAsync<Post, PostDto>(
            bas => bas + query, selector: post => new PostDto
            {
                Id = post.Id,
                CampaignInviteId = post.CampaignInvite!.Id,
                BrandId = post.Brand.Id,
                BrandName = post.Brand.Name,
                BrandBio = post.Brand.Bio,
                Platform = post.DistributorSocialAccount!.Platform!.Value,
                PostStatus = post.PostStatus,
                PostApprovalStatus = post.PostApprovalStatus,
                PostPayoutStatus = post.PostPayoutStatus,
                Link = post.Link,
                Likes = post.PostValuation?.LikeCount ?? 0,
                Views = post.PostValuation?.ViewCount ?? 0,
                EngagementRate = post.PostValuation?.EngagementRate ?? 0,
                Comments = post.PostValuation?.CommentCount ?? 0,
                Embedding = post.Embedding,
                Payout = post.CampaignInvite.Bid,
                PaidOut = post.DistrbuteTransaction?.AmountAfterCharges ?? 0,
                CreatedAt = post.CreatedAt!.Value
            });

        return paged.ToOkApiResponse();
    }

    public async Task<IApiResponse<CampaignSummary>> SummaryAsync(Email principal)
    {
        var results = await _dbRepository.AggregateAsync<Post, int, CampaignSummary>(q => q
            .IncludeWith(e => e.DistributorSocialAccount, e => e.Distributor, e => e.Email)
            .Where(e => e.DistributorSocialAccount!.Distributor.Email.Address == principal.Address)
            .IncludeWith(e => e.CampaignInvite, e => e.Campaign)
            .Include(e => e.DistrbuteTransaction),
            groupBy: p => 1,
            selector: g => new CampaignSummary
            {
                Earnings = g
                    .Where(p => p.PostPayoutStatus == PostPayoutStatus.Paid)
                    .Sum(p => p.DistrbuteTransaction?.AmountAfterCharges ?? 0),
                Completed = g
                    .Where(p => p.PostPayoutStatus == PostPayoutStatus.Paid)
                    .Select(p => p.CampaignInvite!.Campaign.Id)
                    .Distinct()
                    .Count(),
                Active = g
                    .Where(p => p.PostPayoutStatus != PostPayoutStatus.Paid)
                    .Select(p =>p.CampaignInvite!.Campaign.Id)
                    .Distinct()
                    .Count(),
                PendingSubmission = g
                    .Where(i => i.CampaignInvite!.Status == CampaignInviteStatus.Accepted && !i.CampaignInvite.PostSubmitted)
                    .Select(p => p.CampaignInvite!.Campaign.Id)
                    .Distinct()
                    .Count(),
                PendingApproval = g
                    .Where(p => p.PostApprovalStatus == PostApprovalStatus.Pending)
                    .Select(p => p.CampaignInvite!.Campaign.Id)
                    .Distinct()
                    .Count(),
                Disputed = g
                    .Where(p => p.PostApprovalStatus == PostApprovalStatus.Disputed)
                    .Select(p => p.CampaignInvite!.Campaign.Id)
                    .Distinct()
                    .Count()
            });

        var result = results.FirstOrDefault() ?? new CampaignSummary();

        return result.ToOkApiResponse();
    }

    public async Task<IApiResponse<List<PostTimeseriesSlot>>> TimeseriesAsync(Email principal, PostTimeseriesQueryRequest queryRequest)
    {
        // derive timeframe boundaries
        var now = DateTime.UtcNow;
        var startDate = queryRequest.Timeframe switch
        {
            Timeframe.Today => now.Date,
            Timeframe.Yesterday => now.Date.AddDays(-1),
            Timeframe.LastWeek => now.Date.AddDays(-7),
            Timeframe.LastMonth => now.Date.AddMonths(-1),
            Timeframe.Last90Days => now.Date.AddDays(-90),
            Timeframe.ThisYear => new DateTime(now.Year, 1, 1),
            _ => DateTime.MinValue // Default = no filter
        };
        
        var results = await _dbRepository.AggregateAsync<Post, DateTime, PostTimeseriesSlot>(q => q
                .IncludeWith(e => e.DistributorSocialAccount, e => e.Distributor, e => e.Email)
                .Where(e => e.DistributorSocialAccount!.Distributor.Email.Address == principal.Address)
                .Include(e => e.PostValuation)
                .Where(e => e.PostValuation != null)
                .Where(e => (queryRequest.Timeframe == null || e.PostedAt >= startDate) &&
                             (queryRequest.Platform == null || queryRequest.Platform == e.DistributorSocialAccount!.Platform))
                .IncludeWith(e => e, e => e.DistrbuteTransaction),
            groupBy: e => e.PostedAt!.Value.Date,
            selector: g => new PostTimeseriesSlot
            {
                Date = g.Key,
                Earnings = g.Sum(e => e.DistrbuteTransaction?.AmountAfterCharges ?? 0),
                AvgEarnings = Math.Round(g.Average(e => e.DistrbuteTransaction?.AmountAfterCharges ?? 0), 2),

                Comments = g.Sum(e => e.PostValuation!.CommentCount),
                AvgComments = Math.Round(g.Average(e => e.PostValuation!.CommentCount), 2),

                Likes = g.Sum(e => e.PostValuation!.LikeCount),
                AvgLikes = Math.Round(g.Average(e => e.PostValuation!.LikeCount), 2),

                Views = g.Sum(e => e.PostValuation!.ViewCount),
                AvgViews = Math.Round(g.Average(e => e.PostValuation!.ViewCount), 2),

                EngagementRate = Math.Round(g.Sum(e => e.PostValuation!.EngagementRate), 2),

                AvgEngagementRate = Math.Round(g.Average(e => e.PostValuation!.EngagementRate), 2)
            });
        
        results = results.OrderBy(e => e.Date).ToList();

        return results.ToOkApiResponse();
    }

    private async Task<SocialPost> RetrievePost(SocialProfile profile, string postUrl)
    {
        var platform = profile.Platform!.Value;
        var displayName = platform.GetDisplayName();
        try
        {
            var embedding = platform switch
            {
                Platform.Twitter => await _twitterSdk.GetPost(profile, postUrl),
                Platform.Instagram => await _instagramSdk.GetPost(profile, postUrl),
                Platform.TikTok => await _tiktokSdk.GetPost(profile, postUrl),
                _ => throw new ArgumentException("The provided platform is not yet supported.")
            };

            return embedding;
        }
        catch
        {
            throw new BadRequest(
                $"We couldn’t find this {displayName} post. Please double-check the URL and make sure the post still exists on {displayName}.");
        }
    }

    private async Task SendPostSubmittedEmail(Post existingPost)
    {
        try
        {
            var templateData = new Dictionary<string, string>();
            templateData.Add("postId", existingPost.Id);
            templateData.Add("brandName", existingPost.Brand.Name);
            var reviewWindow = existingPost.ContentType!.Value.GetReviewWindow();
            templateData.Add("window", reviewWindow);
            templateData.Add("year", DateTime.UtcNow.Year.ToString());

            var personalization = new Personalization();
            personalization.Subject = "📣 New Post Submitted For Your Campaign On Distrbute!";
            templateData.Add("subject", personalization.Subject);
            personalization.Tos = new List<EmailAddress> { new(existingPost.Brand.Email.Address) };
            personalization.TemplateData = templateData;

            var mail = new BulkEmail();
            mail.Subject = personalization.Subject;
            mail.TemplateId = _mailTemplateConfig.AdPostedTemplateId;
            mail.Personalizations = [personalization];

            // Send mail
            await _emailClient.SendBulkAsync(mail);
        }
        catch (Exception)
        {
        }
    }

    private static PostDto GetResponseDto(Post post)
    {
        var resp = post.Adapt<PostDto>();
        resp.Payout = post.CampaignInvite!.Bid;
        return resp;
    }

    private async Task<CampaignInvite> GetInvite(Email principal, string id)
    {
        // fetch the invite for this distributor
        var invite = await _dbRepository
            .GetAsync<CampaignInvite>(q => q
                .IncludeWith(e => e.DistributorSocialAccount, e => e.Distributor, e => e.Email)
                .Where(e => e.DistributorSocialAccount.Distributor.Email.Address == principal.Address)
                .Where(e => e.Id == id)
                .IncludeWith(e => e.Campaign, e => e.Brand, e => e.Email));

        return invite;
    }
}
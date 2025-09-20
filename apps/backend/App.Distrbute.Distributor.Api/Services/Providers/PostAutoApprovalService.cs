using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Options;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Messaging.Sdk.Mail.Core;
using Microsoft.Extensions.Options;
using Persistence.Sdk.Core.Interfaces;
using SendGrid.Helpers.Mail;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Distributor.Api.Services.Providers;

public class PostAutoApprovalService : IPostAutoApprovalService
{
    private readonly IDbRepository _dbRepository;
    private readonly IEmailClient _emailClient;
    private readonly IJobSchedulingService _jobSchedulingService;
    private readonly MailTemplateConfig _mailTemplateConfig;

    public PostAutoApprovalService(
        IDbRepository dbRepository,
        IJobSchedulingService jobSchedulingService,
        IEmailClient emailClient,
        IOptions<MailTemplateConfig> mailTemplateConfig)
    {
        _dbRepository = dbRepository;
        _jobSchedulingService = jobSchedulingService;
        _emailClient = emailClient;
        _mailTemplateConfig = mailTemplateConfig.Value;
    }

    public async Task<PostApprovalTaskResponse> ApproveAsync(string postId)
    {
        Post pendingPost;
        try
        {
            pendingPost = await _dbRepository.GetAsync<Post>(q => q
                .Where(e => e.Id == postId && e.PostApprovalStatus == PostApprovalStatus.Pending)
                .IncludeWith(e => e.DistributorSocialAccount, e => e.Distributor, e => e.Email)
                .IncludeWith(e => e.CampaignInvite, e => e.Campaign, e => e.Brand));
        }
        catch (NotFound ex)
        {
            var noPendingPostResp = new PostApprovalTaskResponse();
            noPendingPostResp.Successful = false;
            noPendingPostResp.Retry = false;
            noPendingPostResp.Description = ex.Message;

            return noPendingPostResp;
        }

        try
        {
            // throws
            await _jobSchedulingService.SchedulePostPayoutAsync(pendingPost);

            pendingPost.PostApprovalStatus = PostApprovalStatus.Approved;
            await _dbRepository.UpdateAsync(pendingPost);

            // send email
            await SendPostApprovedEmail(pendingPost);
        }
        catch (Exception ex)
        {
            var failedUpdateResp = new PostApprovalTaskResponse();
            failedUpdateResp.Successful = false;
            failedUpdateResp.Retry = true;
            failedUpdateResp.MaxRetries = 3;
            failedUpdateResp.Description = ex.Message;

            return failedUpdateResp;
        }

        var successResp = new PostApprovalTaskResponse();
        successResp.Successful = true;
        successResp.Retry = false;

        return successResp;
    }

    private async Task SendPostApprovedEmail(Post existingPost)
    {
        try
        {
            var templateData = new Dictionary<string, string>();
            templateData.Add("postId", existingPost.Id);
            templateData.Add("name", existingPost.DistributorSocialAccount!.Distributor.Name);
            templateData.Add("brandName", existingPost.CampaignInvite!.Campaign.Brand.Name);
            templateData.Add("platform", existingPost.DistributorSocialAccount.Platform!.Value.GetDisplayName());
            templateData.Add("contentType", existingPost.ContentType!.Value.GetDisplayName());
            templateData.Add("amount", $"₵{existingPost.CampaignInvite.Bid:N}");
            templateData.Add("year", DateTime.UtcNow.Year.ToString());

            var personalization = new Personalization();
            personalization.Subject = "📣 Your Post Has Been Approved On Distrbute!";
            templateData.Add("subject", personalization.Subject);
            personalization.Tos = new List<EmailAddress> { new(existingPost.DistributorSocialAccount.Distributor.Email.Address) };
            personalization.TemplateData = templateData;

            var mail = new BulkEmail();
            mail.Subject = personalization.Subject;
            mail.TemplateId = _mailTemplateConfig.AdSystemApprovedTemplateId;
            mail.Personalizations = [personalization];

            // Send mail
            await _emailClient.SendBulkAsync(mail);
        }
        catch (Exception)
        {
        }
    }
}
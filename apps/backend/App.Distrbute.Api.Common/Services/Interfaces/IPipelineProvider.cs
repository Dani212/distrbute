using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Common.Models;
using ObjectStorage.Sdk.Dtos;
using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IPipelineProvider
{
    Task ExecuteMediaProcessingPipeline(Email principal, MediaProcessingReq req, string prefix);
    Task ExecutePostTrackingPipeline(TrackOnePostReq req);
    Task ExecuteInitSocialAccountValuePipeline<T>(T socialAccount) where T : SocialAccountBase;
    Task<CheckStatusResponse> ExecuteDepositProcessingPipeline(string clientReference);
    Task<PayoutTaskResponse> ExecutePayoutProcessingPipeline(string postId);
}
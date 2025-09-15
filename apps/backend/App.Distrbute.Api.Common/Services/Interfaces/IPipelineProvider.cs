using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Common.Models;
using ObjectStorage.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IPipelineProvider
{
    Task ExecuteMediaProcessingPipeline(Email principal, MediaProcessingReq req, string prefix);
    Task ExecutePostTrackingPipeline(string postId);
    Task ExecuteInitSocialAccountValuePipeline(string socialAccountId);
    Pipeline.Sdk.Core.Pipeline DepositProcessingPipeline();
    Task<CheckStatusResponse> ExecuteDepositProcessingPipeline(string clientReference);
}
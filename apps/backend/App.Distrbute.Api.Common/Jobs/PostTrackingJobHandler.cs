using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Models;
using Newtonsoft.Json;
using Persistence.Sdk.Core.Interfaces;
using Quartz;
using Scheduler.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Jobs;

public class PostTrackingJobHandler : IJob
{
    private readonly IPipelineProvider _pipelineProvider;
    private readonly IDbRepository _dbRepository;

    public PostTrackingJobHandler(
        IPipelineProvider pipelineProvider,
        IDbRepository dbRepository)
    {
        _pipelineProvider = pipelineProvider;
        _dbRepository = dbRepository;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var taskConfigJson = context.JobDetail.JobDataMap.GetString(nameof(TaskConfig))!;
            var taskConfig = JsonConvert.DeserializeObject<TaskConfig>(taskConfigJson)!;

            var id = taskConfig.UniqueId;
            var post = await _dbRepository.GetAsync<Post>(q => q.Where(p => p.Id == id));
            var distributorSocialAccount = post.DistributorSocialAccount;
            var socialProfile = distributorSocialAccount!.AsSocialProfile();

            var trackOnePostReq = new TrackOnePostReq();
            trackOnePostReq.SocialProfile = socialProfile;
            trackOnePostReq.Link = post.Link;

            await _pipelineProvider.ExecutePostTrackingPipeline(trackOnePostReq);
        }
        catch (Exception)
        {
        }
    }
}
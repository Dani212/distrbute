using Actors.Sdk;
using App.Distrbute.Api.Common.Actors.ActorMessages;
using App.Distrbute.Api.Common.Jobs;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Api.Common.Services.Providers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Scheduler.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Actors;

public class ProcessPayoutActor : BaseActor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ProcessPayoutActor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;

        ReceiveAsync<ProcessPayoutMessage>(DoProcessWalletDeposit);
    }

    /// <summary>
    ///     The whole purpose of an actor is to ensure queueing and ensure messages are processed one at a time
    /// </summary>
    /// <param name="message"></param>
    private async Task DoProcessWalletDeposit(ProcessPayoutMessage message)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPipelineProvider>();
        var res = await service.ExecutePayoutProcessingPipeline(message.Id);

        var taskConfigJson = message.JobExecutionContext.JobDetail.JobDataMap.GetString(nameof(TaskConfig))!;
        var taskConfig = JsonConvert.DeserializeObject<TaskConfig>(taskConfigJson)!;
        var failed = !res.Successful;
        var shouldRetry = res.Retry;

        if (failed && shouldRetry && taskConfig.RetryCount < res.MaxRetries)
            await ReSchedulerService.Reschedule<PayoutJobHandler>(message.JobExecutionContext);
    }
}
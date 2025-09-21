using Akka.Actor;
using Akka.Hosting;
using App.Distrbute.Api.Common.Actors;
using App.Distrbute.Api.Common.Actors.ActorMessages;
using Newtonsoft.Json;
using Quartz;
using Scheduler.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Jobs;

public class PayoutJobHandler : IJob
{
    private readonly IRequiredActor<ProcessPayoutActor> _actorRef;

    public PayoutJobHandler(IRequiredActor<ProcessPayoutActor> actorRef)
    {
        _actorRef = actorRef;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var taskConfigJson = context.JobDetail.JobDataMap.GetString(nameof(TaskConfig))!;
            var taskConfig = JsonConvert.DeserializeObject<TaskConfig>(taskConfigJson)!;

            var id = taskConfig.UniqueId;

            var actor = await _actorRef.GetAsync();
            var message = new ProcessPayoutMessage(id, context);

            actor.Tell(message);
        }
        catch (Exception)
        {
        }
    }
}
using Akka.Actor;
using Akka.Hosting;
using App.Distrbute.Api.Common.Actors;
using App.Distrbute.Api.Common.Actors.ActorMessages;
using Newtonsoft.Json;
using Quartz;
using Scheduler.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Jobs;

public class PostAutoApprovalJobHandler : IJob
{
    private readonly IRequiredActor<ProcessPostAutoApprovalActor> _actorRef;

    public PostAutoApprovalJobHandler(IRequiredActor<ProcessPostAutoApprovalActor> actorRef)
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
            var message = new ProcessPostAutoApprovalMessage(id, context);

            actor.Tell(message);
        }
        catch (Exception)
        {
        }
    }
}
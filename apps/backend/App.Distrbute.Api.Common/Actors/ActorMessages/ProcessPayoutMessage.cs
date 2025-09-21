using Quartz;

namespace App.Distrbute.Api.Common.Actors.ActorMessages;

public struct ProcessPayoutMessage
{
    public string Id { get; set; }
    public IJobExecutionContext JobExecutionContext { get; set; }

    public ProcessPayoutMessage(string id, IJobExecutionContext jobExecutionContext)
    {
        Id = id;
        JobExecutionContext = jobExecutionContext;
    }
}
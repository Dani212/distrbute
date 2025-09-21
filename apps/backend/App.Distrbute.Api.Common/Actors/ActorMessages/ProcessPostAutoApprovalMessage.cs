using Quartz;

namespace App.Distrbute.Api.Common.Actors.ActorMessages;

public struct ProcessPostAutoApprovalMessage
{
    public string Id { get; set; }
    public IJobExecutionContext JobExecutionContext { get; set; }

    public ProcessPostAutoApprovalMessage(string id, IJobExecutionContext jobExecutionContext)
    {
        Id = id;
        JobExecutionContext = jobExecutionContext;
    }
}
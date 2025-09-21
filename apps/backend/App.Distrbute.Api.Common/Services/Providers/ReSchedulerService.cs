using Newtonsoft.Json;
using Quartz;
using Scheduler.Sdk.Dtos;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Services.Providers;

public class ReSchedulerService
{
    public static async Task Reschedule<THandler>(IJobExecutionContext context) where THandler : IJob
    {
        var newJobDetail = context.JobDetail.Clone();
        var taskConfigJson = context.JobDetail.JobDataMap.GetString(nameof(TaskConfig))!;
        var taskConfig = JsonConvert.DeserializeObject<TaskConfig>(taskConfigJson)!;
        taskConfig.RetryCount = taskConfig.RetryCount + 1;
        newJobDetail.JobDataMap[nameof(TaskConfig)] = taskConfig.Serialize();

        var now = DateTimeOffset.UtcNow;
        var runDate = now.AddMinutes(taskConfig.RetryInterval);
        var minute = runDate.Minute;
        var hour = runDate.Hour;
        var day = runDate.Day;
        var month = runDate.Month;
        var year = runDate.Year;
        var cron = $"0 {minute} {hour} {day} {month} ? {year}";

        var oldTrigger = context.Trigger;
        var retryScheduled = DateTime.UtcNow;

        var job = JobBuilder.Create<THandler>()
            .WithIdentity(newJobDetail.Key)
            .SetJobData(newJobDetail.JobDataMap)
            .StoreDurably(false)
            .RequestRecovery()
            .Build();

        var newTrigger = TriggerBuilder.Create()
            .WithIdentity($"{oldTrigger.Key.Name}-retry-{retryScheduled}", oldTrigger.Key.Group)
            .WithDescription(oldTrigger.Description)
            .WithCronSchedule(cron)
            .UsingJobData(newJobDetail.JobDataMap)
            .StartAt(now)
            .EndAt(runDate.AddMinutes(10))
            .Build();

        await context.Scheduler.ScheduleJob(job, newTrigger);
    }
}
using App.Distrbute.Api.Common.Jobs;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Extensions;
using App.Distrbute.Common.Models;
using Scheduler.Sdk.Dtos;
using Scheduler.Sdk.Services.Interfaces;

namespace App.Distrbute.Api.Common.Services.Providers;

public class JobSchedulingService : IJobSchedulingService
{
    private readonly ISchedulerSdk _schedulerSdk;

    public JobSchedulingService(ISchedulerSdk schedulerSdk)
    {
        _schedulerSdk = schedulerSdk;
    }

    public async Task SchedulePostPayoutAsync(Post post)
    {
        // schedule payout
        var task = new TaskScheduleRequest();
        var now = DateTime.UtcNow;
        task.Name = $"{post.Id}-{now}-ad-payout";
        task.Description = $"Ad payout schedule for {post.Id}";
        var contentType = post.ContentType!.Value;
        var submittedAt = post.CreatedAt!.Value;
        var dueDate = contentType.GetPayoutTime(submittedAt);
        // var dueDate = DateTimeOffset.UtcNow.AddMinutes(3); // for tests
        task.StartDate = dueDate;
        var taskConfig = new TaskConfig();
        taskConfig.RetryCount = 1;
        taskConfig.RetryInterval = 5;
        taskConfig.UniqueId = post.Id;
        task.TaskConfig = taskConfig;

        // throws
        await _schedulerSdk.ScheduleTaskAsync<PayoutJobHandler>(task);
    }

    public async Task SchedulePostAutoApprovalAsync(Post post)
    {
        // schedule auto approval
        var task = new TaskScheduleRequest();
        var now = DateTime.UtcNow;
        task.Name = $"{post.Id}-{now}-ad-auto-approval";
        task.Description = $"Ad approval schedule for {post.Id}";
        var submittedAt = post.CreatedAt!.Value;
        var contentType = post.ContentType!.Value;
        var dueDate = contentType.GetAutoApprovalTime(submittedAt);
        // var dueDate = DateTimeOffset.UtcNow.AddMinutes(1); // for tests
        task.StartDate = dueDate;
        var taskConfig = new TaskConfig();
        taskConfig.RetryCount = 1;
        taskConfig.RetryInterval = 5;
        taskConfig.UniqueId = post.Id;
        task.TaskConfig = taskConfig;

        // throws
        await _schedulerSdk.ScheduleTaskAsync<PostAutoApprovalJobHandler>(task);
    }

    public async Task SchedulePostTrackingAsync(Post post)
    {
        var contentType = post.ContentType!.Value;
        var submittedAt = post.CreatedAt!.Value;

        var timeSpans = contentType.GetTrackingPeriods();

        foreach (var timeSpan in timeSpans)
        {
            var dueDate = submittedAt.Add(timeSpan);

            // schedule post tracking
            var task = new TaskScheduleRequest();
            task.Name = $"{post.Id}-{dueDate}-ad-tracking";
            task.Description = $"Ad tracking schedule for {post.Id}";
            task.StartDate = dueDate;
            var taskConfig = new TaskConfig();
            taskConfig.RetryCount = 1;
            taskConfig.RetryInterval = 5;
            taskConfig.UniqueId = post.Id;
            task.TaskConfig = taskConfig;

            // throws
            await _schedulerSdk.ScheduleTaskAsync<PostTrackingJobHandler>(task);
        }
    }
}
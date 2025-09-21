using App.Distrbute.Common.Models;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IJobSchedulingService
{
    Task SchedulePostPayoutAsync(Post post);
    Task SchedulePostAutoApprovalAsync(Post post);
    Task SchedulePostTrackingAsync(Post post);
}
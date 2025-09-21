using App.Distrbute.Api.Common.Dtos.Post;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IPostMetricService
{
    Task<TrackedPost> GatherAsync(TrackOnePostReq req);
    Task<TrackedPosts> GatherManyAsync(TrackPostBase req);
}
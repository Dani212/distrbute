using App.Distrbute.Api.Common.Dtos.Post;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IPostValuationService
{
    public ValuedPost Value(TrackedPost trackedPost);
    public ValuedPosts ValueMany(TrackedPosts trackedPosts);
}
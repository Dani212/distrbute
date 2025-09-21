namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IPostAutoApprovalService
{
    public Task<PostApprovalTaskResponse> ApproveAsync(string postId);
}

public class PostApprovalTaskResponse
{
    public bool Successful { get; set; }
    public bool Retry { get; set; }
    public int MaxRetries { get; set; }
    public string Description { get; set; } = null!;
}
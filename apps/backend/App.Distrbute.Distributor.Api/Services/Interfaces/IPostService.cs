using System.Collections.Generic;
using System.Threading.Tasks;
using App.Distrbute.Api.Common.Dtos.Post;
using App.Distrbute.Common.Models;
using App.Distrbute.Distributor.Api.Dtos;
using Persistence.Sdk.Dtos;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Services.Interfaces;

public interface IPostService
{
    public Task<IApiResponse<PostDto>> CreateAsync(Email principal, CreatePostReq req);
    public Task<IApiResponse<PostDto>> GetAsync(Email principal, string id, PostsQueryRequest query);
    public Task<IApiResponse<PagedResult<PostDto>>> AllAsync(Email principal, PostPageRequest page);
    Task<IApiResponse<CampaignSummary>> SummaryAsync(Email principal);

    Task<IApiResponse<List<PostTimeseriesSlot>>> TimeseriesAsync(Email principal,
        PostTimeseriesQueryRequest queryRequest);
}
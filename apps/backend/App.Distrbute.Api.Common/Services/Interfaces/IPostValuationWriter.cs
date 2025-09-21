using App.Distrbute.Api.Common.Dtos.Post;
using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IPostValuationWriter
{
    Task<SocialProfile> WriteAsync(ValuedPost req);
    Task<SocialProfile> WriteManyAsync(ValuedPosts req);
}
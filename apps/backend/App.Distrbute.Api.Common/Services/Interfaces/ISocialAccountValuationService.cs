using Socials.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface ISocialAccountValuationService
{
    Task<bool> ValueAsync(SocialProfile socialProfile);
}
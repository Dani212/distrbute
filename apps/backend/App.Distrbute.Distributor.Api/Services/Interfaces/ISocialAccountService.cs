using System.Collections.Generic;
using System.Threading.Tasks;
using App.Distrbute.Common.Models;
using App.Distrbute.Distributor.Api.Dtos;
using Persistence.Sdk.Dtos;
using Socials.Sdk.Services.Interfaces;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Services.Interfaces;

public interface ISocialAccountService : IOauthAccountHandler
{
    Task<IApiResponse<SocialAccountDto>> GetAsync(Email principal, string id);
    Task<IApiResponse<List<ConnectedPlatformsSummary>>> GetConnectedAccountsSummary(Email principal);
    Task<IApiResponse<PagedResult<SocialAccountDto>>> GetConnectedAccounts(Email principal, SocialAccountsPageRequest page);
    Task<IApiResponse<SocialAccountDto>> UpdatePreferences(Email principal, string id, SocialAccountPreferencesReq req);
}
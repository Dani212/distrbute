using App.Distrbute.Api.Common.Dtos;
using App.Distrbute.Api.Common.Dtos.Wallet;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Common.Models;
using Persistence.Sdk.Dtos;
using Utility.Sdk.Dtos;
using WalletPageRequest = App.Distrbute.Api.Common.Dtos.WalletPageRequest;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IWalletService
{
    Task<IApiResponse<WalletResDto>> GetAsync<T>(Email principal, string id, WalletQueryRequest query)
        where T : BaseBrandDistributor;

    Task<IApiResponse<PagedResult<WalletResDto>>> AllAsync<T>(Email principal, WalletPageRequest filter)
        where T : BaseBrandDistributor;
}
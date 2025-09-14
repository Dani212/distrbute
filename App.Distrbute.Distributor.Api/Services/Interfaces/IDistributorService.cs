using System.Threading.Tasks;
using App.Distrbute.Api.Common.Dtos;
using App.Distrbute.Api.Common.Dtos.Distributor;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Common.Models;
using Persistence.Sdk.Dtos;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Services.Interfaces;

public interface IDistributorService
{
    Task<IApiResponse<DistributorDto>> CreateAsync(Email principal, CreateDistributorDto dto);
    Task<IApiResponse<DistributorDto>> UpdateAsync(Email principal, CreateDistributorDto dto);
    Task<IApiResponse<DistributorDto>> GetAsync(Email principal);
    Task<IApiResponse<WalletResDto>> GetEarned(Email principal);

    Task<IApiResponse<PagedResult<DistrbuteTransactionDto>>> AllTransactionAsync(Email principal,
        TransactionPageRequest filter);

    Task<IApiResponse<InitializeTransactionDto>> LinkWalletAsync(Email principal, string? callbackUrl);
}
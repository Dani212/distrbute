using System.Collections.Generic;
using System.Threading.Tasks;
using App.Distrbute.Common.Dtos;
using Persistence.Sdk.Dtos;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Services.Interfaces;

public interface IDistributorNicheService
{
    Task<IApiResponse<NicheDto>> CreateAsync(NicheDto dto);
    Task<IApiResponse<List<NicheDto>>> CreateManyAsync(List<NicheDto> dtos);
    Task<IApiResponse<NicheDto>> UpdateAsync(string name, NicheDto dto);
    Task<IApiResponse<bool>> DeleteAsync(string name);
    Task<IApiResponse<PagedResult<NicheDto>>> AllAsync();
}
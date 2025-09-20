using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Common.Models;
using App.Distrbute.Distributor.Api.Services.Interfaces;
using Mapster;
using Persistence.Sdk.Core.Interfaces;
using Persistence.Sdk.Dtos;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Distributor.Api.Services.Providers;

public class DistributorNicheService : IDistributorNicheService
{
    private IDbRepository _dbRepository;

    public DistributorNicheService(IDbRepository dbRepository)
    {
        _dbRepository = dbRepository;
    }
    
    public async Task<IApiResponse<NicheDto>> CreateAsync(NicheDto dto)
    {
        var niche = dto.Adapt<DistributorNiche>();
        var added = await _dbRepository.AddAsync(niche);
        
        var response = added.Adapt<NicheDto>();
        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<List<NicheDto>>> CreateManyAsync(List<NicheDto> dtos)
    {
        var niches = dtos.Select(d => d.Adapt<DistributorNiche>()).ToList();
        var added = await _dbRepository.AddManyAsync(niches);
        
        var responses = added.Select(d => d.Adapt<NicheDto>()).ToList();
        
        return responses.ToOkApiResponse();
    }

    public async Task<IApiResponse<NicheDto>> UpdateAsync(string name, NicheDto dto)
    {
        var existing = await _dbRepository.GetAsync<DistributorNiche>(q => q
            .Where(e => e.Name == name));

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        
        var updated = await _dbRepository.UpdateAsync(existing);
        
        var response = updated.Adapt<NicheDto>();
        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<bool>> DeleteAsync(string name)
    {
        await _dbRepository.DeleteAsync<DistributorNiche>(q => q
            .Where(e => e.Name == name));
        
        return true.ToOkApiResponse();
    }

    public async Task<IApiResponse<PagedResult<NicheDto>>> AllAsync()
    {
        var paged = await _dbRepository.GetAllAsync<DistributorNiche>(q => q
            .Where(e => true));

        var response = new PagedResult<NicheDto>();
        response.Page = paged.Page;
        response.PageSize = paged.PageSize;
        response.TotalCount = paged.TotalCount;
        response.Data = paged.Data.Select(d => d.Adapt<NicheDto>()).ToList();
        
        return response.ToOkApiResponse();
    }
}
using App.Distrbute.Api.Common.Dtos;
using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Dtos;
using App.Distrbute.Common.Enums;
using App.Distrbute.Common.Models;
using Ledgr.Sdk.Dtos.Wallet.Balance;
using Ledgr.Sdk.Exceptions;
using Ledgr.Sdk.Services.Interfaces;
using Persistence.Sdk.Core.Interfaces;
using Persistence.Sdk.Dtos;
using Utility.Sdk.Dtos;
using Utility.Sdk.Exceptions;
using Utility.Sdk.Extensions;
using WalletPageRequest = App.Distrbute.Api.Common.Dtos.WalletPageRequest;

namespace App.Distrbute.Api.Common.Services.Providers;

public class WalletService : IWalletService
{
    private readonly IDbRepository _repository;
    private readonly IWalletSdk _walletSdk;

    public WalletService(IDbRepository repository,
        IWalletSdk walletSdk)
    {
        _repository = repository;
        _walletSdk = walletSdk;
    }

    public async Task<IApiResponse<WalletResDto>> GetAsync<T>(Email principal, string id, WalletQueryRequest query)
        where T : BaseBrandDistributor
    {
        var type = typeof(T);
        Wallet wallet;
        if (type == typeof(Brand))
            wallet = await GetBrandWallet(principal, id, query.TenantId);

        else
            wallet = await _repository.GetAsync<Wallet>(q => q
                .Where(e => e.Distributor != null)
                .IncludeWith(e => e.Distributor, e => e.Email)
                .Where(e => e.Distributor!.Email.Address == principal.Address && e.Id == id && e.Active));

        var ledgerBalance = await _walletSdk
            .GetBalanceAsync(wallet.AccountId)
            .CatchAndThrowAsOrReturn<LedgerException, FailedDependency, WalletBalanceDto>
                ("We're having trouble processing request on your wallet. Please try again in a few minutes.");

        var availableBalance = ledgerBalance.AvailableBalance;
        var balance = ledgerBalance.AccountBalance;

        var response = wallet.ToResponseDto(availableBalance, balance);

        return response.ToOkApiResponse();
    }

    public async Task<IApiResponse<PagedResult<WalletResDto>>> AllAsync<T>(Email principal, WalletPageRequest filter)
        where T : BaseBrandDistributor
    {
        var type = typeof(T);
        PagedResult<Wallet> paged;
        if (type == typeof(Brand))
        {
            var brandWhereOwnsOrManager = await GetBrandWhereOwnsOrManages(principal, filter.TenantId);
        
            paged = await _repository.GetAllAsync<Wallet>(q => q
                .IncludeWith(e => e.Brand, e => e.Email)
                .Where(e => e.Brand != null && e.Brand!.Id == brandWhereOwnsOrManager.Id)
                .Where(e => e.Brand!.Email.Address == principal.Address)
                .Where(e => (filter.Type == null || e.Type == filter.Type) &&
                            (filter.Provider == null || e.Provider == filter.Provider && e.Active))
                .Page(filter));
        }
        else
        {
            paged = await _repository.GetAllAsync<Wallet>(
                q => q
                    .Where(e => e.Distributor != null)
                    .IncludeWith(e => e.Distributor, e => e.Email)
                    .Where(e => e.Distributor!.Email.Address == principal.Address)
                    .Where(e => (filter.Type == null || e.Type == filter.Type) &&
                                (filter.Provider == null || e.Provider == filter.Provider && e.Active))
                    .Page(filter)
            );
        }

        var response = new PagedResult<WalletResDto>
        {
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalCount = paged.TotalCount,
            Data = paged.Data.ConvertAll(c => c.ToResponseDto(0, 0))
        };

        return response.ToOkApiResponse();
    }

    private async Task<Wallet> GetBrandWallet(Email principal, string id, string brandId)
    {
        var brandWhereOwnsOrManager = await GetBrandWhereOwnsOrManages(principal, brandId);
        
        var wallet = await _repository.GetAsync<Wallet>(q => q
            .Include(e => e.Brand)
            .Where(e => e.Brand != null && e.Brand.Id == brandWhereOwnsOrManager.Id)
            .Where(e =>  e.Id == id && e.Active));
        
        return wallet;
    }

    private async Task<BrandMember> GetBrandWhereOwnsOrManages(Email principal, string brandId)
    {
        var brandWhereOwnsOrManager = await _repository.GetAsync<BrandMember>(q => q
            .Include(e => e.Brand)
            .Where(e => e.Brand.Id == brandId)
            .Include(e => e.Email)
            .Where(e => e.Email.Address == principal.Address && (BrandRole.Owner == e.Role || BrandRole.Manager == e.Role)));
        return brandWhereOwnsOrManager;
    }
}
using DistributorApi.Models;

namespace DistributorApi.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<object>> GetCatalogAsync();
        Task<object?> GetProductByIdAsync(int productId);
        Task<IEnumerable<object>> SearchProductsAsync(string query);
        Task<IEnumerable<object>> GetProductsByCategoryAsync(string category);
    }
}

using BrandApi.Models;

namespace BrandApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsByBrandIdAsync(int brandId);
    }
}

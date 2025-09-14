using Microsoft.EntityFrameworkCore;
using BrandApi.Data;
using BrandApi.Models;

namespace BrandApi.Services
{
    public class ProductService : IProductService
    {
        private readonly BrandDbContext _context;

        public ProductService(BrandDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return null;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Sku = product.Sku;
            existingProduct.Price = product.Price;
            existingProduct.Currency = product.Currency;
            existingProduct.Category = product.Category;
            existingProduct.Images = product.Images;
            existingProduct.Specifications = product.Specifications;
            existingProduct.IsActive = product.IsActive;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandIdAsync(int brandId)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Where(p => p.BrandId == brandId && p.IsActive)
                .ToListAsync();
        }
    }
}

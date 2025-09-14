using DistributorApi.Models;

namespace DistributorApi.Services
{
    public class CatalogService : ICatalogService
    {
        // In a real application, this would call the Brand API or a shared database
        // For now, we'll return mock data
        public async Task<IEnumerable<object>> GetCatalogAsync()
        {
            // Simulate API call delay
            await Task.Delay(100);

            return new[]
            {
                new
                {
                    Id = 1,
                    Name = "Wireless Headphones Pro",
                    Description = "High-quality wireless headphones with noise cancellation",
                    Sku = "WH-PRO-001",
                    Price = 299.99m,
                    Currency = "USD",
                    Brand = "TechBrand",
                    Category = "Electronics",
                    Images = new[] { "https://example.com/headphones1.jpg", "https://example.com/headphones2.jpg" },
                    Specifications = new { battery = "30 hours", connectivity = "Bluetooth 5.0", weight = "250g" },
                    IsActive = true
                },
                new
                {
                    Id = 2,
                    Name = "Smart Watch Series 5",
                    Description = "Advanced smartwatch with health monitoring features",
                    Sku = "SW-S5-001",
                    Price = 399.99m,
                    Currency = "USD",
                    Brand = "TechBrand",
                    Category = "Electronics",
                    Images = new[] { "https://example.com/smartwatch1.jpg" },
                    Specifications = new { display = "1.9 inch OLED", battery = "48 hours", water_resistance = "5ATM" },
                    IsActive = true
                },
                new
                {
                    Id = 3,
                    Name = "Premium T-Shirt",
                    Description = "100% organic cotton t-shirt with modern design",
                    Sku = "TS-PREM-001",
                    Price = 49.99m,
                    Currency = "USD",
                    Brand = "FashionCorp",
                    Category = "Clothing",
                    Images = new[] { "https://example.com/tshirt1.jpg" },
                    Specifications = new { material = "100% organic cotton", sizes = "S, M, L, XL", colors = "Black, White, Navy" },
                    IsActive = true
                }
            };
        }

        public async Task<object?> GetProductByIdAsync(int productId)
        {
            var catalog = await GetCatalogAsync();
            return catalog.FirstOrDefault(p => ((dynamic)p).Id == productId);
        }

        public async Task<IEnumerable<object>> SearchProductsAsync(string query)
        {
            var catalog = await GetCatalogAsync();
            return catalog.Where(p => 
                ((dynamic)p).Name.ToLower().Contains(query.ToLower()) ||
                ((dynamic)p).Description.ToLower().Contains(query.ToLower())
            );
        }

        public async Task<IEnumerable<object>> GetProductsByCategoryAsync(string category)
        {
            var catalog = await GetCatalogAsync();
            return catalog.Where(p => 
                ((dynamic)p).Category.ToLower() == category.ToLower()
            );
        }
    }
}

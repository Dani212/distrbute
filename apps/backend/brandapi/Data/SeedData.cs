using BrandApi.Models;

namespace BrandApi.Data
{
    public static class SeedData
    {
        public static void Initialize(BrandDbContext context)
        {
            if (context.Brands.Any())
            {
                return; // Database has been seeded
            }

            var brands = new[]
            {
                new Brand
                {
                    Id = 1,
                    Name = "TechBrand",
                    Description = "Leading technology products for modern businesses",
                    Logo = "https://example.com/techbrand-logo.png",
                    Website = "https://techbrand.com",
                    ContactEmail = "contact@techbrand.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Brand
                {
                    Id = 2,
                    Name = "FashionCorp",
                    Description = "Premium fashion and lifestyle products",
                    Logo = "https://example.com/fashioncorp-logo.png",
                    Website = "https://fashioncorp.com",
                    ContactEmail = "contact@fashioncorp.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Brands.AddRange(brands);

            var products = new[]
            {
                new Product
                {
                    Id = 1,
                    Name = "Wireless Headphones Pro",
                    Description = "High-quality wireless headphones with noise cancellation",
                    Sku = "WH-PRO-001",
                    Price = 299.99m,
                    Currency = "USD",
                    BrandId = 1,
                    Category = "Electronics",
                    Images = "[\"https://example.com/headphones1.jpg\", \"https://example.com/headphones2.jpg\"]",
                    Specifications = "{\"battery\": \"30 hours\", \"connectivity\": \"Bluetooth 5.0\", \"weight\": \"250g\"}",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Smart Watch Series 5",
                    Description = "Advanced smartwatch with health monitoring features",
                    Sku = "SW-S5-001",
                    Price = 399.99m,
                    Currency = "USD",
                    BrandId = 1,
                    Category = "Electronics",
                    Images = "[\"https://example.com/smartwatch1.jpg\"]",
                    Specifications = "{\"display\": \"1.9 inch OLED\", \"battery\": \"48 hours\", \"water_resistance\": \"5ATM\"}",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "Premium T-Shirt",
                    Description = "100% organic cotton t-shirt with modern design",
                    Sku = "TS-PREM-001",
                    Price = 49.99m,
                    Currency = "USD",
                    BrandId = 2,
                    Category = "Clothing",
                    Images = "[\"https://example.com/tshirt1.jpg\"]",
                    Specifications = "{\"material\": \"100% organic cotton\", \"sizes\": \"S, M, L, XL\", \"colors\": \"Black, White, Navy\"}",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Products.AddRange(products);

            var orders = new[]
            {
                new Order
                {
                    Id = 1,
                    DistributorId = 1,
                    BrandId = 1,
                    Items = "[{\"productId\": 1, \"quantity\": 10, \"unitPrice\": 299.99, \"totalPrice\": 2999.90}]",
                    TotalAmount = 2999.90m,
                    Currency = "USD",
                    Status = "Pending",
                    ShippingAddress = "{\"street\": \"123 Main St\", \"city\": \"New York\", \"state\": \"NY\", \"postalCode\": \"10001\", \"country\": \"USA\"}",
                    BillingAddress = "{\"street\": \"123 Main St\", \"city\": \"New York\", \"state\": \"NY\", \"postalCode\": \"10001\", \"country\": \"USA\"}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Orders.AddRange(orders);

            context.SaveChanges();
        }
    }
}

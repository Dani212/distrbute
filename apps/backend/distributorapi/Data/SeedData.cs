using DistributorApi.Models;

namespace DistributorApi.Data
{
    public static class SeedData
    {
        public static void Initialize(DistributorDbContext context)
        {
            if (context.Distributors.Any())
            {
                return; // Database has been seeded
            }

            var distributors = new[]
            {
                new Distributor
                {
                    Id = 1,
                    Name = "Tech Distributors Inc.",
                    Description = "Leading technology distributor serving retail and B2B markets",
                    Logo = "https://example.com/techdist-logo.png",
                    Website = "https://techdist.com",
                    ContactEmail = "contact@techdist.com",
                    BusinessType = "Wholesaler",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Distributor
                {
                    Id = 2,
                    Name = "Retail Solutions Ltd.",
                    Description = "Comprehensive retail distribution solutions",
                    Logo = "https://example.com/retailsol-logo.png",
                    Website = "https://retailsol.com",
                    ContactEmail = "contact@retailsol.com",
                    BusinessType = "Retailer",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Distributors.AddRange(distributors);

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
                    ShippingAddress = "{\"street\": \"456 Business Ave\", \"city\": \"Los Angeles\", \"state\": \"CA\", \"postalCode\": \"90210\", \"country\": \"USA\"}",
                    BillingAddress = "{\"street\": \"456 Business Ave\", \"city\": \"Los Angeles\", \"state\": \"CA\", \"postalCode\": \"90210\", \"country\": \"USA\"}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Order
                {
                    Id = 2,
                    DistributorId = 2,
                    BrandId = 2,
                    Items = "[{\"productId\": 3, \"quantity\": 50, \"unitPrice\": 49.99, \"totalPrice\": 2499.50}]",
                    TotalAmount = 2499.50m,
                    Currency = "USD",
                    Status = "Confirmed",
                    ShippingAddress = "{\"street\": \"789 Commerce St\", \"city\": \"Chicago\", \"state\": \"IL\", \"postalCode\": \"60601\", \"country\": \"USA\"}",
                    BillingAddress = "{\"street\": \"789 Commerce St\", \"city\": \"Chicago\", \"state\": \"IL\", \"postalCode\": \"60601\", \"country\": \"USA\"}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Orders.AddRange(orders);

            context.SaveChanges();
        }
    }
}

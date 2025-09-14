using System.ComponentModel.DataAnnotations;

namespace DistributorApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public int DistributorId { get; set; }
        
        [Required]
        public int BrandId { get; set; }
        
        public string Items { get; set; } = string.Empty; // JSON array of order items
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public string Status { get; set; } = "Pending";
        
        public string ShippingAddress { get; set; } = string.Empty; // JSON object
        
        public string BillingAddress { get; set; } = string.Empty; // JSON object
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Distributor Distributor { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace BrandApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Sku { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";
        
        [Required]
        public int BrandId { get; set; }
        
        [Required]
        public string Category { get; set; } = string.Empty;
        
        public string Images { get; set; } = string.Empty; // JSON array of image URLs
        
        public string Specifications { get; set; } = string.Empty; // JSON object
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Brand Brand { get; set; } = null!;
    }
}

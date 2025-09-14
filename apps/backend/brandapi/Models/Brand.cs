using System.ComponentModel.DataAnnotations;

namespace BrandApi.Models
{
    public class Brand
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Logo { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string Website { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string ContactEmail { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

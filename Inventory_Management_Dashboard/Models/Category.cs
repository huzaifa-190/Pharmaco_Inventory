using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_Dashboard.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // Navigation property for related Products
        public List<Product>? Products { get; set; }
    }
}

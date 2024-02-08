using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public Category Category { get; set; } = new Category();

        public int CategoryId { get; set; }

        public bool IsDeleted { get; set; }
    }
}

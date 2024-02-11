using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Product
    {
        public Product()
        {
            Description = string.Empty;
            UserId = string.Empty;
            Name = string.Empty;
            User = new ApplicationUser();
            Category = new Category();
            CartsProducts = new HashSet<CartsProducts>();
            Category = new Category();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public Category Category { get; set; }

        public int CategoryId { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<CartsProducts> CartsProducts { get; set; }
    }
}

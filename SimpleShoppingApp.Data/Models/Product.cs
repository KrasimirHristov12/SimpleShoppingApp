using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class Product
    {
        public Product()
        {
            CartsProducts = new HashSet<CartsProducts>();
            OrdersProducts = new HashSet<OrdersProducts>();
            UsersRating = new HashSet<UsersRating>();
            Images = new HashSet<Image>();
        }
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public double Rating { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public Category Category { get; set; } = null!;

        public int CategoryId { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<CartsProducts> CartsProducts { get; set; }

        public ICollection<OrdersProducts> OrdersProducts { get; set; }

        public ICollection<UsersRating> UsersRating { get; set; }

        public ICollection<Image> Images { get; set; }
    }
}

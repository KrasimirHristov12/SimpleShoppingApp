using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class CartsProducts
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        [Required]
        public ShoppingCart Cart { get; set; } = null!;

        public int ProductId { get; set; }

        [Required]
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }
    }
}

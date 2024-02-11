using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class CartsProducts
    {
        public CartsProducts()
        {
            Product = new Product();
            Cart = new ShoppingCart();
        }
        public int Id { get; set; }

        public int CartId { get; set; }

        [Required]
        public ShoppingCart Cart { get; set; }

        public int ProductId { get; set; }

        [Required]
        public Product Product { get; set; }

        public bool IsDeleted { get; set; }
    }
}

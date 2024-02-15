using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class OrdersProducts
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required]
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }

        [Required]
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}

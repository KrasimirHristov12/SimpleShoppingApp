using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            CartsProducts = new HashSet<CartsProducts>();
        }

        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public ICollection<CartsProducts> CartsProducts { get; set; }
    }
}

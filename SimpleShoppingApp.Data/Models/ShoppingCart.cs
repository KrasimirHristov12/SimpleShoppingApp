using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            UserId = string.Empty;
            CartsProducts = new HashSet<CartsProducts>();
            User = new ApplicationUser();
        }

        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<CartsProducts> CartsProducts { get; set; }
    }
}

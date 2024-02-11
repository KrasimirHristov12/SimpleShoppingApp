using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Products = new HashSet<Product>();
            Cart = new ShoppingCart();
        }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]   
        public string LastName { get; set; }

        public ICollection<Product> Products { get; set; }

        [Required]
        public ShoppingCart Cart { get; set; }

        public int CartId { get; set; }


    }
}

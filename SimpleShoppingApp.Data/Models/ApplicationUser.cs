using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>();
        }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]   
        public string LastName { get; set; } = null!;

        public ICollection<Product> Products { get; set; }

        [Required]
        public ShoppingCart Cart { get; set; } = null!;

        public int CartId { get; set; }

        public ICollection<Order> Orders { get; set; }


    }
}

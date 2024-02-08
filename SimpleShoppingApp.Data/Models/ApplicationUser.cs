using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Products = new HashSet<Product>();
        }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]   
        public string? LastName { get; set; }

        public ICollection<Product> Products { get; set; }


    }
}

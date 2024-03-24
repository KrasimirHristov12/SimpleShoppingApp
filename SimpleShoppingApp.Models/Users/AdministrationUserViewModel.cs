using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Users
{
    public class AdministrationUserViewModel
    {
        public string Id { get; set; } = null!;

        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [BindNever]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [RegularExpression("^([A-Za-z][A-Za-z]+){2,50}$")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [RegularExpression("^([A-Za-z][A-Za-z]+){2,50}$")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Account
{
    public class RegisterInputModel
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression("^([A-Za-z][A-Za-z]+){2,50}$")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression("^([A-Za-z][A-Za-z]+){2,50}$")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(6)]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;
    }
}

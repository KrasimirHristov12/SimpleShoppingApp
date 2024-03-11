using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Account
{
    public class NewPasswordInputModel
    {
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

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 6)]
        public string Email { get; set; } = null!;
    }
}

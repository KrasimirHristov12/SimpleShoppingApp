using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Account
{
    public class NewPasswordInputModel
    {
        [Required]
        [MinLength(PasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [MinLength(PasswordMinLength)]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [Display(Name = ConfirmPasswordDisplay)]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        public string Email { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Account
{
    public class RegisterInputModel
    {
        [Required]
        [StringLength(EmailMaxLength, MinimumLength = EmailMinLength)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        [Display(Name = FirstNameDisplay)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(FirstLastNameMaxLength, MinimumLength = FirstLastNameMinLength)]
        [Display(Name = LastNameDisplay)]
        public string LastName { get; set; } = null!;

        [Required]
        [Phone]
        [Display(Name = PhoneNumberDisplay)]
        public string PhoneNumber { get; set; } = null!;

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
    }
}

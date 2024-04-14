using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Account
{
    public class ChangePasswordInputModel
    {
        [Required]
        [MinLength(PasswordMinLength)]
        [DataType(DataType.Password)]
        [Display(Name = CurrentPasswordDisplay)]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [MinLength(PasswordMinLength)]
        [DataType(DataType.Password)]
        [Display(Name = NewPasswordDisplay)]
        public string NewPassword { get; set; } = null!;

        [Required]
        [MinLength(PasswordMinLength)]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword))]
        [Display(Name = ConfirmNewPasswordDisplay)]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}

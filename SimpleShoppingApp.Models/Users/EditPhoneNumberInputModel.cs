using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Users
{
    public class EditPhoneNumberInputModel
    {
        [Required]
        [RegularExpression(PhoneRegex, ErrorMessage = PhoneNumberErrorMessage)]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
    }
}

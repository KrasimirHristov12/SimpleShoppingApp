using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Users
{
    public class EditFullNameInputModel
    {
        [Required]
        [RegularExpression(FullNameRegex, ErrorMessage = FullNameErrorMessage)]
        public string FullName { get; set; } = null!;
    }
}

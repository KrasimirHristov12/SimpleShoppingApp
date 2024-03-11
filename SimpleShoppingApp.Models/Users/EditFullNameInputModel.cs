using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Users
{
    public class EditFullNameInputModel
    {
        [Required]
        [RegularExpression("^([A-Za-z][A-Za-z]+){2,50}\\s([A-Za-z][A-Za-z]+){2,50}$")]
        public string FullName { get; set; } = null!;
    }
}

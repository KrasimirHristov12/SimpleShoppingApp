using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Users
{
    public class EditFullNameInputModel
    {
        [Required]
        public string FullName { get; set; } = null!;
    }
}

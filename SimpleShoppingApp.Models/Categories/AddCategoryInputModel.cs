using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Categories
{
    public class AddCategoryInputModel
    {
        [Required]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength)]
        [Display(Name = CategoryNameDisplay)]
        public string Name { get; set; } = null!;
    }
}

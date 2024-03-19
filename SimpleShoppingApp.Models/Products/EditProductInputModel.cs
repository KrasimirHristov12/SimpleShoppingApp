using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Products
{
    public class EditProductInputModel : ProductInputModel, IValidatableObject
    {
        [Display(Name = "Has Discount")]
        public bool HasDiscount { get; set; }

        [Display(Name = "New Price")]
        public decimal? NewPrice { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!HasDiscount && NewPrice != null)
            {
                yield return new ValidationResult("Please check Has Discount or delete the specified new price");
            }
            if (HasDiscount && NewPrice == null)
            {
                yield return new ValidationResult("Please specify new price");
            }
        }
    }
}

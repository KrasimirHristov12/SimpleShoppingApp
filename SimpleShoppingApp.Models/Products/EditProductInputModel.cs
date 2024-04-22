using System.ComponentModel.DataAnnotations;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Products
{
    public class EditProductInputModel : ProductInputModel, IValidatableObject
    {
        [Display(Name = HasDiscountDisplay)]
        public bool HasDiscount { get; set; }

        [Display(Name = NewPriceDisplay)]
        public decimal? NewPrice { get; set; }

        [Required]
        [Range(typeof(int), EditProductQuantityMinRange, ProductQuantityMaxRange)]
        public int? Quantity { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!HasDiscount && NewPrice != null)
            {
                yield return new ValidationResult(HasDiscountErrorMessage);
            }
            if (HasDiscount && NewPrice == null)
            {
                yield return new ValidationResult(NewPriceErrorMessage);
            }

            if (NewPrice >= Price)
            {
                yield return new ValidationResult(NewPriceHigherOrEqualToOriginalErrorMessage);
            }
        }
    }
}

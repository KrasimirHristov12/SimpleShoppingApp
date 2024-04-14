using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleShoppingApp.Models.Categories;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Products
{
    public abstract class ProductInputModel
    {
        public ProductInputModel()
        {
            Categories = new List<CategoryViewModel>();
        }
        public int Id { get; set; }

        [Required]
        [StringLength(ProductNameMaxLength, MinimumLength = ProductNameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(ProductDescriptionMinLength)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(typeof(decimal), ProductPriceMinRange, ProductPriceMaxRange, ConvertValueInInvariantCulture = true)]
        public decimal? Price { get; set; }

        [BindNever]
        public IEnumerable<CategoryViewModel> Categories { get; set; }

        [Display(Name = CategoryIdDisplay)]
        public int CategoryId { get; set; }
    }
}

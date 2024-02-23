using Microsoft.AspNetCore.Mvc.ModelBinding;
using SimpleShoppingApp.Models.Categories;

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
        [StringLength(100, MinimumLength = 5)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(10)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(typeof(decimal), "0.50", "10000", ConvertValueInInvariantCulture = true)]
        public decimal? Price { get; set; }

        [Required]
        [Range(typeof(int), "1", "1000000")]
        public int? Quantity { get; set; }

        [BindNever]
        public IEnumerable<CategoryViewModel> Categories { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}

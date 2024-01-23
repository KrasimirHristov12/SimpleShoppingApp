using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.ValidationAttributes;
using SimpleShoppingApp.Web.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Products
{
    public class AddProductInputModel
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string? Name { get; set; }

        [Required]
        [MinLength(10)]
        public string? Description { get; set; }

        [Required]
        [Range(typeof(decimal), "0.50", "10000", ConvertValueInInvariantCulture = true)]
        public decimal? Price { get; set; }

        [Required]
        [Range(typeof(int), "1", "1000000")]
        public int? Quantity { get; set; }

        [Required]
        [ValidateImage]
        [FileMaxSize(5)]
        public IEnumerable<IFormFile>? Images { get; set; }

    }
}

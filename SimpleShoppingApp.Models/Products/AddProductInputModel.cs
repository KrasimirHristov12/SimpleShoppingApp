using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.Models.Categories;
using SimpleShoppingApp.ValidationAttributes;
using SimpleShoppingApp.Web.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Products
{
    public class AddProductInputModel : ProductInputModel
    {
        [Required]
        [ValidateImage]
        [FileMaxSize(5)]
        public IEnumerable<IFormFile>? Images { get; set; }

    }
}

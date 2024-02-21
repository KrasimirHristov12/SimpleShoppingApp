using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.ValidationAttributes;
using SimpleShoppingApp.Web.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Models.Products
{
    public class AddProductInputModel : ProductInputModel
    {
        public AddProductInputModel()
        {
            Images = new List<IFormFile>();
        }
        [Required]
        [ValidateImage]
        [FileMaxSize(5)]
        public IEnumerable<IFormFile> Images { get; set; }

    }
}

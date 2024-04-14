using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using static SimpleShoppingApp.Data.Constants.GlobalConstants;

namespace SimpleShoppingApp.Models.Products
{
    public class AddProductInputModel : ProductInputModel, IValidatableObject
    {
        public AddProductInputModel()
        {
            Images = new List<FormFile>();
            ImagesUrls = new List<string>();
        }

        [Required]
        [Range(typeof(int), ProductQuantityMinRange, ProductQuantityMaxRange)]
        public int? Quantity { get; set; }

        [ValidateImage]
        [FileMaxSize(5)]
        public IEnumerable<IFormFile>? Images { get; set; }

        [Display(Name = ImagesUrlsDisplay)]
        public IEnumerable<string>? ImagesUrls { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Images.Where(i => i != null).Count() == 0 && ImagesUrls.Where(i => !string.IsNullOrEmpty(i)).Count() == 0)
            {
                yield return new ValidationResult(NoImagesErrorMessage);
            }

            else if (ImagesUrls != null)
            {
                string pattern = ImagesUrlsRegex;
                foreach (var url in ImagesUrls)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        bool isValidUrl = Regex.IsMatch(url, pattern);
                        if (!isValidUrl)
                        {
                            yield return new ValidationResult(InvalidImageUrlErrorMessage);
                            break;
                        }
                    }

                }
            }

        }
    }
}

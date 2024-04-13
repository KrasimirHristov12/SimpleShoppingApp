using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
        [Range(typeof(int), "1", "1000000")]
        public int? Quantity { get; set; }

        [ValidateImage]
        [FileMaxSize(5)]
        public IEnumerable<IFormFile>? Images { get; set; }

        [Display(Name = "Images Links")]
        public IEnumerable<string>? ImagesUrls { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Images.Where(i => i != null).Count() == 0 && ImagesUrls.Where(i => !string.IsNullOrEmpty(i)).Count() == 0)
            {
                yield return new ValidationResult("Please provide at least one url or at least one image");
            }

            else if (ImagesUrls != null)
            {
                string pattern = "^((http|https)://)[-a-zA-Z0-9@:%._\\+~#?&//=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%._\\+~#?&//=]*)$";
                foreach (var url in ImagesUrls)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        bool isValidUrl = Regex.IsMatch(url, pattern);
                        if (!isValidUrl)
                        {
                            yield return new ValidationResult("Invalid image url");
                            break;
                        }
                    }

                }
            }

        }
    }
}

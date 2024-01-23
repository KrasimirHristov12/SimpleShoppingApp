using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.ValidationAttributes
{
    public class ValidateImageAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var images = value as IEnumerable<IFormFile>;

            if (images == null)
            {
                return ValidationResult.Success;
            }

            foreach (var image in images)
            {
                try
                {
                    var stream = image.OpenReadStream();

                    var imageInfo = Image.Identify(stream);

                    if (imageInfo == null)
                    {
                        return new ValidationResult("Invalid image format");
                    }
                }

                catch (Exception)
                {

                    return new ValidationResult("Invalid image format");
                }

            }

            return ValidationResult.Success;

        }
    }
}

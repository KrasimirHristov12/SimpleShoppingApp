using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SimpleShoppingApp.Web.ValidationAttributes
{
    public class FileMaxSizeAttribute : ValidationAttribute
    {
        public int MaxSizeInMb { get; set; }

        public FileMaxSizeAttribute(int maxSizeInMb)
        {
            MaxSizeInMb = maxSizeInMb;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var files = value as IEnumerable<IFormFile>;

            if (files == null)
            {
                return ValidationResult.Success;
            }

            foreach (var file in files)
            {
                if (file.Length > MaxSizeInMb * 1000 * 1000)
                {
                    return new ValidationResult($"The allowed maximum size is {MaxSizeInMb} MB per file");
                }
            }

            return ValidationResult.Success;
        }
    }
}

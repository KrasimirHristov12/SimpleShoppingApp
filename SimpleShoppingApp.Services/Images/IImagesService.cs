using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.Data.Enums;

namespace SimpleShoppingApp.Services.Images
{
    public interface IImagesService
    {
        Task AddAsync(IFormFile image, string imageName, ImageType imageType, string wwwrootDirectory);
    }
}

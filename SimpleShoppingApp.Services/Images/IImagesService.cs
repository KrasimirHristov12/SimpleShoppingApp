using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Services.Images
{
    public interface IImagesService
    {
        Task AddAsync(IFormFile image, string imageName, ImageType imageType, string wwwrootDirectory);
        Task<IEnumerable<ImageViewModel>> GetAsync(int entityId, ImageType imageType);
    }
}

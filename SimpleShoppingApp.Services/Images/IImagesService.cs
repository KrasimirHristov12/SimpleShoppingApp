using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Services.Images
{
    public interface IImagesService
    {
        Task AddAsync(string imageUrl, int productId);
        Task AddAsync(IFormFile image, string imageName, string directory, int productId);
        Task<IEnumerable<ImageViewModel>> GetAsync(int productId);
        Task<ImageViewModel?> GetFirstAsync(int productId);
    }
}

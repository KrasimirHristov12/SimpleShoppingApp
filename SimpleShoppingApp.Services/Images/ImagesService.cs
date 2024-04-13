using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Services.Images
{
    public class ImagesService : IImagesService
    {
        private readonly IRepository<Image> imagesRepo;
        public ImagesService(IRepository<Image> _imagesRepo)
        {
            imagesRepo = _imagesRepo;
        }

        public async Task AddAsync(string imageUrl, int productId)
        {
            var imageDb = new Image
            {
                ProductId = productId,
                ImageUrl = imageUrl,
            };

            await imagesRepo.AddAsync(imageDb);
            await imagesRepo.SaveChangesAsync();
        }
        public async Task AddAsync(IFormFile image, string imageName, string directory, int productId)
        {
            string extension = Path.GetExtension(image.FileName);

            var imageDb = new Image
            {
                ProductId = productId,
                Extension = extension,
                Name = imageName,
            };

            await imagesRepo.AddAsync(imageDb);
            await imagesRepo.SaveChangesAsync();

            string path = Path.Combine(directory, "images", "products", imageName + extension);

            using FileStream fs = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(fs);
        }

        public async Task<IEnumerable<ImageViewModel>> GetAsync(int productId)
        {
            if (productId <= 0)
            {
                return new List<ImageViewModel>();
            }

            var images = await imagesRepo
                .AllAsNoTracking()
                .Where(i => i.ProductId == productId && !i.IsDeleted)
                .Select(i => new ImageViewModel
                {
                    Name = i.Name,
                    Extension = i.Extension,
                    ImageUrl = i.ImageUrl,
                })
                .ToListAsync();

            return images;
        }

        public async Task<ImageViewModel?> GetFirstAsync(int productId)
        {
            var images = await GetAsync(productId);

            if (images.Count() == 0)
            {
                return null;
            }

            return images.First();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Images;

namespace SimpleShoppingApp.Services.Images
{
    public class ImagesService : IImagesService
    {
        private readonly IRepository<Image> imagesRepo;

        private readonly IDictionary<ImageType, string> imageTypeDirectoryMap = new Dictionary<ImageType, string>()
        {
            { ImageType.Product, "products" },
        };

        private readonly IDictionary<string, string> imageTypeNameMap = new Dictionary<string, string>()
        {
            { "products", "prod" },
        };

        public ImagesService(IRepository<Image> _imagesRepo)
        {
            imagesRepo = _imagesRepo;
        }
        public async Task AddAsync(IFormFile image, string imageName, ImageType imageType, string wwwrootDirectory)
        {
            string extension = Path.GetExtension(image.FileName);

            var imageDb = new Image
            {
                Extension = extension,
                Name = imageName,
            };

            await imagesRepo.AddAsync(imageDb);
            await imagesRepo.SaveChangesAsync();

            string path = $"{wwwrootDirectory}\\images\\{imageTypeDirectoryMap[imageType]}\\{imageName}.{extension}";


            using FileStream fs = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(fs);
        }

        public async Task<IEnumerable<ImageViewModel>> GetAsync(int entityId, ImageType imageType)
        {
            var entityDirName = imageTypeDirectoryMap[imageType];

            var startImageName = imageTypeNameMap[entityDirName];

            var images = await imagesRepo
                .AllAsNoTracking()
                .Where(i => i.Name.StartsWith($"{startImageName}{entityId}"))
                .Select(i => new ImageViewModel
                {
                    Name = i.Name,
                    Extension = i.Extension,
                })
                .ToListAsync();

            return images;
        }
    }
}

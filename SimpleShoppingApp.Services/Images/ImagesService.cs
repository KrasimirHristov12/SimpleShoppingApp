using Microsoft.AspNetCore.Http;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;

namespace SimpleShoppingApp.Services.Images
{
    public class ImagesService : IImagesService
    {
        private readonly IRepository<Image> imagesRepo;

        public ImagesService(IRepository<Image> _imagesRepo)
        {
            imagesRepo = _imagesRepo;
        }
        public async Task AddAsync(IFormFile image, string imageName, ImageType imageType, string wwwrootDirectory)
        {
            var imageTypeDirectoryMap = new Dictionary<ImageType, string>()
            {
                { ImageType.Product, "products" },
            };


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
    }
}

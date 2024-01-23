using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;
        private readonly IWebHostEnvironment env;
        private readonly IImagesService imagesService;

        public ProductsController(IProductsService _productsService, 
            IWebHostEnvironment _env,
            IImagesService _imagesService)
        {
            productsService = _productsService;
            env = _env;
            imagesService = _imagesService;
        }
        public IActionResult Index(int id)
        {

            return View();
        }

        [Route("/[controller]/[action]/{name}")]
        public IActionResult Category(string name)
        {
            // Get category products - Get count, Implement Paging, filters (like in Emag)

            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int newProductId = await productsService.AddAsync(model);


            foreach (var imageFromModel in model.Images)
            {
                string imageUID = Guid.NewGuid().ToString();

                string imageName = $"prod{newProductId}_{imageUID}";

                string wwwrootDir = env.WebRootPath;

                await imagesService.AddAsync(imageFromModel, imageName, ImageType.Product, wwwrootDir);

            }



            return RedirectToAction(nameof(Index), new { id = newProductId });
           
        }
    }
}

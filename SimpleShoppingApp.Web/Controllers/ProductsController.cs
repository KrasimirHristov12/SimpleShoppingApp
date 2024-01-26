using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Categories;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;
        private readonly IWebHostEnvironment env;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;

        public ProductsController(IProductsService _productsService, 
            IWebHostEnvironment _env,
            IImagesService _imagesService,
            ICategoriesService _categoriesService)
        {
            productsService = _productsService;
            env = _env;
            imagesService = _imagesService;
            categoriesService = _categoriesService;
        }
        public async Task<IActionResult> Index(int id)
        {
            var product = await productsService.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> Category(int id, string name, int page = 1)
        {
            // Get category products - Get count, Implement Paging, filters (like in Emag)

            var productsPerPage = await productsService.GetByCategoryAsync(id, 1, page);

            var model = new CategoryProductsViewModel
            {
                CategoryName = name,
                CategoryId = id,
                ProductsPerPage = productsPerPage,
                CurrentPage = page,
                TotalProductsCount = await categoriesService.GetCountOfProductsAsync(id),
            };

            return View(model);
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                TotalProductsCount = await productsService.GetCountForCategoryAsync(id),
            };

            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            var categories = await categoriesService.GetAllAsync();
            var model = new AddProductInputModel
            {
                Categories = categories,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await categoriesService.GetAllAsync();
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

        [HttpPost]

        public async Task<IActionResult> Delete(int id)
        {
            bool deleteResult = await productsService.DeleteAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> Update(int id)
        {
            var productToEdit = await productsService.GetToEditAsync(id);

            if (productToEdit == null)
            {
                return NotFound();
            }

            return View(productToEdit);
        }

        [HttpPost]

        public async Task<IActionResult> Update(EditProductInputModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await categoriesService.GetAllAsync();
                return View(model);
            }

            await productsService.UpdateAsync(model);

            return RedirectToAction(nameof(Index), new { id = model.Id });
        }
    }
}

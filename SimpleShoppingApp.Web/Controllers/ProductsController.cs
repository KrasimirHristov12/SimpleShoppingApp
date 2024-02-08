using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Categories;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;
using System.Security.Claims;

namespace SimpleShoppingApp.Web.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductsService productsService;
        private readonly IWebHostEnvironment env;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly IUsersService usersService;

        public ProductsController(IProductsService _productsService, 
            IWebHostEnvironment _env,
            IImagesService _imagesService,
            ICategoriesService _categoriesService,
            IUsersService _usersService)
        {
            productsService = _productsService;
            env = _env;
            imagesService = _imagesService;
            categoriesService = _categoriesService;
            usersService = _usersService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int id)
        {
            var product = await productsService.GetAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated)
            {
                string loggedInUserId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

                product.BelongsToCurrentUser = await productsService.BelognsToUserAsync(id, loggedInUserId);
            }

            else
            {
                product.BelongsToCurrentUser = false;
            }

            

            return View(product);
        }

        [AllowAnonymous]
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

            string loggedInUserId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!await productsService.BelognsToUserAsync(id, loggedInUserId))
            {
                return Forbid();
            }

            bool deleteResult = await productsService.DeleteAsync(id);

            if (!deleteResult)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> Update(int id)
        {
            string loggedInUserId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!await productsService.BelognsToUserAsync(id, loggedInUserId))
            {
                return Forbid();
            }
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
            string loggedInUserId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (!await productsService.BelognsToUserAsync(model.Id, loggedInUserId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await categoriesService.GetAllAsync();
                return View(model);
            }

            await productsService.UpdateAsync(model);

            return RedirectToAction(nameof(Index), new { id = model.Id });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) // If JS turned off
            {
                return BadRequest();
            }

            ViewBag.Name = name;

            var model = await productsService.GetByNameAsync(name);

            return View(model);
        }
    }
}

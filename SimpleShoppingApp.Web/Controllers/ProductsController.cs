using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Models.Categories;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Notifications;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;
using SimpleShoppingApp.Web.Hubs;

namespace SimpleShoppingApp.Web.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductsService productsService;
        private readonly IWebHostEnvironment env;
        private readonly IImagesService imagesService;
        private readonly ICategoriesService categoriesService;
        private readonly IUsersService usersService;
        private readonly INotificationsService notificationsService;
        private readonly IHubContext<NotificationsHub> notificationsHub;

        public ProductsController(IProductsService _productsService, 
            IWebHostEnvironment _env,
            IImagesService _imagesService,
            ICategoriesService _categoriesService,
            IUsersService _usersService,
            INotificationsService _notificationsService,
            IHubContext<NotificationsHub> _notificationsHub)
        {
            productsService = _productsService;
            env = _env;
            imagesService = _imagesService;
            categoriesService = _categoriesService;
            usersService = _usersService;
            notificationsService = _notificationsService;
            notificationsHub = _notificationsHub;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int id)
        {
            string? loggedInUserId = User.Identity != null && User.Identity.IsAuthenticated ? User.GetId() : null;
            var product = await productsService.GetAsync(id, loggedInUserId);

            if (product == null)
            {
                return NotFound();
            }       

            return View(product);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Category(int id, string name, int page = 1)
        {
            var filterModel = new ProductsFilterModel
            {
                CategoryId = id,
                Page = page,
                ProductsPerPage = 12,
            };
            var productsPerPage = await productsService.GetByCategoryAsync(filterModel);


            if (productsPerPage == null || productsPerPage.Products.Count() == 0)
            {
                return NotFound();
            }

            var model = new CategoryProductsViewModel
            {
                CategoryName = name,
                CategoryId = id,
                ProductsPerPage = productsPerPage.Products,
                CurrentPage = page,
                TotalProductsCount = await productsService.GetCountForCategoryAsync(id),
            };

            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            var categories = await categoriesService.GetAllAsync();

            if (categories.Count() == 0)
            {
                return NotFound();
            }

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
                var categories = await categoriesService.GetAllAsync();
                if (categories.Count() == 0)
                {
                    return NotFound();
                }
                model.Categories = categories;
                return View(model);
            }

            var userId = User.GetId();

            var addProductResult = await productsService.AddAsync(model, userId, env.WebRootPath);

            if (addProductResult.Result == AddUpdateDeleteResult.NotFound)
            {
                return NotFound();
            }

            bool isApproved = addProductResult.IsApproved;

            var productId = addProductResult.ProductId;

            if (!isApproved)
            {
                string? adminUserId = await usersService.GetAdminIdAsync();
                if (!string.IsNullOrWhiteSpace(adminUserId))
                {
                    string notificationText = "A new product was added for approval.";
                    string productLink = $"/Products/Index/{productId}";
                    int notificationId = await notificationsService.AddAsync(userId, adminUserId, notificationText, productLink);
                    if (notificationId > 0)
                    {
                        await notificationsHub.Clients.User(adminUserId).SendAsync("ReceiveNotification", notificationText, productLink, notificationId);
                    }

                }

                TempData["SuccessfullyAdded"] = "Product successfully added and is sent for approval to the administrator! We will get back to you soon.";

                return RedirectToAction(nameof(Index), "Home");
            }

            TempData["SuccessfullyAddedByAdmin"] = "Product successfully added!";

            return RedirectToAction(nameof(Index), new { id = productId });

           
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            string loggedInUserId = User.GetId();

            var deleteResult = await productsService.DeleteAsync(id, loggedInUserId);

            if (deleteResult == AddUpdateDeleteResult.NotFound)
            {
                return NotFound();
            }

            if (deleteResult == AddUpdateDeleteResult.Forbidden)
            {
                return Forbid();
            }

            TempData["ProductDeleted"] = "You have successfully deleted the product";

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Update(int id)
        {
            string loggedInUserId = User.GetId();

            var productToEdit = await productsService.GetToEditAsync(id, loggedInUserId);

            if (productToEdit.Result == AddUpdateDeleteResult.NotFound)
            {
                return NotFound();
            }

            if (productToEdit.Result == AddUpdateDeleteResult.Forbidden)
            {
                return Forbid();
            }

            if (productToEdit.Model == null)
            {
                return NotFound();
            }

            if (productToEdit.Model.Categories.Count() == 0)
            {
                return NotFound();
            }

            return View(productToEdit.Model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(EditProductInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await categoriesService.GetAllAsync();
                if (categories.Count() == 0)
                {
                    return NotFound();
                }
                model.Categories = categories;
                return View(model);
            }

            string loggedInUserId = User.GetId();

            var result =  await productsService.UpdateAsync(model, loggedInUserId);

            if (result == AddUpdateDeleteResult.NotFound)
            {
                return NotFound();
            }

            if (result == AddUpdateDeleteResult.Forbidden)
            {
                return Forbid();
            }

            TempData["ProductUpdated"] = "You have successfully updated this product.";

            return RedirectToAction(nameof(Index), new { id = model.Id });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest();
            }

            ViewBag.Name = name;

            var model = await productsService.GetByNameAsync(name);

            return View(model);
        }

    }
}

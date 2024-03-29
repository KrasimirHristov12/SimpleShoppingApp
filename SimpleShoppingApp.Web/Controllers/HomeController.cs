using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models;
using SimpleShoppingApp.Models.Index;
using SimpleShoppingApp.Services.Categories;
using SimpleShoppingApp.Services.Products;
using System.Diagnostics;

namespace SimpleShoppingApp.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductsService productsService;
        private readonly ICategoriesService categoriesService;

        public HomeController(IProductsService _productsService,
            ICategoriesService _categoriesService)
        {
            productsService = _productsService;
            categoriesService = _categoriesService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel();

            model.TotalProductsCount = await productsService.GetCountAsync();

            model.TotalCategoriesCount = await categoriesService.GetCountAsync();

            model.RandomProducts = await productsService.GetRandomProductsAsync(12);

            return View(model);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
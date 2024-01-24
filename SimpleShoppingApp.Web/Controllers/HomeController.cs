using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models;
using SimpleShoppingApp.Models.Index;
using SimpleShoppingApp.Services.Products;
using System.Diagnostics;

namespace SimpleShoppingApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductsService productsService;

        public HomeController(ILogger<HomeController> logger, IProductsService _productsService)
        {
            _logger = logger;
            productsService = _productsService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel();

            model.RandomProducts = await productsService.GetRandomProductsAsync(9);

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
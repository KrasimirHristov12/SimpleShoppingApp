using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService _productsService)
        {
            productsService = _productsService;
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


            return RedirectToAction(nameof(Index), new { id = newProductId });
           
        }
    }
}

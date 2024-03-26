using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Web.Areas.Administration.Controllers
{
    public class ProductsController : AdministrationBaseController
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService _productsService)
        {
            productsService = _productsService;
        }
        public async Task<IActionResult> Approve(int id)
        {
            var product = await productsService.GetProductToApproveAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

    }
}

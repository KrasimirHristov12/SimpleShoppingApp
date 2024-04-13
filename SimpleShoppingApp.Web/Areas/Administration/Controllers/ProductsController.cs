using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Extensions;
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

        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var adminUserId = User.GetId();
            var approveResult = await productsService.ApproveAsync(id, adminUserId);
            if (!approveResult)
            {
                return NotFound();
            }

            TempData["ProductApproved"] = "You have successfully approved this product.";

            return RedirectToAction("Index", "Products", new { id, area = "" });
        }

        [HttpPost]
        public async Task<IActionResult> UnApprove(int id)
        {
            var adminUserId = User.GetId();
            var approveResult = await productsService.UnApproveAsync(id, adminUserId);
            if (!approveResult)
            {
                return NotFound();
            }

            TempData["ProductUnApproved"] = "You have not approved the product.";

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}

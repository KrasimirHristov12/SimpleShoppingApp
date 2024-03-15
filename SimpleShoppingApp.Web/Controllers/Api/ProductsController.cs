using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models.Products;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Web.Controllers.Api
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService _productsService)
        {
            productsService = _productsService;
        }

        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<ProductsPerPageModel>> GetProductsPerPage([FromQuery]ProductsFilterModel model)
        {
            var products = await productsService.GetByCategoryAsync(model);

            return Ok(products);
        }
    }
}

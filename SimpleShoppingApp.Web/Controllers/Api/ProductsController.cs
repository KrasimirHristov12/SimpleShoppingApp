using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Models.Categories;
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
        public async Task<ActionResult<IEnumerable<ListProductsViewModel>>> GetFilteredProducts([FromQuery]ProductsFilterModel model)
        {
           var products = await productsService.GetFilteredProductsAsync(model);

            return Ok(products);

        }

        [AllowAnonymous]
        [Route("[action]")]
        public async Task<ActionResult<ProductsPerPageModel>> GetProductsPerPage(int currentPage, int elementsPerPage, int categoryId)
        {
            var productsPerPage = await productsService.GetByCategoryAsync(categoryId, 1, currentPage);

            if (productsPerPage.Count() == 0)
            {
                return NotFound();
            }
            var totalProductsForCategory = await productsService.GetCountForCategoryAsync(categoryId);

            var model = new ProductsPerPageModel
            {
                TotalPages = (int)Math.Ceiling((decimal)totalProductsForCategory / elementsPerPage),
                Products = productsPerPage,
            };

            return Ok(model);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Extensions;
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

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<double>> AddUpdateRating([FromForm]AddUpdateProductRatingJsonModel model)
        {
            string loggedInUserId = User.GetId();
            var ratingModel = await productsService.AddRatingFromUserAsync(model.ProductId, loggedInUserId, model.Rating);
            if (ratingModel.Result == AddUpdateDeleteResult.NotFound)
            {
                return NotFound();
            }
            return Ok(ratingModel.AvgRating);
        }
    }
}

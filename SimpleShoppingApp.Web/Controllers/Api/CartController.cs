using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Models.Carts;
using SimpleShoppingApp.Services.Carts;

namespace SimpleShoppingApp.Web.Controllers.Api
{
    public class CartController : BaseApiController
    {
        private readonly ICartsService cartService;

        public CartController(ICartsService _cartService)
        {
            cartService = _cartService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<UpdateQuantityJsonViewModel>> UpdateQuantity([FromForm]UpdateQuantityCartJsonModel model)
        {
            var userId = User.GetId();
            var cartId = await cartService.GetIdAsync(userId);

            var updatedInfo = await cartService.UpdateQuantityInCartAsync(cartId, model.ProductId, model.UpdatedQuantity, userId);

            if (updatedInfo == null)
            {
                return NotFound();
            }

            return Ok(updatedInfo.Model);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<CartJsonViewModel>> DeleteProduct([FromForm]int productId)
        {
            string userId = User.GetId();

            int cartId = await cartService.GetIdAsync(userId);

            var removedProductInfo = await cartService.RemoveProductAsync(cartId, productId, userId);

            if (removedProductInfo.Result == AddUpdateDeleteResult.NotFound)
            {
                return NotFound();
            }

            if (removedProductInfo.Result == AddUpdateDeleteResult.Forbidden)
            {
                return Forbid();
            }

            return Ok(removedProductInfo.Model);
        }
    }
}

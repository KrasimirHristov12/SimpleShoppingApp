using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Web.Controllers
{
    public class CartController : BaseController
    {
        private readonly ICartsService cartService;
        private readonly IUsersService usersService;
        private readonly IProductsService productsService;
        private readonly IAddressesService addressesService;

        public CartController(ICartsService _cartService, 
            IUsersService _usersService, 
            IProductsService _productsService,
            IAddressesService _addressesService)
        {
            cartService = _cartService;
            usersService = _usersService;
            productsService = _productsService;
            addressesService = _addressesService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = usersService.GetId(User);
            var cart = await cartService.GetAsync(userId);

            if (cart == null)
            {
                return NotFound();
            }

            cart.Input = new MakeOrderInputModel();

            cart.Input.PhoneNumber = await usersService.GetPhoneNumberAsync(userId);

            cart.Input.Addresses = await addressesService.GetAllForUserAsync(userId);

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var userId = usersService.GetId(User);
            var cartId = await cartService.GetIdAsync(userId);
            var addResult = await cartService.AddProductAsync(cartId, id, userId);
            if (addResult == AddUpdateProductToCartResult.NotFound)
            {
                return NotFound();
            }

            if (addResult == AddUpdateProductToCartResult.Forbidden)
            {
                return Forbid();
            }

            if (addResult == AddUpdateProductToCartResult.NotInStock)
            {
                TempData["NotInStock"] = "You can't add this product to your cart because it's currently not in stock.";
                return RedirectToAction("Index", "Products", new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            string userId = usersService.GetId(User);

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

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int updatedQuantity)
        {
            var userId = usersService.GetId(User);
            var cartId = await cartService.GetIdAsync(userId);

            var updatedInfo = await cartService.UpdateQuantityInCartAsync(cartId, productId, updatedQuantity, userId);

            if (updatedInfo == null)
            {
                return NotFound();
            }

            return Json(updatedInfo.Model);

        }
    }
}

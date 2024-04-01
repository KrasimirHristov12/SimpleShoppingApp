using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Orders;
using SimpleShoppingApp.Services.Addresses;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;
using SimpleShoppingApp.Extensions;

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
            var userId = User.GetId();
            var cart = await cartService.GetAsync(userId);

            if (cart == null)
            {
                return NotFound();
            }

            cart.Input = new MakeOrderInputModel();

            cart.Input.PhoneNumber = await usersService.GetPhoneNumberAsync(userId) ?? string.Empty;

            cart.Input.Addresses = await addressesService.GetAllForUserAsync(userId);

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var userId = User.GetId();
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



    }
}

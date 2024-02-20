using Microsoft.AspNetCore.Mvc;
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
            if (id <= 0 || !await productsService.DoesProductExistAsync(id))
            {
                return NotFound();
            }
            var userId = usersService.GetId(User);
            var cartId = await cartService.GetIdAsync(userId);
            if (cartId == null)
            {
                return NotFound();
            }
            await cartService.AddProductAsync((int)cartId, id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            string userId = usersService.GetId(User);

            int? cartId = await cartService.GetIdAsync(userId);

            if (cartId == null)
            {
                return NotFound();
            }
            var removedProductInfo = await cartService.RemoveProductAsync((int)cartId, productId);

            if (removedProductInfo == null)
            {
                return NotFound();
            }
            
            return Ok(removedProductInfo);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int updatedQuantity)
        {
            var userId = usersService.GetId(User);
            var cartId = await cartService.GetIdAsync(userId);

            if (cartId == null)
            {
                return NotFound();
            }

            var updatedInfo = await cartService.UpdateProductQuantityAsync((int)cartId, productId, updatedQuantity);

            if (updatedInfo == null)
            {
                return NotFound();
            }

            return Ok(updatedInfo);

        }
    }
}

using Microsoft.AspNetCore.Mvc;
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

        public CartController(ICartsService _cartService, IUsersService _usersService, IProductsService _productsService)
        {
            cartService = _cartService;
            usersService = _usersService;
            productsService = _productsService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = usersService.GetId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
            if (!await cartService.DoesUserHaveCartAsync(userId))
            {
                await cartService.AddAsync(userId);
            }

            var cart = await cartService.GetAsync(userId);

            if (cart == null)
            {
                return NotFound();
            }

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
            if (userId == null)
            {
                return Unauthorized();
            }
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
            string? userId = usersService.GetId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

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
            if (User == null)
            {
                return Unauthorized();
            }
            var userId = usersService.GetId(User);
            if (userId == null)
            {
                return Unauthorized();
            }
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

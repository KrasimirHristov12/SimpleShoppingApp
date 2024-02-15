using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Carts;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Products;

namespace SimpleShoppingApp.Services.Carts
{
    public class CartsService : ICartsService
    {
        private readonly IRepository<ShoppingCart> cartsRepo;
        private readonly IRepository<CartsProducts> cartsProductsRepo;
        private readonly IProductsService productsService;
        private readonly IImagesService imagesService;

        public CartsService(IRepository<ShoppingCart> _cartsRepo,
            IRepository<CartsProducts> _cartsProductsRepo,
            IProductsService _productsService,
            IImagesService _imagesService)
        {
            cartsRepo = _cartsRepo;
            cartsProductsRepo = _cartsProductsRepo;
            productsService = _productsService;
            imagesService = _imagesService;
        }
        public async Task AddAsync(string userId)
        {
            var cart = new ShoppingCart()
            {
                UserId = userId,
            };

            await cartsRepo.AddAsync(cart);
            await cartsRepo.SaveChangesAsync();

        }

        public async Task<bool> DoesUserHaveCartAsync(string userId)
        {
            return await cartsRepo.AllAsNoTracking().AnyAsync(c => c.UserId == userId);
        }
        public async Task AddProductAsync(int cartId, int productId)
        {
            var foundCartProduct = await cartsProductsRepo.AllAsTracking().FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);
            if (foundCartProduct == null)
            {
                await cartsProductsRepo.AddAsync(new CartsProducts
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = 1,
                });
                await cartsProductsRepo.SaveChangesAsync();
                return;
            }

            if (foundCartProduct.IsDeleted)
            {
                foundCartProduct.IsDeleted = false;
                foundCartProduct.Quantity = 1;
                await cartsProductsRepo.SaveChangesAsync();
            }

        }

        public async Task<int?> UpdateQuantityInCartAsync(int cartId, int productId, QuantityOperation operation)
        {
            var productCart = await cartsProductsRepo.AllAsTracking().FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);

            int? productsQuantityAvailable = await productsService.GetQuantityAsync(productId);

            if (productCart == null)
            {
                return null;
            }

            if (productsQuantityAvailable == null)
            {
                return null;
            }

            if (operation == QuantityOperation.Decrease)
            {
                if (productCart.Quantity > 1 && productCart.Quantity <= productsQuantityAvailable)
                {
                    productCart.Quantity--;
                }
                else
                {
                    productCart.Quantity = (int)productsQuantityAvailable;
                }
            }
            else if (operation == QuantityOperation.Increase)
            {
                if (productCart.Quantity < productsQuantityAvailable)
                {
                    productCart.Quantity++;
                }
                else
                {
                    productCart.Quantity = (int)productsQuantityAvailable;
                }
            }

            await cartsProductsRepo.SaveChangesAsync();
            return productCart.Quantity;

        }

        public async Task<CartViewModel?> GetAsync(string userId)
        {
            var cart = await cartsRepo.AllAsNoTracking()
                .Where(c => c.UserId == userId)
                .Select(c => new CartViewModel
                {
                    CartProducts = c.CartsProducts.Where(cp => !cp.IsDeleted).Select(cp => new CartProductsViewModel
                    {
                        ProductId = cp.ProductId,
                        ProductName = cp.Product.Name,
                        ProductPrice = cp.Product.Price,
                        ProductQuantity = cp.Quantity,
                    })
                }).FirstOrDefaultAsync();
            if (cart == null)
            {
                return null;
            }

            foreach (var product in cart.CartProducts)
            {
                product.Image = await imagesService.GetFirstAsync(product.ProductId, ImageType.Product);
            }

            return cart;
        }
        public async Task<int?> GetIdAsync(string userId)
        {
            return await cartsRepo.AllAsNoTracking()
                .Where(c => c.UserId == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<CartJsonViewModel?> RemoveProductAsync(int cartId, int productId)
        {
            var cartProduct = await cartsProductsRepo.AllAsTracking()
                .Where(cp => cp.CartId == cartId && cp.ProductId == productId)
                .FirstOrDefaultAsync();

            if (cartProduct == null)
            {
                return null;
            }

            if (!cartProduct.IsDeleted)
            {
                cartProduct.IsDeleted = true;
                await cartsProductsRepo.SaveChangesAsync();
                var cartUpdatedInfo = await cartsProductsRepo.AllAsTracking()
                .Where(cp => cp.CartId == cartId && !cp.IsDeleted)
                .Select(cp => cp.Product.Price * cp.Quantity)
                .ToListAsync();

                return new CartJsonViewModel
                {
                    ProductId = productId,
                    NewCount = cartUpdatedInfo.Count,
                    NewTotalPrice = cartUpdatedInfo.Sum(),
                };
            }

            return null;
        }

        public async Task RemoveAllProductsAsync(int cartId)
        {
            var cartProduct = await cartsProductsRepo.AllAsTracking()
                .Where(cp => cp.CartId == cartId)
                .ToListAsync();
            foreach (var cp in cartProduct)
            {
                if (!cp.IsDeleted)
                {
                    cp.IsDeleted = true;
                }
            }

            await cartsProductsRepo.SaveChangesAsync();
                
        }

        public async Task<UpdateQuantityJsonViewModel?> UpdateProductQuantityAsync(int cartId, int productId, int updatedQuantity)
        {
            var cartProduct = await cartsProductsRepo.AllAsTracking()
                .Include(cp => cp.Product)
                .FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);
            if (cartProduct == null)
            {
                return null;
            }

            if (updatedQuantity == 0)
            {
                return null;
            }

            cartProduct.Quantity = updatedQuantity;
            await cartsProductsRepo.SaveChangesAsync();

            var cartUpdatedInfo = await cartsProductsRepo.AllAsTracking()
                .Where(cp => cp.CartId == cartId && !cp.IsDeleted)
                .Select(cp => cp.Product.Price * cp.Quantity)
                .ToListAsync();
            return new UpdateQuantityJsonViewModel
            {
                NewProductPrice = updatedQuantity * cartProduct.Product.Price,
                NewTotalPrice =  cartUpdatedInfo.Sum(),
            };



        }
    }
}

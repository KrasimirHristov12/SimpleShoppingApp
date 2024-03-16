using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Carts;
using SimpleShoppingApp.Services.Images;
using SimpleShoppingApp.Services.Products;
using SimpleShoppingApp.Services.Users;

namespace SimpleShoppingApp.Services.Carts
{
    public class CartsService : ICartsService
    {
        private readonly IRepository<ShoppingCart> cartsRepo;
        private readonly IRepository<CartsProducts> cartsProductsRepo;
        private readonly IProductsService productsService;
        private readonly IImagesService imagesService;
        private readonly IUsersService usersService;

        public CartsService(IRepository<ShoppingCart> _cartsRepo,
            IRepository<CartsProducts> _cartsProductsRepo,
            IProductsService _productsService,
            IImagesService _imagesService,
            IUsersService _usersService)
        {
            cartsRepo = _cartsRepo;
            cartsProductsRepo = _cartsProductsRepo;
            productsService = _productsService;
            imagesService = _imagesService;
            usersService = _usersService;
        }
        public async Task<int> AddAsync(string userId)
        {
            var cart = new ShoppingCart()
            {
                UserId = userId,
            };

            await cartsRepo.AddAsync(cart);
            await cartsRepo.SaveChangesAsync();
            return cart.Id;

        }

        public async Task<bool> CartExistsAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            return await cartsRepo.
                AllAsNoTracking()
               .AnyAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<AddUpdateProductToCartResult> AddProductAsync(int cartId, int productId, string currentUserId)
        {
            if (cartId <= 0 || productId <= 0)
            {
                return AddUpdateProductToCartResult.NotFound;
            }

            if (!await productsService.DoesProductExistAsync(productId))
            {
                return AddUpdateProductToCartResult.NotFound;
            }

            var ownerUserId = await GetUserIdAsync(cartId);

            if (ownerUserId == null)
            {
                return AddUpdateProductToCartResult.NotFound;
            }

            if (!await BelongsToUserAsync(ownerUserId, currentUserId))
            {
                return AddUpdateProductToCartResult.Forbidden;
            }

            var foundCartProduct = await cartsProductsRepo
                .AllAsTracking()
                .FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId);

            if (foundCartProduct != null && !foundCartProduct.IsDeleted)
            {
                return AddUpdateProductToCartResult.AlreadyExist;
            }

            else if (foundCartProduct != null && foundCartProduct.IsDeleted)
            {
                var productQuantity = await productsService.GetQuantityAsync(productId);
                if (productQuantity == null)
                {
                    return AddUpdateProductToCartResult.NotFound;
                }
                if (productQuantity == 0)
                {
                    return AddUpdateProductToCartResult.NotInStock;
                }

                foundCartProduct.IsDeleted = false;
                foundCartProduct.Quantity = 1;
                await cartsProductsRepo.SaveChangesAsync();
            }

            if (foundCartProduct == null)
            {
                var productQuantity = await productsService.GetQuantityAsync(productId);
                if (productQuantity == null)
                {
                    return AddUpdateProductToCartResult.NotFound;
                }
                if (productQuantity == 0)
                {
                    return AddUpdateProductToCartResult.NotInStock;
                }

                await cartsProductsRepo.AddAsync(new CartsProducts
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = 1,
                });
                await cartsProductsRepo.SaveChangesAsync();
            }

            return AddUpdateProductToCartResult.Success;

        }

        public async Task<CartViewModel?> GetAsync(string currentUserId)
        {
            var cart = await cartsRepo.AllAsNoTracking()
                .Where(c => c.UserId == currentUserId && !c.IsDeleted)
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
                var image = await imagesService.GetFirstAsync(product.ProductId);
                if (image != null)
                {
                    product.Image = image;
                }
            }

            return cart;
        }
        public async Task<int> GetIdAsync(string userId)
        {
            return await cartsRepo.AllAsNoTracking()
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .Select(c => c.Id)
                .FirstAsync();
        }

        public async Task<RemoveProductFromCartModel> RemoveProductAsync(int cartId, int productId, string currentUserId)
        {
            if (cartId <= 0 || productId <= 0)
            {
                return new RemoveProductFromCartModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }
            var cartProduct = await cartsProductsRepo.AllAsTracking()
            .Where(cp => cp.CartId == cartId && cp.ProductId == productId && !cp.IsDeleted)
            .FirstOrDefaultAsync();

            if (cartProduct == null)
            {
                return new RemoveProductFromCartModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            var ownerUserId = await GetUserIdAsync(cartId);

            if (ownerUserId == null) 
            {
                return new RemoveProductFromCartModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            if (!await BelongsToUserAsync(ownerUserId, currentUserId))
            {
                return new RemoveProductFromCartModel
                {
                    Result = AddUpdateDeleteResult.Forbidden,
                    Model = null,
                };
            }

            cartProduct.IsDeleted = true;
            await cartsProductsRepo.SaveChangesAsync();
            var cartUpdatedInfo = await cartsProductsRepo.AllAsTracking()
            .Where(cp => cp.CartId == cartId && !cp.IsDeleted)
            .Select(cp => cp.Product.Price * cp.Quantity)
            .ToListAsync();

            return new RemoveProductFromCartModel
            {
                Result = AddUpdateDeleteResult.Success,
                Model = new CartJsonViewModel
                {
                    ProductId = productId,
                    NewCount = cartUpdatedInfo.Count,
                    NewTotalPrice = cartUpdatedInfo.Sum(),
                },
            };
        }

        public async Task<AddUpdateDeleteResult> RemoveAllProductsAsync(int cartId, string currentUserId)
        {
            if (cartId <= 0)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            var cartProduct = await cartsProductsRepo
                .AllAsTracking()
                .Where(cp => cp.CartId == cartId && !cp.IsDeleted)
                .ToListAsync();

            if (cartProduct.Count == 0)
            {
                return AddUpdateDeleteResult.NotFound;
            }
            var ownerUserId = await GetUserIdAsync(cartId);

            if (ownerUserId == null)
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (!await BelongsToUserAsync(ownerUserId, currentUserId))
            {
                return AddUpdateDeleteResult.Forbidden;
            }

            foreach (var cp in cartProduct)
            {
                cp.IsDeleted = true;
            }

            await cartsProductsRepo.SaveChangesAsync();

            return AddUpdateDeleteResult.Success;

        }

        public async Task<UpdateQuantityModel> UpdateQuantityInCartAsync(int cartId, int productId, int updatedQuantity, string currentUserId)
        {
            if (cartId <= 0 || productId <= 0)
            {
                return new UpdateQuantityModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            var productCart = await cartsProductsRepo
                .AllAsTracking()
                .Include(cp => cp.Product)
                .FirstOrDefaultAsync(cp => cp.CartId == cartId && cp.ProductId == productId && !cp.IsDeleted);

            if (productCart == null)
            {
                return new UpdateQuantityModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            var ownerUserId = await GetUserIdAsync(cartId);

            if (ownerUserId == null)
            {
                return new UpdateQuantityModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            if (!await BelongsToUserAsync(ownerUserId, currentUserId))
            {
                return new UpdateQuantityModel
                {
                    Result = AddUpdateDeleteResult.Forbidden,
                    Model = null,
                };
            }

            int? productQuantity = await productsService.GetQuantityAsync(productId);

            if (productQuantity == null)
            {
                return new UpdateQuantityModel
                {
                    Result = AddUpdateDeleteResult.NotFound,
                    Model = null,
                };
            }

            bool isThereLessThanRequested = false;

            if (productQuantity == 0)
            {
                updatedQuantity = 0;
            }
            else if (updatedQuantity > productQuantity)
            {
                updatedQuantity = (int)productQuantity;
                isThereLessThanRequested = true;
            }
            else if (updatedQuantity <= 0)
            {
                updatedQuantity = 1;
            }

            productCart.Quantity = updatedQuantity;

            await cartsProductsRepo.SaveChangesAsync();

            var cartUpdatedInfo = await cartsProductsRepo
                .AllAsTracking()
                .Where(cp => cp.CartId == cartId && !cp.IsDeleted)
                .Select(cp => cp.Product.Price * cp.Quantity)
                .ToListAsync();

            return new UpdateQuantityModel
            {
                Result = AddUpdateDeleteResult.Success,
                Model = new UpdateQuantityJsonViewModel
                {
                    UpdatedQuantity = updatedQuantity,
                    NewProductPrice = productCart.Quantity * productCart.Product.Price,
                    NewTotalPrice = cartUpdatedInfo.Sum(),
                    IsThereLessThanRequested = isThereLessThanRequested
                },
            };

        }

        public async Task<string?> GetUserIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await cartsRepo.
                AllAsNoTracking()
                .Where(c => c.Id == id && !c.IsDeleted)
                .Select(c => c.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> BelongsToUserAsync(string ownerUserId, string currentUserId)
        {
            if (ownerUserId == currentUserId)
            {
                return true;
            }

            var adminUserId = await usersService.GetAdminIdAsync();

            if (adminUserId == null)
            {
                return false;
            }

            if (currentUserId == adminUserId)
            {
                return true;
            }

            return false;

        }

    }
}

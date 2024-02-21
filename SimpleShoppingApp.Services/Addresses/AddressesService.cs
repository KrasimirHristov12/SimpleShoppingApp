using Microsoft.EntityFrameworkCore;
using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Addresses;

namespace SimpleShoppingApp.Services.Addresses
{
    public class AddressesService : IAddressesService
    {
        private readonly IRepository<ShippingAddress> addressesRepo;

        public AddressesService(IRepository<ShippingAddress> _addressesRepo)
        {
            addressesRepo = _addressesRepo;
        }
        public async Task<AddressViewModel> AddAsync(string name, string userId)
        {
            var address = new ShippingAddress 
            {
                Name = name, 
                UserId = userId, 
            };

            await addressesRepo.AddAsync(address);
            await addressesRepo.SaveChangesAsync();

            return await GetByIdAsync(address.Id);
        }

        public async Task<AddUpdateDeleteResult> DeleteAsync(int id, string currentUserId)
        {
            var foundAddress = await addressesRepo
                .AllAsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (foundAddress == null) 
            {
                return AddUpdateDeleteResult.NotFound;
            }

            if (foundAddress.UserId != currentUserId)
            {
                return AddUpdateDeleteResult.Forbidden;
            }

            foundAddress.IsDeleted = true;
            await addressesRepo.SaveChangesAsync();
            return AddUpdateDeleteResult.Success;
        }

        public async Task<IEnumerable<AddressViewModel>> GetAllForUserAsync(string userId)
        {
            return await addressesRepo
                .AllAsNoTracking()
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .Select(a => new AddressViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                })
                .ToListAsync();
        }

        public async Task<AddressViewModel> GetByIdAsync(int id)
        {
            return await addressesRepo
                .AllAsNoTracking()
                .Where(a => a.Id == id && !a.IsDeleted)
                .Select(a => new AddressViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                })
                .FirstAsync();

        }
    }
}

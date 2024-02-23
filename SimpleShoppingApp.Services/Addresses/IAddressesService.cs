using SimpleShoppingApp.Data.Enums;
using SimpleShoppingApp.Models.Addresses;

namespace SimpleShoppingApp.Services.Addresses
{
    public interface IAddressesService
    {
       Task<AddressViewModel?> AddAsync(string name, string userId);
       Task<AddUpdateDeleteResult> DeleteAsync(int id, string currentUserId);
       Task<IEnumerable<AddressViewModel>> GetAllForUserAsync(string userId);
       Task<AddressViewModel?> GetByIdAsync(int id);
        Task<bool> DoesAddressExistAsync(int id);
    }
}

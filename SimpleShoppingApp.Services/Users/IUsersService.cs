using SimpleShoppingApp.Models.Users;

namespace SimpleShoppingApp.Services.Users
{
    public interface IUsersService
    {
        Task<string?> GetAdminIdAsync();
        Task<string?> GetPhoneNumberAsync(string userId);
        Task<string?> GetEmailAsync(string userId);
        Task<string?> GetFullNameAsync(string userId);
        Task<UserInfoViewModel?> GetUserInfoAsync(string userId);
        Task<bool> UpdateFullNameAsync(EditFullNameInputModel model, string userId);

        Task<bool> UpdatePhoneNumberAsync(EditPhoneNumberInputModel model, string userId);

        Task<IEnumerable<AdministrationUserViewModel>> GetUsersPerPageAsync(int page, int usersPerPage);

        Task<int> GetUsersCountAsync();

        Task<AdministrationUserViewModel?> GetEditUserAsync(string userId);

        Task<bool> EditUserAsync(AdministrationUserViewModel model);

        Task<bool> IsInRoleAsync(string userId, string roleName);

        Task<bool> DoesUserExistAsync(string userId);
    }
}

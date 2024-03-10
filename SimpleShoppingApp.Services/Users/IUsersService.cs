namespace SimpleShoppingApp.Services.Users
{
    public interface IUsersService
    {
        Task<string?> GetAdminIdAsync();
        Task<string?> GetPhoneNumberAsync(string userId);
        Task<string?> GetEmailAsync(string userId);

        Task<string?> GetFullNameAsync(string userId);
    }
}

namespace SimpleShoppingApp.Services.Users
{
    public interface IUsersService
    {
        Task<string?> GetAdminIdAsync();
        Task<string> GetPhoneNumberAsync(string userId);
    }
}

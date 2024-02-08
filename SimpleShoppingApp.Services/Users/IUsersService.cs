namespace SimpleShoppingApp.Services.Users
{
    public interface IUsersService
    {
        Task<string> GetAdminUserIdAsync();
    }
}

using System.Security.Claims;

namespace SimpleShoppingApp.Services.Users
{
    public interface IUsersService
    {
        Task<string?> GetAdminIdAsync();

        string? GetId(ClaimsPrincipal user);
    }
}

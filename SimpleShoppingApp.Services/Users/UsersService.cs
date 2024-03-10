using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SimpleShoppingApp.Data.Models;
using System.Security.Claims;

namespace SimpleShoppingApp.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public UsersService(UserManager<ApplicationUser> _userManager, IConfiguration _configuration)
        {
            userManager = _userManager;
            configuration = _configuration;
        }
        public async Task<string?> GetAdminIdAsync()
        {
            string adminUsername = configuration["AdminAccount:Email"];
            var adminUser = await userManager.FindByNameAsync(adminUsername);
            return await userManager.GetUserIdAsync(adminUser);
        }

        public async Task<string?> GetPhoneNumberAsync(string userId)
        {
            var foundUser = await userManager.FindByIdAsync(userId);
            if (foundUser == null)
            {
                return null;
            }
            var phoneNumber = foundUser.PhoneNumber;

            return phoneNumber;
        }

        public async Task<string?> GetEmailAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            var email = user.Email;

            return email;
        }

        public async Task<string?> GetFullNameAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            var fullName = user.FirstName + " " + user.LastName;
            return fullName;
        }
    }
}

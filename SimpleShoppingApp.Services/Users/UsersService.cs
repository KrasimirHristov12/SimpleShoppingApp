using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Users;

namespace SimpleShoppingApp.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IRepository<ApplicationUser> userRepo;

        public UsersService(UserManager<ApplicationUser> _userManager, 
            IConfiguration _configuration,
            IRepository<ApplicationUser> _userRepo)
        {
            userManager = _userManager;
            configuration = _configuration;
            userRepo = _userRepo;
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

        public async Task<UserInfoViewModel?> GetUserInfoAsync(string userId)
        {
            var user = await userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserInfoViewModel
                {
                    Email = u.Email,
                    FullName = u.FirstName + " " + u.LastName,
                    PhoneNumber = u.PhoneNumber
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> UpdateFullNameAsync(EditFullNameInputModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var nameSplitted = model.FullName.Split(" ", 2, StringSplitOptions.RemoveEmptyEntries);
            string firstName = nameSplitted[0];
            string lastName = nameSplitted[1];
            user.FirstName = firstName;
            user.LastName = lastName;
            await userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePhoneNumberAsync(EditPhoneNumberInputModel model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            user.PhoneNumber = model.PhoneNumber;
            await userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AdministrationUserViewModel>> GetUsersPerPageAsync(int page, int usersPerPage)
        {
            return await userManager.Users
                .Skip((page - 1) * usersPerPage)
                .Take(usersPerPage)
                .Select(u => new AdministrationUserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    UserName = u.UserName,
                })
                .ToListAsync();
        }

        public async Task<int> GetUsersCountAsync()
        {
            return await userManager.Users.CountAsync();
        }
    }
}

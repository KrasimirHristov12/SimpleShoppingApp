using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SimpleShoppingApp.Data.Models;

namespace SimpleShoppingApp.Data.Seeders
{
    public class UserSeeder : ISeeder
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;

        public UserSeeder(UserManager<ApplicationUser> _userManager,
            IConfiguration _config)
        {
            userManager = _userManager;
            config = _config;
        }
        public async Task SeedAsync()
        {
            string adminEmail = config["AdminAccount:Email"];
            string password = config["AdminAccount:Password"];
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                };

                var userCreationResult = await userManager.CreateAsync(user, password);
                if (userCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                }
            }

            
        }
    }
}

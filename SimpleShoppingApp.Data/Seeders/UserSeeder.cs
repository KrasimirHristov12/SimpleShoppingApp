using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleShoppingApp.Data.Models;

namespace SimpleShoppingApp.Data.Seeders
{
    public class UserSeeder : ISeeder
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration config;
        private readonly ILogger<UserSeeder> logger;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserSeeder(UserManager<ApplicationUser> _userManager,
            IConfiguration _config, 
            ILogger<UserSeeder> _logger,
            RoleManager<IdentityRole> _roleManager)
        {
            userManager = _userManager;
            config = _config;
            logger = _logger;
            roleManager = _roleManager;
        }
        public async Task SeedAsync()
        {
            string adminEmail = config["AdminAccount:Email"];
            string password = config["AdminAccount:Password"];
            var user = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
            };

            var userCreationResult = await userManager.CreateAsync(user, password);
            if (userCreationResult.Succeeded)
            {
                logger.LogInformation("Admin User Created.");
                var addToRoleResult = await userManager.AddToRoleAsync(user, "Administrator");
                if (addToRoleResult.Succeeded)
                {
                    logger.LogInformation("Successfully added to role Administator.");
                }
                else
                {
                    logger.LogInformation("Already in role administrator.");
                }
            }
            else
            {
                logger.LogInformation("Admin User Already Exists.");
            }
            
        }
    }
}

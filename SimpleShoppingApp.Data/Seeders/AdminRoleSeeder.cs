
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace SimpleShoppingApp.Data.Seeders
{
    public class AdminRoleSeeder : ISeeder
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<AdminRoleSeeder> logger;

        public AdminRoleSeeder(RoleManager<IdentityRole> _roleManager, ILogger<AdminRoleSeeder> _logger)
        {
            roleManager = _roleManager;
            logger = _logger;
        }
        public async Task SeedAsync()
        {
            var adminRole = new IdentityRole
            {
                Name = "Administrator",
            };
            var roleCreationResult = await roleManager.CreateAsync(adminRole);

            if (roleCreationResult.Succeeded)
            {
                logger.LogInformation("Role Administrator created successfully.");
            }
            else
            {
                logger.LogInformation("Role Administrator already exists");
            }
        }
    }
}

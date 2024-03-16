using Microsoft.AspNetCore.Identity;

namespace SimpleShoppingApp.Data.Seeders
{
    public class AdminRoleSeeder : ISeeder
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminRoleSeeder(RoleManager<IdentityRole> _roleManager)
        {
            roleManager = _roleManager;
        }
        public async Task SeedAsync()
        {
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                var adminRole = new IdentityRole
                {
                    Name = "Administrator",
                };
                await roleManager.CreateAsync(adminRole);
            }

        }
    }
}

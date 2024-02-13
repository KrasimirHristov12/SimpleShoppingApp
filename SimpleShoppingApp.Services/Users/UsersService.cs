﻿using Microsoft.AspNetCore.Identity;
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
        public string? GetId(ClaimsPrincipal user)
        {
            return userManager.GetUserId(user);
        }
    }
}
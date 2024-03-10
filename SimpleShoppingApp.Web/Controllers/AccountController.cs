using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Account;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Emails;
using SimpleShoppingApp.Web.Filters;

namespace SimpleShoppingApp.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ICartsService cartsService;
        private readonly IEmailsService emailsService;
        private readonly IRepository<ShoppingCart> cartsRepo;

        public AccountController(UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            IRepository<ShoppingCart> _cartsRepo,
            ICartsService cartsService,
            IEmailsService _emailsService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            this.cartsService = cartsService;
            emailsService = _emailsService;
            cartsRepo = _cartsRepo;
        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [NotAllowLoggedUsers]
        public async Task<IActionResult> Login(LoginInputModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return LocalRedirect(returnUrl);
        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }

                return View(model);
            }

            var cartId = await cartsService.AddAsync(user.Id);
            user.CartId = cartId;
            await cartsRepo.SaveChangesAsync();
            var sendEmailResult = await emailsService.SendAsync(user.Email, $"{user.FirstName} {user.LastName}", "Registration successful", "Thank you!");
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public async Task<IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}

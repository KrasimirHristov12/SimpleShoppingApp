using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleShoppingApp.Data.Models;
using SimpleShoppingApp.Data.Repository;
using SimpleShoppingApp.Models.Account;
using SimpleShoppingApp.Services.Carts;
using SimpleShoppingApp.Services.Emails;
using SimpleShoppingApp.Services.Users;
using SimpleShoppingApp.Extensions;
using SimpleShoppingApp.Web.Filters;

namespace SimpleShoppingApp.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ICartsService cartsService;
        private readonly IEmailsService emailsService;
        private readonly IUsersService usersService;
        private readonly IRepository<ShoppingCart> cartsRepo;

        public AccountController(UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            IRepository<ShoppingCart> _cartsRepo,
            ICartsService _cartsService,
            IEmailsService _emailsService,
            IUsersService _usersService)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            cartsService = _cartsService;
            emailsService = _emailsService;
            usersService = _usersService;
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
            var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token = confirmationToken }, Request.Scheme);
            var sendEmailResult = await emailsService.SendAsync(user.Email, $"{user.FirstName} {user.LastName}", "Confirm your account", $"Please click <a href=\"{confirmationLink}\" target=\"blank\">here</a> to confirm your account!");
            TempData["ConfirmationSent"] = "A confirmation mail was sent to you. Please review your inbox";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public async Task<IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return Redirect("/");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Redirect("/");
            }

            if (user.EmailConfirmed)
            {
                return Redirect("/");
            }

            var confirmResult = await userManager.ConfirmEmailAsync(user, token);

            if (!confirmResult.Succeeded)
            {
                return Redirect("/");
            }
            var cartId = await cartsService.AddAsync(user.Id);
            user.CartId = cartId;
            await cartsRepo.SaveChangesAsync();
            return View();
        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User with such email address does not exist.");
                return View(model);
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetPasswordLink = Url.Action(nameof(NewPassword), "Account", new { email = model.Email, token = token }, Request.Scheme);

            string mailContent = $"Hello,<br/>Please click the link below to reset your password.<br/><br/><a href=\"{resetPasswordLink}\">{resetPasswordLink}</a>";

            var mailResult = await emailsService.SendAsync(model.Email, string.Empty, "Reset your password", mailContent);

            TempData["ResetLinkSent"] = "A mail with instructions on how to change your password was sent. Please review your inbox.";
            return RedirectToAction(nameof(Login));

        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        public IActionResult NewPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                return Redirect("/");
            }

            var model = new NewPasswordInputModel
            {
                Email = email,
                Token = token,
            };

            return View(model);
        }

        [AllowAnonymous]
        [NotAllowLoggedUsers]
        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty ,"Something went wrong");
                return View(model);
            }

            var resetResult = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!resetResult.Succeeded) 
            {
                foreach (var err in resetResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }

            TempData["ChangePasswordSuccessful"] = "You have successfully changed your password!";
            string subject = "Password changed";
            string content = "You have successfully changed your password!";
            var emailResult = await emailsService.SendAsync(model.Email, string.Empty, subject, content);
            return RedirectToAction(nameof(Login));
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string userId = User.GetId();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }

            await signInManager.SignOutAsync();

            string email = user.Email;
            string subject = "Password changed";
            string content = "You have successfully changed your password!";

            var mailResult = await emailsService.SendAsync(email, string.Empty, subject, content);

            TempData["PasswordChangedSuccessfully"] = "You have successfully changed your password.";
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> Info()
        {
            var userId = User.GetId();
            var userInfo = await usersService.GetUserInfoAsync(userId);
            if (userInfo == null)
            {
                return NotFound();
            }
            return View(userInfo);
        }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private IEmailService _emailService;

        public HomeController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        private async Task<IActionResult> SignInUser (string username, string password)
        {
            await _signInManager.SignOutAsync();

            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (signInResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }

            }

            return RedirectToAction("Failed");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            return await SignInUser(username, password);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var user = new IdentityUser
            {
                UserName = username
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(EmailVerified), "Home", new { userId = user.Id, code}, Request.Scheme, Request.Host.ToString());
                await _emailService.SendAsync(email, "Verify Email", $"<a href=\"{link}\">Verify Email</a>", true);
                return RedirectToAction("EmailVerification");
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EmailVerified(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {

                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (result.Succeeded)
                {
                    return View();
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> LogOut() 
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Failed()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult EmailVerification() => View();
    }
}

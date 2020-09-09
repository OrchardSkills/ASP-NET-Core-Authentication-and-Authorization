using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Secret()
        {
            return View();
        }
        public IActionResult Authenticate()
        {
            var gradmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Bill"),
                new Claim(ClaimTypes.Email, "wpbest@gmail.com"),
                new Claim("Grama.Says", "Very nice man.")
            };
            var licenseClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Bill"),
                new Claim("DrivingLicense", "A+")
            };

            var grandmaIdentity = new ClaimsIdentity(gradmaClaims, "Grandma Identity");

            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Index");
        }
    }
}

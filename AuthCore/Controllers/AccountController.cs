using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AuthCore.Model;
//using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;


namespace AuthCore.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private static Dictionary<string, ClaimsPrincipal> _UserStore = new Dictionary<string, ClaimsPrincipal>();


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View(new UserProfile());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(UserProfile profile)
        {
            if (ModelState.IsValid)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, profile.UserName, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Role, profile.Role, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Email, profile.Email, ClaimValueTypes.Email),
                    new Claim("Username", profile.UserName),
                    new Claim("Password", profile.Password)
                };

                //Change
                //Middleware
                var id = new ClaimsIdentity(claims);
                var user = new ClaimsPrincipal(id);
                _UserStore.Add(profile.Email, user);

                return RedirectToAction("Signin");
            }

            return RedirectToAction("Register");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult SignIn()
        {
            return View(new UserProfile());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Signin(UserProfile p)
        {
            if (ModelState.IsValid && _UserStore.ContainsKey(p.Email))
            {
                //Unlike Framework we can't call this
                //var auth = Request.GetOwinContext().Authentication;
                //Deprecated
                var auth = HttpContext.Authentication;
                var props = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                };

                auth.SignInAsync("SuperCookieHW",_UserStore[p.Email], props);
                return RedirectToAction("Home");
            }
            return RedirectToAction("Error");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Signout()
        {
            if (ModelState.IsValid)
            {

                //var req = Request.GetOwinContext().Authentication;
                //Deprecated
                var req = Request.HttpContext.Authentication;
                req.SignOutAsync("SuperCookieHW");
                //req.SignOut();

                return RedirectToAction("Home");
            }

            return RedirectToAction("Error");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AuthStandard.Models;
using System.Security.Cryptography;
using Microsoft.Owin.Security;

namespace AuthStandard.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        private static Dictionary<string, ClaimsIdentity> _UserStore = new Dictionary<string, ClaimsIdentity>();
         

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
            if(ModelState.IsValid)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, profile.UserName, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Role, profile.Role, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Email, profile.Email, ClaimValueTypes.Email),
                    new Claim("Username", profile.UserName),
                    new Claim("Password", profile.Password)
                };

                var user = new ClaimsIdentity(claims, "ApplicationCookie");
                //var s = new SHA256CryptoServiceProvider().ComputeHash()

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
            if(ModelState.IsValid && _UserStore.ContainsKey(p.Email))
            {

                var auth = Request.GetOwinContext().Authentication;
                var props = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                };

                auth.SignIn(props, _UserStore[p.Email]);
                return RedirectToAction("Home");
            }
            return RedirectToAction("Error");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Signout()
        {
            if(ModelState.IsValid)
            {

                var req = Request.GetOwinContext().Authentication;
                req.SignOut();

                return RedirectToAction("Home");
            }

            return RedirectToAction("Error");
        }
    }
}
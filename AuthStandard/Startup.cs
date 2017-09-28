//using Microsoft.Owin.Builder;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace AuthStandard
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var opt = new CookieAuthenticationOptions()
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieHttpOnly = true,
                CookieName = "Anthony_Cookie",
                LoginPath = new PathString("/account/Signin"),

            };


            appBuilder.UseCookieAuthentication(opt);
        }
    }
}
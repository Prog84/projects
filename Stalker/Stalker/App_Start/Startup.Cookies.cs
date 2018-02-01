using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Stalker.Infrastructure;
using Stalker.Infrastructure.DbContexts;

namespace Stalker
{
    public partial class Startup
    {
        public void ConfigureCookies(IAppBuilder app)
        {
            app.CreatePerOwinContext<StalkerDbContext>(StalkerDbContext.Create);
            app.CreatePerOwinContext<StalkerUserManager>(StalkerUserManager.Create);
            app.CreatePerOwinContext<StalkerRoleManager>(StalkerRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}
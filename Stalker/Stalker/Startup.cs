using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
//using Stalker.WebApi;

//[assembly: OwinStartup(typeof(FoxController))]
namespace Stalker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureCookies(app);
            ConfigureOAuth(app);

            //HttpConfiguration config = new HttpConfiguration();
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //app.UseWebApi(config);
        }
    }
}
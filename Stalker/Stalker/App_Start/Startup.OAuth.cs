using System;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Stalker.Infrastructure;
using Stalker.Infrastructure.DbContexts;


namespace Stalker
{
	public partial class Startup
	{
	    public void ConfigureOAuth(IAppBuilder app)
	    {
	        var issuer = ConfigurationManager.AppSettings["issuer"];
	        var secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["secret"]);

            app.CreatePerOwinContext<StalkerDbContext>(StalkerDbContext.Create);
            app.CreatePerOwinContext<StalkerUserManager>(StalkerUserManager.Create);
            app.CreatePerOwinContext<StalkerRoleManager>(StalkerRoleManager.Create);

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(20),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(issuer)
            });

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = new[] { "Any" },
                IssuerSecurityTokenProviders =
                 new List<IIssuerSecurityTokenProvider> { new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret) }
            });
           
        }
    }
}
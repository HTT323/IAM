#region

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Web.Helpers;
using Demo.Nebula.Web;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

#endregion

[assembly: OwinStartup(typeof(Startup))]

namespace Demo.Nebula.Web
{
    public class Startup
    {
        [UsedImplicitly]
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    Authority = "https://auth.iam.dev:44300",
                    ClientId = "nebula-portal",
                    ResponseType = "id_token",
                    SignInAsAuthenticationType = "Cookies",
                    Scope = "openid profile email role",
                    RedirectUri = "https://www.nebula-portal.dev:44320/"
                });
        }
    }
}
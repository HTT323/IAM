#region

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web.Helpers;
using Iam.Orion.Web;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

#endregion

[assembly: OwinStartup(typeof(Startup))]

namespace Iam.Orion.Web
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
                    ClientId = "orion-portal",
                    ResponseType = "id_token",
                    SignInAsAuthenticationType = "Cookies",
                    Scope = "openid profile email role",
                    RedirectUri = "https://www.orion-portal.dev:44310/"
                });
        }
    }
}
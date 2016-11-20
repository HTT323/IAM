#region

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web.Helpers;
using Iam.Common;
using Iam.Web;
using Iam.Web.Middlewares;
using Iam.Web.Services;
using IdentityServer3.Core;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

#endregion

[assembly: OwinStartup(typeof(Startup))]

namespace Iam.Web
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseMigrations(AppSettings.IdsConnectionString);

            app.Use(typeof(TenantMiddleware));

            app.MapWhen(IsAuthDomain, ids => { ids.Use<ReconfigureIdsMiddleware>(); });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.MapWhen(IsClientDomain, iam =>
            {
                iam.UseOpenIdConnectAuthentication(
                    new OpenIdConnectAuthenticationOptions
                    {
                        Authority = AppSettings.IdpAuthority,
                        ClientId = AppSettings.IamClientId,
                        ResponseType = "id_token",
                        SignInAsAuthenticationType = "Cookies",
                        Scope = "openid profile email role",
                        Notifications = GetNotifications()
                    });
            });
        }

        private OpenIdConnectAuthenticationNotifications GetNotifications()
        {
            return new OpenIdConnectAuthenticationNotifications
            {
                RedirectToIdentityProvider =
                    rto =>
                    {
                        var uri = rto.Request.Uri;

                        if (rto.ProtocolMessage.RequestType == OpenIdConnectRequestType.AuthenticationRequest)
                        {
                            var parts = uri.Host.Split('.');

                            Ensure.Equal(parts.Length, 3);

                            rto.ProtocolMessage.AcrValues = $"tenant:{parts[0]}";
                        }

                        var redirectUri = uri.Port == 443
                            ? $"{uri.Scheme}://{uri.Host}/"
                            : $"{uri.Scheme}://{uri.Host}:{uri.Port}/";

                        rto.ProtocolMessage.RedirectUri = redirectUri;
                        rto.ProtocolMessage.PostLogoutRedirectUri = redirectUri;

                        return Task.FromResult(0);
                    }
            };
        }

        private bool IsAuthDomain(IOwinContext context)
        {
            var sd = context.Get<string>(TenantMiddleware.TenantKey);

            return sd == AppSettings.AuthDomain;
        }

        private bool IsClientDomain(IOwinContext context)
        {
            var sd = context.Get<string>(TenantMiddleware.TenantKey);

            return sd != AppSettings.AuthDomain;
        }
    }
}
#region

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Helpers;
using Iam.Common;
using Iam.Web;
using Iam.Web.Middlewares;
using Iam.Web.Services;
using IdentityModel;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using JetBrains.Annotations;
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

            app.UseMigrations(AppSettings.IamConnectionString);

            app.Use(typeof(TenantMiddleware));

            app.MapWhen(IsAuthDomain, ids =>
            {
                ids.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = AppSettings.IdentityServerSiteName,
                    SigningCertificate = LoadCertificate(),
                    RequireSsl = true,
                    Factory = new IdentityServerServiceFactory().Configure(AppSettings.IamConnectionString),
                    EnableWelcomePage = false
                });
            });

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
                        Scope = "openid profile email",
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

        private X509Certificate2 LoadCertificate()
        {
            return X509.LocalMachine.My.SubjectDistinguishedName
                .Find(AppSettings.CertificateSubject)
                .FirstOrDefault();
        }
    }
}
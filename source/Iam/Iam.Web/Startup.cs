#region

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using Iam.Common;
using Iam.Web;
using Iam.Web.Plumbing;
using IdentityModel;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
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

            app.Map(AppSettings.IdentityServerPath, ids =>
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

            ConfigureIam(app);
        }

        /// <summary>
        /// Configure Identity and Access Management.
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureIam(IAppBuilder app)
        {
            
        }

        /// <summary>
        /// Load certificate using subject distinguished name.
        /// </summary>
        /// <returns></returns>
        private X509Certificate2 LoadCertificate()
        {
            return X509.LocalMachine.My.SubjectDistinguishedName
                .Find(AppSettings.CertificateSubject)
                .FirstOrDefault();
        }
    }
}
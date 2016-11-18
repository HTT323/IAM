#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Iam.Common;
using Iam.Web.Services;
using IdentityModel;
using IdentityServer3.Core.Configuration;
using JetBrains.Annotations;
using Microsoft.Owin.Builder;
using Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

#endregion

namespace Iam.Web.Middlewares
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ReconfigureIdsMiddleware
    {
        private AppFunc _dynamicAppFunc;

        public ReconfigureIdsMiddleware(AppFunc next)
        {
            var fileSystemWatcher =
                new FileSystemWatcher(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWatch"))
                {
                    EnableRaisingEvents = true
                };

            fileSystemWatcher.Changed += (sender, args) => ConfigurePipeline(sender, args, next);

            ConfigurePipeline(null, null, next);
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            return _dynamicAppFunc(environment);
        }

        private void ConfigurePipeline(object sender, FileSystemEventArgs args, AppFunc next)
        {
            // TODO: Handle multiple events being triggered by FileSystemWatcher.
            var app = new AppBuilder();

            app.UseIdentityServer(new IdentityServerOptions
            {
                SiteName = AppSettings.IdentityServerSiteName,
                SigningCertificate = LoadCertificate(),
                RequireSsl = true,
                Factory = new IdentityServerServiceFactory().Configure(),
                EnableWelcomePage = false,
                AuthenticationOptions = new AuthenticationOptions { RememberLastUsername = true }
            });

            app.Run(ctx => next(ctx.Environment));

            _dynamicAppFunc = app.Build();
        }

        private static X509Certificate2 LoadCertificate()
        {
            return X509.LocalMachine.My.SubjectDistinguishedName
                .Find(AppSettings.CertificateSubject)
                .FirstOrDefault();
        }
    }
}
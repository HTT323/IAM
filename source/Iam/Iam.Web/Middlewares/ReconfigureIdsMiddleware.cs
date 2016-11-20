﻿#region

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Iam.Common;
using Iam.Identity.Tenant;
using Iam.Web.Services;
using IdentityModel;
using IdentityServer3.Core.Configuration;
using JetBrains.Annotations;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Microsoft.Owin.Security.WsFederation;
using Newtonsoft.Json;
using Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

#endregion

namespace Iam.Web.Middlewares
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ReconfigureIdsMiddleware
    {
        private AppFunc _dynamicAppFunc;
        private string _file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWatch", "config.ids");
        private DateTime? _lastReconfigureDateUtc;
        private string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWatch");
        private string _wsfed = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ConfigWatch", "wsfedmappings.json");

        public ReconfigureIdsMiddleware(AppFunc next)
        {
            var fileSystemWatcher =
                new FileSystemWatcher(_path)
                {
                    EnableRaisingEvents = true
                };

            fileSystemWatcher.Changed += (_, __) => ConfigurePipeline(next);

            ConfigurePipeline(next);
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            return _dynamicAppFunc(environment);
        }

        private void ConfigurePipeline(AppFunc next)
        {
            EnsureConfigIds();

            // TODO: A change in WSFED mapping should update both config.ids and wsfedmappings.json
            var stamp = File.GetLastWriteTimeUtc(_file);

            if (_lastReconfigureDateUtc != null && _lastReconfigureDateUtc == stamp)
            {
                return;
            }

            _lastReconfigureDateUtc = stamp;

            var app = new AppBuilder();

            if (!app.Properties.ContainsKey("host.AppName"))
            {
                app.Properties.Add("host.AppName", "IdServerHost");
            }

            app.UseIdentityServer(new IdentityServerOptions
            {
                SiteName = AppSettings.IdentityServerSiteName,
                SigningCertificate = LoadCertificate(),
                RequireSsl = true,
                Factory = new IdentityServerServiceFactory().Configure(),
                EnableWelcomePage = false,
                AuthenticationOptions =
                    new AuthenticationOptions
                    {
                        RememberLastUsername = true,
                        IdentityProviders = ConfigureIdentityProviders,
                        EnableAutoCallbackForFederatedSignout = true
                    }
            });

            app.Run(ctx => next(ctx.Environment));

            _dynamicAppFunc = app.Build();
        }

        private void EnsureConfigIds()
        {
            if (File.Exists(_file))
                return;

            using (var sw = File.CreateText(_file))
            {
                sw.Write(DateTime.UtcNow);
            }
        }

        private static X509Certificate2 LoadCertificate()
        {
            return X509.LocalMachine.My.SubjectDistinguishedName
                .Find(AppSettings.CertificateSubject)
                .FirstOrDefault();
        }

        private IEnumerable<WsFedMapping> GetWsFedProviders()
        {
            if (!File.Exists(_wsfed))
                return new List<WsFedMapping>();

            var json = File.ReadAllText(_wsfed);

            return string.IsNullOrWhiteSpace(json)
                ? new List<WsFedMapping>()
                : JsonConvert.DeserializeObject<IEnumerable<WsFedMapping>>(json);
        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var feds = GetWsFedProviders();

            foreach (var wsFed in feds)
            {
                var wsf = new WsFederationAuthenticationOptions
                {
                    AuthenticationType = $"wsfed{wsFed.WsFedId}",
                    Caption = wsFed.Caption,
                    TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidAudience = wsFed.Audience
                        },
                    SignInAsAuthenticationType = signInAsType,
                    CallbackPath = new PathString($"/callback/wsfed{wsFed.WsFedId}"),
                    MetadataAddress = wsFed.MetadataAddress,
                    Wtrealm = wsFed.Realm
                };

                app.UseWsFederationAuthentication(wsf);
            }
        }
    }
}
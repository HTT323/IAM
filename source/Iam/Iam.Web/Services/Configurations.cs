#region

using System.Data.Entity;
using System.Diagnostics;
using Iam.Common;
using Iam.Common.Contracts;
using Iam.Identity;
using Iam.Web.Migrations.Clients;
using Iam.Web.Migrations.Scopes;
using Iam.Web.Migrations.Users;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.EntityFramework;
using JetBrains.Annotations;
using Owin;

#endregion

namespace Iam.Web.Services
{
    [UsedImplicitly]
    public static class Configurations
    {
        private static bool _debug;

        static Configurations()
        {
            SetDebugFlag();
        }

        [Conditional("DEBUG")]
        private static void SetDebugFlag()
        {
            _debug = true;
        }

        /// <summary>
        ///     Enable migrations.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IAppBuilder UseMigrations(this IAppBuilder app, string connectionString)
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<IdsContext, UserMigration>(connectionString));

            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<ScopeConfigurationDbContext, ScopeMigration>(connectionString));

            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<ClientConfigurationDbContext, ClientMigration>(connectionString));

            return app;
        }

        /// <summary>
        ///     Configure ID Server to use a custom view service, EF and ASP.NET Identity.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static IdentityServerServiceFactory Configure(
            this IdentityServerServiceFactory factory)
        {
            if (_debug)
            {
                factory.Register(new Registration<ICache, NoCache>());
            }
            else
            {
                factory.Register(new Registration<ICache, HttpCache>());
            }

            factory.Register(new Registration<IBundle, Bundle>());
            
            factory.CustomRequestValidator = new Registration<ICustomRequestValidator, CustomRequestValidator>();
            factory.ViewService = new Registration<IViewService, CustomViewService>();

            return factory.RegisterIdentityServices().RegisterUserServices();
        }

        private static IdentityServerServiceFactory RegisterIdentityServices(
            this IdentityServerServiceFactory factory)
        {
            var option = new EntityFrameworkServiceOptions {ConnectionString = AppSettings.IdsConnectionString};

            factory.RegisterOperationalServices(option);
            factory.RegisterConfigurationServices(option);

            return factory;
        }

        private static IdentityServerServiceFactory RegisterUserServices(
            this IdentityServerServiceFactory factory)
        {
            factory.UserService = new Registration<IUserService, UserService>();

            factory.Register(new Registration<IdsContext>());
            factory.Register(new Registration<IdsUserStore>());
            factory.Register(new Registration<IdsUserManager>());

            factory.Register(new Registration<TenantContext>());
            factory.Register(new Registration<TenantUserStore>());
            factory.Register(new Registration<TenantUserManager>());

            return factory;
        }
    }
}
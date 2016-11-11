#region

using System.Data.Entity;
using System.Diagnostics;
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
                new MigrateDatabaseToLatestVersion<IamContext, UserMigration>(connectionString));

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
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IdentityServerServiceFactory Configure(
            this IdentityServerServiceFactory factory,
            string connectionString)
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

            factory.ViewService = new Registration<IViewService, CustomViewService>();

            return factory.RegisterIdentityServices(connectionString).RegisterUserServices(connectionString);
        }

        private static IdentityServerServiceFactory RegisterIdentityServices(
            this IdentityServerServiceFactory factory,
            string connectionString)
        {
            var option = new EntityFrameworkServiceOptions {ConnectionString = connectionString};

            factory.RegisterOperationalServices(option);
            factory.RegisterConfigurationServices(option);

            return factory;
        }

        private static IdentityServerServiceFactory RegisterUserServices(
            this IdentityServerServiceFactory factory,
            string connectionString)
        {
            factory.UserService = new Registration<IUserService, UserService>();
            factory.Register(new Registration<IamUserManager>());
            factory.Register(new Registration<IamUserStore>());
            factory.Register(new Registration<IamContext>(resolver => new IamContext(connectionString)));

            return factory;
        }
    }
}
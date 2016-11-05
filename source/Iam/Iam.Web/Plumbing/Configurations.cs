#region

using System.Data.Entity;
using Iam.Identity;
using Iam.Web.Migrations.Users;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using IdentityServer3.EntityFramework;
using JetBrains.Annotations;
using Owin;

#endregion

namespace Iam.Web.Plumbing
{
    [UsedImplicitly]
    public static class Configurations
    {
        public static IAppBuilder UseMigrations(this IAppBuilder app, string connectionString)
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<IamContext, UserMigration>(connectionString));

            return app;
        }

        public static IdentityServerServiceFactory Configure(
            this IdentityServerServiceFactory factory,
            string connectionString)
        {
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
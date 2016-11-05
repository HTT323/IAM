#region

using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Claims;
using Iam.Common;
using Iam.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Constants = IdentityServer3.Core.Constants;

#endregion

namespace Iam.Web.Migrations.Users
{
    public class UserMigration : DbMigrationsConfiguration<IamContext>
    {
        public UserMigration()
        {
            MigrationsDirectory = @"Migrations\Users";
        }

        protected override void Seed(IamContext context)
        {
            base.Seed(context);

            if (!context.Roles.Any(f => f.Name == AppSettings.IamAdministratorRole))
            {
                new IamRoleManager(new IamRoleStore(context))
                    .Create(new IdentityRole(AppSettings.IamAdministratorRole));
            }

            if (context.Users.Any(f => f.UserName == AppSettings.SeedUserName))
                return;

            var user = new IamUser
            {
                Id = GuidCombGenerator.Generate().ToString(),
                UserName = AppSettings.SeedUserName
            };

            var manager = new IamUserManager(new IamUserStore(context));

            manager.Create(user, AppSettings.SeedPassword);

            manager.AddClaim(
                user.Id,
                new Claim(Constants.ClaimTypes.Role, AppSettings.IamAdministratorRole));
        }
    }
}
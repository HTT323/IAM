#region

using System.Data.Entity.Migrations;
using System.Security.Claims;
using Iam.Common;
using Iam.Identity;
using Microsoft.AspNet.Identity;
using Constants = IdentityServer3.Core.Constants;
using System.Linq;

#endregion

namespace Iam.Web.Migrations.Users
{
    public class UserMigration : DbMigrationsConfiguration<IdsContext>
    {
        public UserMigration()
        {
            MigrationsDirectory = @"Migrations\Users";
        }

        protected override void Seed(IdsContext context)
        {
            if (context.Users.Any(f => f.UserName == AppSettings.SeedUserName))
                return;

            var user = new IamUser
            {
                Id = GuidCombGenerator.Generate().ToString(),
                UserName = AppSettings.SeedUserName
            };

            var manager = new IdsUserManager(new IdsUserStore(context));

            manager.Create(user, AppSettings.SeedPassword);

            manager.AddClaim(
                user.Id,
                new Claim(Constants.ClaimTypes.Role, AppSettings.IamAdministratorRole));
        }
    }
}
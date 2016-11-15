#region

using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Iam.Common;
using Microsoft.AspNet.Identity;
using Constants = IdentityServer3.Core.Constants;

#endregion

namespace Iam.Identity.Tenant
{
    public class TenantUserInitializer : CreateDatabaseIfNotExists<TenantContext>
    {
        protected override void Seed(TenantContext context)
        {
            SeedTestData(context);

            base.Seed(context);
        }

        [Conditional("DEBUG")]
        private static void SeedTestData(TenantContext context)
        {
            if (context.Users.Any(f => f.UserName == context.CacheKey))
                return;

            var user = new IamUser
            {
                Id = GuidCombGenerator.Generate().ToString(),
                UserName = context.CacheKey
            };

            var manager = new TenantUserManager(new TenantUserStore(context));

            manager.Create(user, $"{ConvertToProper(context.CacheKey)}123#");

            manager.AddClaim(
                user.Id,
                new Claim(Constants.ClaimTypes.Role, AppSettings.IamAdministratorRole));
        }

        private static string ConvertToProper(string text)
        {
            var ti = new CultureInfo("en-US", false).TextInfo;

            return ti.ToTitleCase(text);
        }
    }
}
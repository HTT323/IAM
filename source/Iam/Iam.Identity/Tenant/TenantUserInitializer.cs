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

        /// <summary>
        ///     Seed data with the following format:
        ///     Username: {tenant} e.g. nebula
        ///     Password: {Tenant}123# e.g. Nebula123#
        ///     Role: {Tenant} Administrator e.g. Nebula Administrator
        /// </summary>
        /// <param name="context"></param>
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
                new Claim(Constants.ClaimTypes.Role, $"{ConvertToProper(context.CacheKey)} Administrator"));
        }

        private static string ConvertToProper(string text)
        {
            var ti = new CultureInfo("en-US", false).TextInfo;

            return ti.ToTitleCase(text);
        }
    }
}
#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;

#endregion

namespace Iam.Web.Migrations.Scopes
{
    public class ScopeMigration : DbMigrationsConfiguration<ScopeConfigurationDbContext>
    {
        public ScopeMigration()
        {
            MigrationsDirectory = @"Migrations\Scopes";
        }

        protected override void Seed(ScopeConfigurationDbContext context)
        {
            var scopes = new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                StandardScopes.Address,
                StandardScopes.Phone,
                StandardScopes.OfflineAccess
            };

            context.Scopes.AddOrUpdate(s => s.Name,
                scopes.Select(m => m.ToEntity()).ToArray());
        }
    }
}
#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;
using System.Diagnostics;

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
            var scopes = new List<Scope>();

            scopes.AddRange(StandardScopes.All);

            scopes.Add(new Scope
            {
                Name = "role",
                Type = ScopeType.Identity,
                Emphasize = true,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(Constants.ClaimTypes.Role)
                }
            });

            AddSampleApi(scopes);

            context.Scopes.AddOrUpdate(s => s.Name,
                scopes.Select(m => m.ToEntity()).ToArray());
        }

        [Conditional("DEBUG")]
        private void AddSampleApi(List<Scope> scopes)
        {
            scopes.Add(new Scope
            {
                Name = "nebula-api-scope",
                Type = ScopeType.Resource,
                DisplayName = "Nebula API Scope",
                Description = "Access to Nebula API"
            });
        }
    }
}
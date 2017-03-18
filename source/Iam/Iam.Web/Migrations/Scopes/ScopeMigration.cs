#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFramework;

#endregion

namespace Iam.Web.Migrations.Scopes
{
    public class ScopeMigration : DbMigrationsConfiguration<ScopeConfigurationDbContext>
    {
        private bool _debug;

        public ScopeMigration()
        {
            MigrationsDirectory = @"Migrations\Scopes";
            SetDebugFlag();
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

            if (_debug)
            {
                AddSampleApi(scopes);
            }

            context.Scopes.AddOrUpdate(s => s.Name,
                scopes.Select(m => m.ToEntity()).ToArray());
        }

        private void AddSampleApi(List<Scope> scopes)
        {
            scopes.Add(new Scope
            {
                Name = "nebula-api-scope",
                Type = ScopeType.Resource,
                DisplayName = "Nebula API Scope",
                Description = "Access to Nebula API",
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(Constants.ClaimTypes.Role)
                }
            });

            scopes.Add(new Scope
            {
                Name = "extranet-scope",
                Type = ScopeType.Resource,
                DisplayName = "Extranet API Scope",
                Description = "Access to Extranet API",
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(Constants.ClaimTypes.Role)
                }
            });
        }

        [Conditional("DEBUG")]
        private void SetDebugFlag()
        {
            _debug = true;
        }
    }
}
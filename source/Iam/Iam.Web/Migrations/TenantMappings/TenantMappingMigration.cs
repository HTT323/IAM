#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using Iam.Common;
using Iam.Identity;
using Iam.Identity.Tenant;

#endregion

namespace Iam.Web.Migrations.TenantMappings
{
    public class TenantMappingMigration : DbMigrationsConfiguration<AdminContext>
    {
        private bool _debug;

        public TenantMappingMigration()
        {
            MigrationsDirectory = @"Migrations\TenantMappings";
            SetDebugFlag();
        }

        protected override void Seed(AdminContext context)
        {
            if (context.TenantMappings.FirstOrDefault() != null)
                return;

            var mappings = new List<TenantMapping>();

            if (_debug)
            {
                context.TenantMappings.AddRange(DataFileLoader.Load<TenantMapping>("tenantmappings.json"));
            }
            else
            {
                context.TenantMappings.Add(
                    new TenantMapping
                    {
                        TenantMappingId = GuidCombGenerator.Generate(),
                        TenantId = AppSettings.AdminDomain,
                        TenantName = AppSettings.IamClientName,
                        ClientId = AppSettings.IamClientId
                    });
            }
            
            context.SaveChanges();
        }

        [Conditional("DEBUG")]
        private void SetDebugFlag()
        {
            _debug = true;
        }
    }
}
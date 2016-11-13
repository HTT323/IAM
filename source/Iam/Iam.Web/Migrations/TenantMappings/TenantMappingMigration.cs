#region

using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Iam.Identity;
using Iam.Identity.Tenant;

#endregion

namespace Iam.Web.Migrations.TenantMappings
{
    public class TenantMappingMigration : DbMigrationsConfiguration<AdminContext>
    {
        public TenantMappingMigration()
        {
            MigrationsDirectory = @"Migrations\TenantMappings";
        }

        protected override void Seed(AdminContext context)
        {
            if (context.TenantMappings.FirstOrDefault() != null)
                return;

            var mappings = new List<TenantMapping>();

            context.TenantMappings.AddRange(DataFileLoader.Load<TenantMapping>("tenantmappings.json"));

            context.SaveChanges();
        }
    }
}
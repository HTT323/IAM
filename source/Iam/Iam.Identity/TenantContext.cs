#region

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Iam.Common;
using Iam.Identity.Tenant;
using JetBrains.Annotations;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly]
    public class TenantContext : IdentityDbContext<IamUser>, IDbModelCacheKeyProvider
    {
        public TenantContext() : base(AppSettings.IamConnectionString)
        {
        }

        public string CacheKey { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (!string.IsNullOrWhiteSpace(CacheKey))
            {
                modelBuilder.HasDefaultSchema(CacheKey);
                Database.SetInitializer(new TenantUserInitializer());
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
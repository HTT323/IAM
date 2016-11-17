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
    public class IamContext : IdentityDbContext<IamUser>, IDbModelCacheKeyProvider
    {
        public IamContext() : base(AppSettings.IamConnectionString)
        {
        }

        public string CacheKey { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (!string.IsNullOrWhiteSpace(CacheKey))
            {
                modelBuilder.HasDefaultSchema(CacheKey);
                Database.SetInitializer(new UserInitializer());
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
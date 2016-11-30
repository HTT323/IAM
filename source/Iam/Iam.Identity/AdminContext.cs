#region

using System;
using System.Data.Entity;
using System.Linq;
using Iam.Common;
using Iam.Common.Contracts;
using Iam.Identity.Tenant;
using JetBrains.Annotations;

#endregion

namespace Iam.Identity
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AdminContext : DbContext, IAdminContext
    {
        public AdminContext() : base(AppSettings.IdsConnectionString)
        {
        }

        public IQueryable<T> Repository<T>() where T : class, IModel
        {
            return Set<T>().AsNoTracking();
        }

        public void Add<T>(T entity) where T : class, IModel
        {
            Set<T>().Add(entity);
        }

        public void Update<T>(T entity) where T : class, IModel
        {
            var original = Set<T>().Find(entity.ObjectId);

            if (original == null)
                throw new InvalidOperationException();

            var entry = Entry(original);

            entry.CurrentValues.SetValues(entity);
        }

        public void Commit()
        {
            SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new TenantMappingConfiguration());
            modelBuilder.Configurations.Add(new WsFedMappingConfiguration());
            modelBuilder.Configurations.Add(new WsFedProtocolMappingConfiguration());

            base.OnModelCreating(modelBuilder);
        }
        
        #region DB Sets

        public DbSet<TenantMapping> TenantMappings { get; set; }
        public DbSet<WsFedMapping> WsFedMappings { get; set; }
        public DbSet<WsFedProtocolMapping> WsFedProtocolMappings { get; set; }

        #endregion
    }
}
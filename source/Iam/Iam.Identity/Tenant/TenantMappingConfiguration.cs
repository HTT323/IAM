#region

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion

namespace Iam.Identity.Tenant
{
    public class TenantMappingConfiguration : EntityTypeConfiguration<TenantMapping>
    {
        public TenantMappingConfiguration()
        {
            HasKey(p => p.TenantMappingId);
            Property(p => p.TenantMappingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.TenantId).HasMaxLength(200).IsRequired();
            Property(p => p.TenantName).HasMaxLength(200).IsRequired();
            Property(p => p.ClientId).HasMaxLength(200).IsRequired();
        }
    }
}
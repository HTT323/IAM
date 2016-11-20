#region

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion

namespace Iam.Identity.Tenant
{
    public class WsFedMappingConfiguration : EntityTypeConfiguration<WsFedMapping>
    {
        public WsFedMappingConfiguration()
        {
            HasKey(p => p.WsFedMappingId);
            Property(p => p.WsFedMappingId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.TenantId).HasMaxLength(200).IsRequired();
            Property(p => p.Caption).HasMaxLength(40).IsRequired();
            Property(p => p.MetadataAddress).HasMaxLength(1000).IsRequired();
            Property(p => p.Realm).HasMaxLength(200).IsRequired();
            Property(p => p.Audience).HasMaxLength(200).IsRequired();
        }
    }
}
#region

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion

namespace Iam.Identity.Tenant
{
    public class WsFedProtocolMappingConfiguration : EntityTypeConfiguration<WsFedProtocolMapping>
    {
        public WsFedProtocolMappingConfiguration()
        {
            HasKey(p => p.WsFedMappingProtocolId);
            Property(p => p.WsFedMappingProtocolId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(p => p.Realm).HasMaxLength(200).IsRequired();
            Property(p => p.TenantId).HasMaxLength(200).IsRequired();
            Property(p => p.Caption).HasMaxLength(200).IsRequired();
        }
    }
}
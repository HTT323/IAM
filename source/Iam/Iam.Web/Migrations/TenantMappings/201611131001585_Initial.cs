namespace Iam.Web.Migrations.TenantMappings
{
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TenantMappings",
                c => new
                    {
                        TenantMappingId = c.Guid(nullable: false),
                        TenantId = c.String(nullable: false, maxLength: 200),
                        TenantName = c.String(nullable: false, maxLength: 200),
                        ClientId = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.TenantMappingId);
            
            CreateTable(
                "dbo.WsFedMappings",
                c => new
                    {
                        WsFedMappingId = c.Guid(nullable: false),
                        WsFedId = c.Int(nullable: false),
                        TenantId = c.String(nullable: false, maxLength: 200),
                        Caption = c.String(nullable: false, maxLength: 40),
                        MetadataAddress = c.String(nullable: false, maxLength: 1000),
                        Realm = c.String(nullable: false, maxLength: 200),
                        Audience = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.WsFedMappingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WsFedMappings");
            DropTable("dbo.TenantMappings");
        }
    }
}

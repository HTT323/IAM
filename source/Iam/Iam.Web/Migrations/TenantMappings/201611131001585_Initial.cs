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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TenantMappings");
        }
    }
}

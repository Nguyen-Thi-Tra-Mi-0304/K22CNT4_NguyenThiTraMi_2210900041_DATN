namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableIndustry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Industries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NAMEINDUSTRY = c.String(),
                        CREATEAT = c.DateTime(nullable: false),
                        STT = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Industries");
        }
    }
}

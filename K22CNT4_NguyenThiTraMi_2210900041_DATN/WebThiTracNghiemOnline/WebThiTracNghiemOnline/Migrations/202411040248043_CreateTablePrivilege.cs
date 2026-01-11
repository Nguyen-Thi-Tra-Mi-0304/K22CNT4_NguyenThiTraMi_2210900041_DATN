namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTablePrivilege : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Privileges",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NAME = c.String(),
                        DISCRIBLE = c.String(),
                        CREATEAT = c.DateTime(),
                        STT = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Privileges");
        }
    }
}

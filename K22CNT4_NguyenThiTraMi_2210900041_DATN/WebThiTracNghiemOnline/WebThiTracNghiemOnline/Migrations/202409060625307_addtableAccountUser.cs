namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtableAccountUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NAMEUSER = c.String(),
                        ACCOUNT = c.String(),
                        PASS = c.String(),
                        IMG = c.String(),
                        SEX = c.String(),
                        EMAIL = c.String(),
                        ROLE = c.Int(nullable: false),
                        CREATEAT = c.DateTime(nullable: false),
                        STT = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccountUsers");
        }
    }
}

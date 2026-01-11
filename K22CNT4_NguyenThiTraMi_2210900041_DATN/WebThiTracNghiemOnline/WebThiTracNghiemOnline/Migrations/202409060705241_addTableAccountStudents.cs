namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableAccountStudents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountStudents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IDINDUSTRY = c.Int(nullable: false),
                        IDCOURSE = c.Int(nullable: false),
                        NAMESTUDENT = c.String(),
                        ACCOUNT = c.String(),
                        PASS = c.String(),
                        SEX = c.String(),
                        EMAIL = c.String(),
                        BIRTHDAY = c.DateTime(),
                        ROLE = c.Int(nullable: false),
                        CREATEAT = c.DateTime(nullable: false),
                        CREATEUPDATE = c.DateTime(),
                        STT = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccountStudents");
        }
    }
}

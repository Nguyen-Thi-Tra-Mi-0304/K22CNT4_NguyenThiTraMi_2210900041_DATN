namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableCLass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Classes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IDSEMESTER = c.Int(nullable: false),
                        IDINDUSTRY = c.Int(nullable: false),
                        IDTEACHER = c.Int(nullable: false),
                        KEYCLASS = c.String(),
                        NAMECLASS = c.String(),
                        DESCRIBE = c.String(),
                        CREATEAT = c.DateTime(nullable: false),
                        CREATEUPDATE = c.DateTime(nullable: false),
                        STT = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.StudentOnClasses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IDSTUDENT = c.Int(nullable: false),
                        IDCLASS = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.StudentOnClasses");
            DropTable("dbo.Classes");
        }
    }
}

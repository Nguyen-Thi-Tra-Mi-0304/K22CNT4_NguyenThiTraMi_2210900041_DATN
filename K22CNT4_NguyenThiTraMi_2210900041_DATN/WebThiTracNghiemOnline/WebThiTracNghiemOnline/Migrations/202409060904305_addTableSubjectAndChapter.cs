namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableSubjectAndChapter : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chapters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IDSUBJECT = c.Int(nullable: false),
                        NAMECHAPTER = c.String(),
                        CREATEAT = c.DateTime(nullable: false),
                        STT = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        IDSEMESTER = c.Int(nullable: false),
                        IDTEACHER = c.Int(nullable: false),
                        NAMESUBJECT = c.String(),
                        TINCHI = c.Int(),
                        DESCRIBE = c.String(),
                        STT = c.Int(nullable: false),
                        CREATEAT = c.DateTime(nullable: false),
                        CREATEUPDATE = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Subjects");
            DropTable("dbo.Chapters");
        }
    }
}

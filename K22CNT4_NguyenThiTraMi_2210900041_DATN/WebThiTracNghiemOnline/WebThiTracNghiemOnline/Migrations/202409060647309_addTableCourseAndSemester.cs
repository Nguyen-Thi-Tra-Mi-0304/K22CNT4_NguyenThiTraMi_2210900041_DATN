namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableCourseAndSemester : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NAMECOURSE = c.String(),
                        DESCRIBLE = c.String(),
                        STT = c.Int(nullable: false),
                        CREATEAT = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Semesters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        COURSEID = c.Int(nullable: false),
                        NAMESEMESTER = c.String(),
                        STARTDAY = c.DateTime(nullable: false),
                        ENDDAY = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Semesters");
            DropTable("dbo.Courses");
        }
    }
}

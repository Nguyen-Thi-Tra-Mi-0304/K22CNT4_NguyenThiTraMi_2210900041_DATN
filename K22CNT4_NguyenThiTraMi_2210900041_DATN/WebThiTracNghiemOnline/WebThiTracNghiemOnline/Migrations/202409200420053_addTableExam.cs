namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableExam : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exams",
                c => new
                    {
                        IDEXAM = c.Int(nullable: false, identity: true),
                        IDSUBJECT = c.String(nullable:false),
                        IDSEMESTER = c.Int(nullable: false),
                        IDCLASS = c.Int(nullable: false),
                        IDTEACHER = c.Int(nullable: false),
                        NAMEEXAM = c.String(),
                        DESCRIBLE = c.String(),
                        CREATEAT = c.DateTime(nullable: false),
                        CREATEUPDATE = c.DateTime(nullable: true),
                        CREATESTART = c.DateTime(nullable: false),
                        CREATEEND = c.DateTime(nullable: false),
                        ASSIGNMENTTIME = c.Int(nullable: false),
                        NUMBER = c.Int(nullable: false),
                        PASSWORD = c.String(),
                        STATUS = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IDEXAM);
            
            CreateTable(
                "dbo.QuestionsToExams",
                c => new
                    {
                        IDQUESTIONTOEXAM = c.Int(nullable: false, identity: true),
                        IDEXAM = c.Int(nullable: false),
                        IDQUESTION = c.Int(nullable: false),
                        STATUS = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IDQUESTIONTOEXAM);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.QuestionsToExams");
            DropTable("dbo.Exams");
        }
    }
}

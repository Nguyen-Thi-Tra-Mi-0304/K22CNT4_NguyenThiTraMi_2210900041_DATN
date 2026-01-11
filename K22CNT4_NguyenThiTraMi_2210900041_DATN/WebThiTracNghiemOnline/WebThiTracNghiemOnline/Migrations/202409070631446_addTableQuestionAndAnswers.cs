namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableQuestionAndAnswers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        IDANSWER = c.Int(nullable: false, identity: true),
                        IDQUESTION = c.Int(nullable: false),
                        ANSWERTEXT = c.String(),
                        ISCORRECT = c.Boolean(nullable: false),
                        ANSWERTYPE = c.String(),
                        BLANKPOSITION = c.Int(),
                    })
                .PrimaryKey(t => t.IDANSWER);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        IDQUESTION = c.Int(nullable: false, identity: true),
                        QUESTIONTEXT = c.String(),
                        QUESTIONTYPE = c.Int(nullable: false),
                        DIFFICULTY = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IDQUESTION);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
        }
    }
}

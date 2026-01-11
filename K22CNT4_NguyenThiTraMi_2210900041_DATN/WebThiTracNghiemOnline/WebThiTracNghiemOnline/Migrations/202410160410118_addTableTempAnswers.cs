namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableTempAnswers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TempAnswers",
                c => new
                    {
                        TEMPANSWERID = c.Int(nullable: false, identity: true),
                        EXAMID = c.Int(nullable: false),
                        STUDENTID = c.Int(nullable: false),
                        QUESTIONID = c.Int(nullable: false),
                        ANSWERSID = c.Int(),
                        ISSUBMITTED = c.Boolean(nullable: false),
                        DATECREATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TEMPANSWERID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TempAnswers");
        }
    }
}

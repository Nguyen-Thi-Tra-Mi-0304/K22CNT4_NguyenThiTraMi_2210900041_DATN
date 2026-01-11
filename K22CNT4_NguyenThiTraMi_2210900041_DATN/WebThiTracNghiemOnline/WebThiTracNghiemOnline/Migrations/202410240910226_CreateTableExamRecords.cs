namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CreateTableExamRecords : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExamRecords",
                c => new
                {
                    RECORDID = c.Int(nullable: false, identity: true),
                    STUDENTID = c.Int(nullable: false),
                    EXAMID = c.Int(nullable: false),
                    STARTTIME = c.DateTime(nullable: false),
                    ENDTIME = c.DateTime(nullable: false),
                    ATTEMPTCOUNT = c.Int(nullable: false),
                    TOTALSCORE = c.Single(nullable: false),
                })
                .PrimaryKey(t => t.RECORDID);
        }

        public override void Down()
        {
            DropTable("dbo.ExamRecords");
        }
    }
}

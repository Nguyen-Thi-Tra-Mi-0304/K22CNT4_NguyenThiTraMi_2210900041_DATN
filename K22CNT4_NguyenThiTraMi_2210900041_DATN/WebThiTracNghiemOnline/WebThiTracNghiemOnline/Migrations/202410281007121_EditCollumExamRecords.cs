namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditCollumExamRecords : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ExamRecords", "TOTALSCORE", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ExamRecords", "TOTALSCORE", c => c.Single(nullable: false));
        }
    }
}

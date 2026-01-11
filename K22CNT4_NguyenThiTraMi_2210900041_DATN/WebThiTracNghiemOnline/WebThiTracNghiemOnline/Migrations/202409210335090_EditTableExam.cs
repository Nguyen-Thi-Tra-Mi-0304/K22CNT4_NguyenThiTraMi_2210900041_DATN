namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditTableExam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Exams", "MIXQUESTION", c => c.Int(nullable: false));
            AddColumn("dbo.Exams", "MIXANSWERS", c => c.Int(nullable: false));
            AddColumn("dbo.Exams", "SHOWPOINT", c => c.Int(nullable: false));
            AddColumn("dbo.Exams", "SHOWANSWERS", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Exams", "SHOWANSWERS");
            DropColumn("dbo.Exams", "SHOWPOINT");
            DropColumn("dbo.Exams", "MIXANSWERS");
            DropColumn("dbo.Exams", "MIXQUESTION");
        }
    }
}

namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditCollumToTableExam : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Exams", "MIXQUESTION", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Exams", "MIXANSWERS", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Exams", "SHOWPOINT", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Exams", "SHOWANSWERS", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Exams", "SHOWANSWERS", c => c.Int(nullable: false));
            AlterColumn("dbo.Exams", "SHOWPOINT", c => c.Int(nullable: false));
            AlterColumn("dbo.Exams", "MIXANSWERS", c => c.Int(nullable: false));
            AlterColumn("dbo.Exams", "MIXQUESTION", c => c.Int(nullable: false));
        }
    }
}

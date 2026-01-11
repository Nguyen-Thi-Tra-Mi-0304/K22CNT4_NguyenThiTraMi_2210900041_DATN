namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollumToExamRecords : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExamRecords", "ISCOMPLETED", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExamRecords", "ISCOMPLETED");
        }
    }
}

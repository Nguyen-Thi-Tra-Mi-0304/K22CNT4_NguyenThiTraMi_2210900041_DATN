namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditCollumTableExamss : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Exams", "IDSUBJECT", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Exams", "IDSUBJECT", c => c.String());
        }
    }
}

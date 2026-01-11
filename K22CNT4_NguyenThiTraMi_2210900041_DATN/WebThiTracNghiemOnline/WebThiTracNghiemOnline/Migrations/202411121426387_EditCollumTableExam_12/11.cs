namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditCollumTableExam_1211 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Exams", "IDSUBJECT", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Exams", "IDSUBJECT", c => c.String());
        }
    }
}

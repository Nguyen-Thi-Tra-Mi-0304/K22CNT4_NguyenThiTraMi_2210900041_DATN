namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditTableSemester : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Semesters", "CREATEAT", c => c.DateTime());
            AddColumn("dbo.Semesters", "STT", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Semesters", "STT");
            DropColumn("dbo.Semesters", "CREATEAT");
        }
    }
}

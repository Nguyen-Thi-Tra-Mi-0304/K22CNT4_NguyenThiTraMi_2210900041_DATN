namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSttToQuestion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "STT", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "STT");
        }
    }
}

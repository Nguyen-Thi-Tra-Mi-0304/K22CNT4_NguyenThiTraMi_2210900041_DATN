namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollumToTempAnswers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempAnswers", "ATTEMPTCOUNT", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempAnswers", "ATTEMPTCOUNT");
        }
    }
}

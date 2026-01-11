namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollumAnswersText : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempAnswers", "ANSWERSTEXT", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempAnswers", "ANSWERSTEXT");
        }
    }
}

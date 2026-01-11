namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addCollumOnTableQuestions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "IDCHAPTER", c => c.Int(nullable: false));
            AddColumn("dbo.Questions", "CREATEAT", c => c.DateTime(nullable: false));
            AddColumn("dbo.Questions", "CREATEUPDATE", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "CREATEUPDATE");
            DropColumn("dbo.Questions", "CREATEAT");
            DropColumn("dbo.Questions", "IDCHAPTER");
        }
    }
}

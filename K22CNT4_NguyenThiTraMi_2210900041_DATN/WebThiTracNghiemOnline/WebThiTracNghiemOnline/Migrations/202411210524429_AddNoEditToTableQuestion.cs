namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNoEditToTableQuestion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "NOEDIT", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "NOEDIT");
        }
    }
}

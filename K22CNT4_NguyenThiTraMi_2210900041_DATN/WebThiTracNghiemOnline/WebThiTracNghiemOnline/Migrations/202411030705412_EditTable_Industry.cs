namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditTable_Industry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Industries", "CODE", c => c.String());
            AddColumn("dbo.Industries", "DISCRIBLR", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Industries", "DISCRIBLR");
            DropColumn("dbo.Industries", "CODE");
        }
    }
}

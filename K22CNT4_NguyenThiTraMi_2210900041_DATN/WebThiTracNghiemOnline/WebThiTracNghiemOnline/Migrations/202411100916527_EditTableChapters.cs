namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditTableChapters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chapters", "IDTEACHER", c => c.Int(nullable: false));
            AlterColumn("dbo.Chapters", "IDSUBJECT", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Chapters", "IDSUBJECT", c => c.String());
            DropColumn("dbo.Chapters", "IDTEACHER");
        }
    }
}

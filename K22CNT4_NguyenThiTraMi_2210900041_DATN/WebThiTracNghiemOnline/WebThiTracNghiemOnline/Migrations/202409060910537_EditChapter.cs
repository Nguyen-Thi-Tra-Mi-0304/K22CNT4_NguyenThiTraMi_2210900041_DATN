namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditChapter : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Chapters", "IDSUBJECT", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Chapters", "IDSUBJECT", c => c.Int(nullable: false));
        }
    }
}

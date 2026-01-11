namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditTableChapterv2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CHAPTER",
                c => new
                {
                    IDCHAPTER = c.Int(nullable: false, identity: true),
                    IDSUBBYTEACHER = c.Int(nullable: false),
                    NAMECHAPTER = c.String(),
                    CREATEAT = c.DateTime(nullable: false),
                    STT = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.IDCHAPTER);
        }

        public override void Down()
        {
            DropTable("dbo.CHAPTER");
        }
    }
}

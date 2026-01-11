namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateData : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Subjects");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IDSEMESTER = c.Int(nullable: false),
                        CODE = c.String(),
                        NAMESUBJECT = c.String(),
                        TINCHI = c.Int(nullable: false),
                        DESCRIBE = c.String(),
                        STT = c.Int(nullable: false),
                        CREATEAT = c.DateTime(nullable: false),
                        CREATEUPDATE = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
    }
}

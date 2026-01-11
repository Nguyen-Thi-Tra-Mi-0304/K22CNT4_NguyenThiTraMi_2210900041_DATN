namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTableSubJect : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IDSEMESTER = c.Int(),
                        CODE = c.String(),
                        NAMESUBJECT = c.String(),
                        TINCHI = c.Int(),
                        DESCRIBE = c.String(),
                        STT = c.Int(),
                        CREATEAT = c.DateTime(nullable: false),
                        CREATEUPDATE = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Subjects");
        }
    }
}

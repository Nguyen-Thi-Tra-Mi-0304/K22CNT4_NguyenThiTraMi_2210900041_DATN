namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCollumTokenxpiry : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AccountStudents", "TOKENEXPIRY", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AccountStudents", "TOKENEXPIRY", c => c.DateTime(nullable: false));
        }
    }
}

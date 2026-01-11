namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollumToTableAccountStudent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountStudents", "CURRENTSESSIONID", c => c.String());
            AddColumn("dbo.AccountStudents", "LASTLOGINTIME", c => c.DateTime());
            AddColumn("dbo.AccountStudents", "ISLOGIN", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountStudents", "ISLOGIN");
            DropColumn("dbo.AccountStudents", "LASTLOGINTIME");
            DropColumn("dbo.AccountStudents", "CURRENTSESSIONID");
        }
    }
}

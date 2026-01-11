namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollumToTableAccountStudent1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountStudents", "PASSWORDRESERTTOKEN", c => c.String());
            AddColumn("dbo.AccountStudents", "TOKENEXPIRY", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountStudents", "TOKENEXPIRY");
            DropColumn("dbo.AccountStudents", "PASSWORDRESERTTOKEN");
        }
    }
}

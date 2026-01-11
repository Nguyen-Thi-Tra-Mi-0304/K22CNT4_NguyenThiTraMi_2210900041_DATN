namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCollumToTableAccountStudents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AccountStudents", "IMAGE", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AccountStudents", "IMAGE");
        }
    }
}

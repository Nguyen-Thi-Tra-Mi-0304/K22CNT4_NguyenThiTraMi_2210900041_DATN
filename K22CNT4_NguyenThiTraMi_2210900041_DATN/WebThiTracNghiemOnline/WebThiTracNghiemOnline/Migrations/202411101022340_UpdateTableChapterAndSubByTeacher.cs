namespace WebThiTracNghiemOnline.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateTableChapterAndSubByTeacher : DbMigration
    {
        public override void Up()
        {
            // Xóa cột khóa chính hiện tại trước khi thêm cột mới với identity
            DropPrimaryKey("dbo.Chapters");

            // Xóa cột ID ban đầu
            DropColumn("dbo.Chapters", "ID");

            // Thêm cột IDCHAPTER làm cột identity mới
            AddColumn("dbo.Chapters", "IDCHAPTER", c => c.Int(nullable: false, identity: true));

            // Thêm cột IDSUBBYTEACHER vào bảng Chapters
            AddColumn("dbo.Chapters", "IDSUBBYTEACHER", c => c.Int(nullable: false));

            // Đặt khóa chính mới cho bảng Chapters là IDCHAPTER
            AddPrimaryKey("dbo.Chapters", "IDCHAPTER");

            // Tạo bảng SubjectByTeachers
            CreateTable(
                "dbo.SubjectByTeachers",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    IDSUBJECT = c.Int(nullable: false),
                    IDTEACHER = c.Int(nullable: false),
                    CREATEAT = c.DateTime(nullable: false),
                    STT = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            // Khôi phục lại các thay đổi nếu cần rollback
            DropPrimaryKey("dbo.Chapters");
            DropColumn("dbo.Chapters", "IDSUBBYTEACHER");
            DropColumn("dbo.Chapters", "IDCHAPTER");

            AddColumn("dbo.Chapters", "ID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Chapters", "IDSUBJECT", c => c.Int(nullable: false));
            AddColumn("dbo.Chapters", "IDTEACHER", c => c.Int(nullable: false));

            AddPrimaryKey("dbo.Chapters", "ID");

            DropTable("dbo.SubjectByTeachers");
        }
    }

}

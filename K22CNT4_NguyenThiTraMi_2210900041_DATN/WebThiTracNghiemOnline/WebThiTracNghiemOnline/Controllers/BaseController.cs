using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Context;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Controllers
{
    public class BaseController : Controller
    {
        protected WebsiteTracNghiemDBContext db;

        public BaseController()
        {
            // Khởi tạo DbContext từ chuỗi kết nối
            db = new WebsiteTracNghiemDBContext();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra xem Session có User không
            if (Session["User"] == null)
            {
                filterContext.Result = RedirectToAction("LoginUser", "Login");
                return;
            }

            // Lấy thông tin user từ session
            var sessionUser = Session["User"] as AccountStudents;

            // Lấy thông tin user từ database
            var dbUser = db.ACCOUNTSTUDENTS.FirstOrDefault(u => u.ACCOUNT == sessionUser.ACCOUNT);

            // Kiểm tra user từ session có khớp với user trong cơ sở dữ liệu
            if (dbUser == null || dbUser.PASS != sessionUser.PASS)
            {
                Session.Clear();  // Xóa session nếu không khớp
                filterContext.Result = RedirectToAction("LoginUser", "Login");
            }

            base.OnActionExecuting(filterContext);
        }

        // Override Dispose để giải phóng tài nguyên
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
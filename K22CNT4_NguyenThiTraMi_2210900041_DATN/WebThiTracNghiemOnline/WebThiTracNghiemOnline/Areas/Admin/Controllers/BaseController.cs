using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Context;
using WebThiTracNghiemOnline.Models; 

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected WebsiteTracNghiemDBContext db;

        public BaseController()
        {
            // Khởi tạo DbContext từ chuỗi kết nối
            db = new WebsiteTracNghiemDBContext();
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

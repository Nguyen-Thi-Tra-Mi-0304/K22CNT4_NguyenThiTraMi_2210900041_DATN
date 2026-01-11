using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Areas.Admin.Data;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    [AuthorizeRole("Admin", "Teacher")]
    public class HomePageController : BaseController
    {
        // GET: Admin/HomePage
        public ActionResult HomePage()
        {
            var countTeacher = db.ACCOUNTUSER.Count();
            var countStudent = db.ACCOUNTSTUDENTS.Count();
            var countExam = db.EXAM.Count();
            var countClass = db.CLASS.Count();
            var exam = db.EXAM.OrderByDescending(n => n.CREATEAT).Take(6).ToList();
            var clas = db.CLASS.OrderByDescending(n => n.CREATEAT).Take(6).ToList();
            ViewBag.Teacher = countTeacher;
            ViewBag.Student = countStudent;
            ViewBag.ExamCount = countExam;
            ViewBag.ClassCount = countClass;
            ViewBag.Exam = exam;
            ViewBag.Class = clas;
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            try
            {
                var user = Session["UserName"] as AccountUser;
                if (user == null)
                {
                    ViewBag.ErrorMessage = "Bạn cần đăng nhập trước khi thực hiện chức năng này.";
                    return View();
                }
                var dbUser = await db.ACCOUNTUSER.FirstOrDefaultAsync(u => u.ID == user.ID);
                if (dbUser == null || !BCrypt.Net.BCrypt.Verify(oldPassword, dbUser.PASS))
                {
                    ViewBag.ErrorMessage = "Mật khẩu hiện tại không chính xác.";
                    return View();
                }
                if (newPassword != confirmPassword)
                {
                    ViewBag.ErrorMessage = "Mật khẩu mới và mật khẩu nhập lại không khớp.";
                    return View();
                }
                if (newPassword.Length < 6)
                {
                    ViewBag.ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                    return View();
                }
                dbUser.PASS = BCrypt.Net.BCrypt.HashPassword(newPassword);
                db.Entry(dbUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                ViewBag.Message = "Đặt lại mật khẩu thành công.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi đặt lại mật khẩu: " + ex.Message;
                return View();
            }
        }
    }

}
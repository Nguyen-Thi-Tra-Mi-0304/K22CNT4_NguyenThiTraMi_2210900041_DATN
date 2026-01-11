using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using WebThiTracNghiemOnline.Context;
using System.Web.Helpers;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        WebsiteTracNghiemDBContext db = new WebsiteTracNghiemDBContext();
        [Route("user-login")]
        public ActionResult LoginUser()
        {
            return View();
        }
        public ActionResult KeepSessionAliveUser()
        {
            return Json(new { success = true, message = "Session is active" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login_AccountUser(string Username, string Password)
        {
            var user = await db.ACCOUNTSTUDENTS.FirstOrDefaultAsync(n => n.ACCOUNT == Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(Password, user.PASS))
            {
                // Kiểm tra nếu tài khoản đã đăng nhập trong một phiên khác
                if (user.ISLOGIN && user.CURRENTSESSIONID != null)
                {
                    user.ISLOGIN = false;
                    user.CURRENTSESSIONID = null;
                    await db.SaveChangesAsync(); 
                }
                user.ISLOGIN = true;
                user.CURRENTSESSIONID = System.Web.HttpContext.Current.Session.SessionID;
                user.LASTLOGINTIME = DateTime.Now;

                await db.SaveChangesAsync();

                // Thiết lập cookie và Session cho phiên đăng nhập mới
                FormsAuthentication.SetAuthCookie(Username, false);
                Session["User"] = user;

                return RedirectToAction("ViewIndex", "Home");
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View("LoginUser");
        }

        public ActionResult Logout_AccountUser()
        {
            if (Session["User"] is AccountStudents user)
            {
                var dbUser = db.ACCOUNTSTUDENTS.FirstOrDefault(u => u.ID == user.ID);
                if (dbUser != null)
                {
                    dbUser.ISLOGIN = false;
                    dbUser.CURRENTSESSIONID = null;
                    db.SaveChanges();
                }
            }
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("LoginUser", "Login");
        }

        [Route("forgot-passworduser")]
        public  ActionResult Forgot_Password()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("forgot-passworduser")]
        public async Task<ActionResult> Forgot_Password(string Username, string Email)
        {
            // Tìm người dùng theo tài khoản và email
            var user = await db.ACCOUNTSTUDENTS.FirstOrDefaultAsync(n => n.ACCOUNT == Username && n.EMAIL == Email);
            if (user != null)
            {
                // Tạo token xác nhận
                var token = Guid.NewGuid().ToString();

                // Lưu token vào cơ sở dữ liệu hoặc bộ nhớ tạm
                user.PASSWORDRESERTTOKEN = token;
                user.TOKENEXPIRY = DateTime.Now.AddHours(1); // Token hết hạn sau 1 giờ
                await db.SaveChangesAsync();

                // Tạo URL đặt lại mật khẩu
                var resetLink = Url.Action("Reset_Password", "Login", new { token = token }, protocol: Request.Url.Scheme);

                // Tạo tiêu đề email
                string subject = "Yêu cầu đặt lại mật khẩu";

                // Đọc nội dung HTML từ tệp mẫu và thay thế các tham số động
                string templatePath = Server.MapPath("~/Templates/ForgotPasswordTemplate.html");
                string emailBody = System.IO.File.ReadAllText(templatePath)
                    .Replace("{{UserName}}", user.ACCOUNT)
                    .Replace("{{ResetLink}}", resetLink);

                try
                {
                    // Gửi email
                    await EmailService.SendEmailAsync(Email, subject, emailBody);
                    ViewBag.Message = "Email đặt lại mật khẩu đã được gửi đến địa chỉ email của bạn.";
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi khi gửi email
                    ViewBag.ErrorMessage = "Đã xảy ra lỗi khi gửi email. Vui lòng thử lại sau.";
                }
            }
            else
            {
                // Không tìm thấy tài khoản hoặc email không hợp lệ
                ViewBag.ErrorMessage = "Không tìm thấy tài khoản hoặc email không hợp lệ.";
            }

            return View();
        }


        [HttpGet]
        [Route("reset-password")]
        public ActionResult Reset_Password(string token)
        {
            var user = db.ACCOUNTSTUDENTS.FirstOrDefault(n => n.PASSWORDRESERTTOKEN == token && n.TOKENEXPIRY > DateTime.Now);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Token không hợp lệ hoặc đã hết hạn.";
                return View("Forgot_Password");
            }

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("reset-password")]
        public async Task<ActionResult> Reset_Password(string token, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu không khớp.";
                return View();
            }

            var user = db.ACCOUNTSTUDENTS.FirstOrDefault(n => n.PASSWORDRESERTTOKEN == token && n.TOKENEXPIRY > DateTime.Now);

            if (user != null)
            {
                user.PASS = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PASSWORDRESERTTOKEN = null; 
                user.TOKENEXPIRY = null;
                await db.SaveChangesAsync();

                ViewBag.Message = "Mật khẩu của bạn đã được đặt lại thành công.";
                return RedirectToAction("LoginUser");
            }
            else
            {
                ViewBag.ErrorMessage = "Token không hợp lệ hoặc đã hết hạn.";
            }

            return View();
        }

    }
}
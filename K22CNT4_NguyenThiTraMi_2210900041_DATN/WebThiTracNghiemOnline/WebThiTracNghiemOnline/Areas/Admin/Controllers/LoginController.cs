using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Admin/Login
        public ActionResult Login()
        {
            if (Session["UserRole"] != null)
            {
                return RedirectToAction("HomePage", "HomePage");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login_Account(string username, string password)
        {
            var tk = db.ACCOUNTUSER.FirstOrDefault(n => n.ACCOUNT == username);

            if (tk != null && BCrypt.Net.BCrypt.Verify(password, tk.PASS))
            {
                FormsAuthentication.SetAuthCookie(username, false);

                Session["UserName"] = tk;
                Session["UserRole"] = tk.ROLE == 1 ? "Admin" : "Teacher";

                return RedirectToAction("HomePage", "HomePage");
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear(); 
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult KeepSessionAlive()
        {
            return Json(new { success = true, message = "Session is active" }, JsonRequestBehavior.AllowGet);
        }


    }
}
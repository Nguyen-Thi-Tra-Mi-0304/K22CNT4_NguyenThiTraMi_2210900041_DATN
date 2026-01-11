
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Areas.Admin.Data;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    [AuthorizeRole("Admin")]
    public class AccountUserController : BaseController
    {
        // GET: Admin/AccountUser

        #region Admin

        public async Task<ActionResult> ListAccoutAdmin()
        {
            var listrole = db.PRIVILEGE.Select(i => new { ID = i.ID, Name = i.NAME }).ToList();
            var list = await db.ACCOUNTUSER.Where(n => n.ROLE == 2 || n.ROLE == 1 && n.STT == 1 ).OrderByDescending(n => n.CREATEAT).ToListAsync();
            ViewBag.List = new SelectList(listrole, "ID", "Name");
            return View(list);
        }

        //Tạo giảng viên

        public ActionResult CreateTeacher()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateTeacher(HttpPostedFileBase imgFile, FormCollection f)
        {
            try
            {
                string email = f["EMAIL"];
                var existingUser = db.ACCOUNTUSER.FirstOrDefault(u => u.EMAIL == email);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Email đã tồn tại. Vui lòng sử dụng email khác.";
                    return View();
                }

                AccountUser user = new AccountUser();
                var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                user.NAMEUSER = f["NAMEUSER"];
                user.ACCOUNT = f["ACCOUNT"];
                user.PASS = BCrypt.Net.BCrypt.HashPassword(f["PASS"]);

                if (imgFile != null && imgFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName) + time + Path.GetExtension(imgFile.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Areas/Admin/Data/imgadmin/"), fileName);
                    imgFile.SaveAs(filePath);
                    user.IMG = "~/Areas/Admin/Data/imgadmin/" + fileName;
                }
                else
                {
                    user.IMG = "";
                }

                user.SEX = f["SEX"];
                user.EMAIL = email;
                user.ROLE = 2;
                user.CREATEAT = DateTime.Now;
                user.STT = 1;

                db.ACCOUNTUSER.Add(user);
                db.SaveChanges();

                return RedirectToAction("ListAccoutAdmin");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        // Chỉnh sửa thông tin giảng viên


        public ActionResult EditTeacher(int id)
        {
            var listrole = db.PRIVILEGE.Select(i => new { ID = i.ID, Name = i.NAME }).ToList();
            var editgv = db.ACCOUNTUSER.Where(x => x.ID == id).SingleOrDefault();
            ViewBag.List = new SelectList(listrole, "ID", "Name");
            return View(editgv);
        }
        [HttpPost]
        public ActionResult EditTeacher(HttpPostedFileBase imgFile, FormCollection f, int id)
        {
            try
            {
                var editgv = db.ACCOUNTUSER.SingleOrDefault(x => x.ID == id);
                var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (editgv != null)
                {
                    editgv.NAMEUSER = f["NAMEUSER"];
                    editgv.ACCOUNT = f["ACCOUNT"];
                    // Mã hóa mật khẩu mới nếu có, nếu không giữ nguyên mật khẩu cũ
                    if (f["PASS"] == "********")
                    {
                        editgv.PASS = f["CurrentPASS"]; // Giữ nguyên mật khẩu cũ
                    }
                    else
                    {
                        editgv.PASS = BCrypt.Net.BCrypt.HashPassword(f["PASS"]);
                    }

                    if (imgFile != null && imgFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(imgFile.FileName) + time + Path.GetExtension(imgFile.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Areas/Admin/Data/imgadmin/"), fileName);
                        imgFile.SaveAs(filePath);
                        editgv.IMG = "~/Areas/Admin/Data/imgadmin/" + fileName;
                    }
                    else
                    {
                        editgv.IMG = f["CurrentIMG"]; // Giữ nguyên đường dẫn ảnh cũ
                    }
                    editgv.SEX = f["SEX"];
                    editgv.EMAIL = f["EMAIL"];
                    editgv.ROLE = int.Parse(f["ROLE"]);
                    editgv.STT = 1;
                    db.SaveChanges();
                    return RedirectToAction("ListAccoutAdmin");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }


        [HttpGet]
        public ActionResult DetailsTeacher(int id)
        {
            var detail = db.ACCOUNTUSER.Where(n => n.ID == id).SingleOrDefault();
            return View(detail);
        }
        #endregion


        [HttpPost]
        public JsonResult DeleteTeacher(int id)
        {
            try
            {
                var delete = db.ACCOUNTUSER.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0; // Cập nhật trạng thái để ẩn người dùng
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa người dùng thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa người dùng!" + ex });
            }
        }


        [HttpPost]
        public async Task<ActionResult> ImportExcel(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn file để upload." });
            }

            try
            {
                // Đọc file Excel
                using (var package = new ExcelPackage(excelFile.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "File Excel không có nội dung." });
                    }

                    var newUsers = new List<AccountUser>();
                    var existingEmails =await db.ACCOUNTUSER.Select(u => u.EMAIL).ToListAsync(); 

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        // Kiểm tra nếu tất cả các ô trong dòng đều trống
                        bool isRowEmpty = true;
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }

                        if (isRowEmpty)
                        {
                            continue; // Bỏ qua dòng này nếu tất cả ô đều trống
                        }

                        var email = worksheet.Cells[row, 7].Text; 
                        if (existingEmails.Contains(email))
                        {
                            continue; // Bỏ qua dòng này nếu email đã tồn tại
                        }

                        var user = new AccountUser
                        {
                            NAMEUSER = worksheet.Cells[row, 2].Text,
                            ACCOUNT = worksheet.Cells[row, 3].Text,
                            PASS = BCrypt.Net.BCrypt.HashPassword(worksheet.Cells[row, 4].Text), // Hash mật khẩu
                            IMG = worksheet.Cells[row, 5].Text,
                            SEX = worksheet.Cells[row, 6].Text,
                            EMAIL = email,
                            ROLE = 2,
                            CREATEAT = DateTime.Now,
                            STT = 1
                        };

                        newUsers.Add(user);
                    }
                    if (newUsers.Count > 0)
                    {
                        db.ACCOUNTUSER.AddRange(newUsers);
                        await db.SaveChangesAsync();
                    }

                    return Json(new { success = true, message = "Nhập file Excel thành công!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý file: " + ex.Message });
            }
        }



        #region Student
        public ActionResult ListStudent()
        {
            var listIndustry = db.INDUSTRY.Select(i => new { ID = i.ID, Name = i.CODE }).ToList();
            var listCourse = db.COURSE.Select(i => new { ID = i.ID, Name = i.NAMECOURSE }).ToList();
            ViewBag.Industry = new SelectList(listIndustry, "ID", "Name");
            ViewBag.Course = new SelectList(listCourse, "ID", "Name");
            var students = db.ACCOUNTSTUDENTS
                   .Where(n => n.ROLE == 3 && n.STT == 1)   
                   .OrderByDescending(n => n.CREATEAT)
                   .ToList();

            return View(students);
        }

        public ActionResult CreateStudent()
        {
            LoadSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStudent(AccountStudents std)
        {
            try
            {
                bool accountExists = db.ACCOUNTSTUDENTS.Any(n => n.ACCOUNT == std.ACCOUNT || n.EMAIL == std.EMAIL);
                if (accountExists)
                {
                    ModelState.AddModelError("", "Tài khoản hoặc email đã tồn tại. Vui lòng sử dụng tên tài khoản hoặc email khác.");
                    LoadSelectLists();
                    return View(std);
                }
                std.PASS = BCrypt.Net.BCrypt.HashPassword(std.PASS);
                std.CREATEAT = DateTime.Now;
                std.CREATEUPDATE = DateTime.Now;
                std.ROLE = 3; 
                std.STT = 1; 
                std.ISLOGIN = false;
                std.LASTLOGINTIME= DateTime.Now;
                db.ACCOUNTSTUDENTS.Add(std);
                db.SaveChanges();

                return RedirectToAction("ListStudent");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình tạo sinh viên: " + ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
            }

            LoadSelectLists();
            return View(std);
        }
        private void LoadSelectLists()
        {
            ViewBag.Privilege = new SelectList(db.PRIVILEGE.Select(i => new { ID = i.ID, Name = i.NAME }).ToList(), "ID", "Name");
            ViewBag.Industry = new SelectList(db.INDUSTRY.Where(n => n.STT == 1).Select(i => new { ID = i.ID, Name = i.NAMEINDUSTRY }).ToList(), "ID", "Name");
            ViewBag.Course = new SelectList(db.COURSE.Where(n => n.STT == 1).Select(i => new { ID = i.ID, Name = i.NAMECOURSE }).ToList(), "ID", "Name");
        }

        public ActionResult EditStudent(int id)
        {
            LoadSelectLists();
            var editstd = db.ACCOUNTSTUDENTS.Where(n => n.ID == id).FirstOrDefault();
            return View(editstd);
        }
        [HttpPost]
        public ActionResult EditStudent(int id, FormCollection f)
        {
            try
            {
                var editstd = db.ACCOUNTSTUDENTS.SingleOrDefault(x => x.ID == id);
                var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (editstd != null)
                {
                    editstd.IDINDUSTRY = int.Parse(f["IDINDUSTRY"]);
                    editstd.IDCOURSE = int.Parse(f["IDCOURSE"]);
                    editstd.NAMESTUDENT = (f["NAMESTUDENT"]);
                    editstd.ACCOUNT = f["ACCOUNT"];
                    if (f["PASS"] == "********")
                    {
                        editstd.PASS = f["CurrentPASS"]; // Giữ nguyên mật khẩu cũ
                    }
                    else
                    {
                        editstd.PASS = BCrypt.Net.BCrypt.HashPassword(f["PASS"]);
                    }
                    editstd.SEX = f["SEX"];
                    editstd.EMAIL = f["EMAIL"];
                    editstd.BIRTHDAY =DateTime.Parse(f["BIRTHDAY"]);
                    editstd.ROLE = int.Parse(f["ROLE"]);
                    db.SaveChanges();
                    return RedirectToAction("ListStudent");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        [HttpGet]
        public ActionResult DetailsStudent(int id)
        {
            var std = db.ACCOUNTSTUDENTS.Where(n => n.ID == id).FirstOrDefault();
            return View(std);
        }

        [HttpPost]
        public async Task<ActionResult> ImportExcelStudent(HttpPostedFileBase excelFile1)
        {
            if (excelFile1 == null || excelFile1.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn file để upload." });
            }

            try
            {
                // Đọc file Excel
                using (var package = new ExcelPackage(excelFile1.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "File Excel không có nội dung." });
                    }

                    var newStudent = new List<AccountStudents>();
                    var existingEmails = await db.ACCOUNTSTUDENTS.Select(u => u.EMAIL).ToListAsync();
                    var existingAccounts = await db.ACCOUNTSTUDENTS.Select(u => u.ACCOUNT).ToListAsync();

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        // Kiểm tra nếu tất cả các ô trong dòng đều trống
                        bool isRowEmpty = true;
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                            {
                                isRowEmpty = false;
                                break;
                            }
                        }

                        if (isRowEmpty)
                        {
                            continue; // Bỏ qua dòng này nếu tất cả ô đều trống
                        }

                        var email = worksheet.Cells[row, 5].Text.Trim();
                        var acc = worksheet.Cells[row, 3].Text.Trim();
                        var industryName = worksheet.Cells[row, 8].Text.Trim();
                        var courseName = worksheet.Cells[row, 9].Text.Trim();

                        // Kiểm tra nếu email hoặc tài khoản đã tồn tại
                        if (existingEmails.Contains(email) || existingAccounts.Contains(acc))
                        {
                            continue;
                        }

                        var industryId = await db.INDUSTRY
                            .Where(n => n.CODE == industryName)
                            .Select(n => n.ID)
                            .FirstOrDefaultAsync();

                        var courseId = await db.COURSE
                            .Where(n => n.NAMECOURSE == courseName)
                            .Select(n => n.ID)
                            .FirstOrDefaultAsync();

                        var birthdayText = worksheet.Cells[row, 6].Text.Trim();
                        DateTime? birthday = null;

                        if (!string.IsNullOrEmpty(birthdayText))
                        {
                            if (DateTime.TryParseExact(birthdayText, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                            {
                                birthday = parsedDate;
                            }
                            else
                            {
                                return Json(new { success = false, message = $"Ngày sinh ở dòng {row} không đúng định dạng (d/M/yyyy)." });
                            }
                        }

                        var user = new AccountStudents
                        {
                            NAMESTUDENT = worksheet.Cells[row, 2].Text.Trim(),
                            ACCOUNT = acc,
                            PASS = BCrypt.Net.BCrypt.HashPassword(worksheet.Cells[row, 4].Text.Trim()), 
                            EMAIL = email,
                            BIRTHDAY = birthday ?? DateTime.MinValue,
                            SEX = worksheet.Cells[row, 7].Text.Trim(),
                            ROLE = 3,
                            CREATEAT = DateTime.Now,
                            CREATEUPDATE = DateTime.Now,
                            ISLOGIN = false,
                            LASTLOGINTIME = DateTime.Now,
                            STT = 1,
                            IDINDUSTRY = industryId,
                            IDCOURSE = courseId 
                        };

                        newStudent.Add(user);
                    }
                    if (newStudent.Count > 0)
                    {
                        db.ACCOUNTSTUDENTS.AddRange(newStudent);
                        await db.SaveChangesAsync();
                    }

                    return Json(new { success = true, message = "Nhập file Excel thành công!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý file: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteStudent(int id)
        {
            try
            {
                var delete = db.ACCOUNTSTUDENTS.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0; // Cập nhật trạng thái để ẩn người dùng
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa người dùng thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa người dùng!" + ex });
            }
        }
        #endregion
    }
}
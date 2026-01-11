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
    [AuthorizeRole("Admin")]
    public class SubjectController : BaseController
    {
        // GET: Admin/Subject
        private void GetSemester()
        {
            var listsemeter = db.SEMESTER
                                .Where(n => n.STT == 1)
                                .Select(s => new { ID = s.ID, Name = s.NAMESEMESTER })
                                .ToList();
            ViewBag.Semester = new SelectList(listsemeter, "ID", "Name");
        }

        public async Task<ActionResult> ListSubject()
        {
            GetSemester();
            var subject = await db.SUBJECT.Where(n => n.STT == 1).OrderByDescending(n=>n.CREATEAT).ToListAsync();
            return View(subject);
        }

        public ActionResult CreateSubject()
        {
            GetSemester();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubject(FormCollection f)
        {
            try
            {
                string code = f["CODE"];
                var existingSubject = db.SUBJECT.FirstOrDefault(s => s.CODE == code);
                var sub = db.SUBJECT.Count();
                if (existingSubject != null)
                {
                    ViewBag.ErrorMessage = "Mã môn học đã tồn tại. Vui lòng sử dụng mã môn học khác.";
                    return View();
                }

                Subject sj = new Subject();
                sj.CODE = f["CODE"];
                sj.NAMESUBJECT = f["NAMESUBJECT"];
                sj.DESCRIBE = f["DESCRIBE"];
                sj.TINCHI = int.Parse(f["TINCHI"]);
                sj.IDSEMESTER = int.Parse(f["IDSEMESTER"]);
                sj.STT = 1;
                sj.CREATEAT = DateTime.Now;
                sj.CREATEUPDATE = DateTime.Now;
                db.SUBJECT.Add(sj);
                db.SaveChanges();

                return RedirectToAction("ListSubject");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
            return View();
        }

        public ActionResult EditSubject (int id)
        {
            var edit = db.SUBJECT.FirstOrDefault(s => s.ID == id);
            GetSemester();
            return View(edit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSubject(int id, FormCollection f)
        {
            try
            {
                var subject = db.SUBJECT.FirstOrDefault(s => s.ID == id);
                string code = f["CODE"];
                var existingSubject = db.SUBJECT.FirstOrDefault(s => s.CODE == code && s.ID != id);
                if (existingSubject != null)
                {
                    ViewBag.ErrorMessage = "Mã môn học đã tồn tại. Vui lòng sử dụng mã môn học khác.";
                    return View(subject);
                }

                subject.CODE = code;
                subject.NAMESUBJECT = f["NAMESUBJECT"];
                subject.DESCRIBE = f["DESCRIBE"];
                subject.TINCHI = int.Parse(f["TINCHI"]);
                subject.IDSEMESTER = int.Parse(f["IDSEMESTER"]);
                subject.CREATEUPDATE = DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("ListSubject");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public JsonResult DeleteSubject(int id)
        {
            try
            {
                var delete = db.SUBJECT.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0; // Cập nhật trạng thái để ẩn người dùng
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa môn học thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Môn học  không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa môn học !" + ex });
            }
        }
    }
}
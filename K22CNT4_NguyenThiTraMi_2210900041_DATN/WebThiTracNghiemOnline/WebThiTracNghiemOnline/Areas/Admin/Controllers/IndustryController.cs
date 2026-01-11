using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Areas.Admin.Data;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    [AuthorizeRole("Admin")]
    public class IndustryController : BaseController
    {
        #region Ngành nghề
        public ActionResult ListIndustry()
        {
            var listindustry = db.INDUSTRY.Where(n => n.STT == 1).ToList();
            return View(listindustry);
        }


        public ActionResult CreateIndustry()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateIndustry(FormCollection f)
        {
            try
            {
                string code = f["CODE"];
                var listcode = db.INDUSTRY.FirstOrDefault(n => n.CODE == code);
                if (listcode != null)
                {
                    ViewBag.ErrorMessage = "Mã ngành nghề đã tồn tại. Vui lòng sử dụng mã ngành khác.";
                    return View();
                }
                Industry industry = new Industry();
                industry.NAMEINDUSTRY = f["NAMEINDUSTRY"];
                industry.CODE = code;
                industry.DISCRIBLR = f["DISCRIBLR"];
                industry.STT = 1;
                industry.CREATEAT = DateTime.Now;
                db.INDUSTRY.Add(industry);
                db.SaveChanges();

                return RedirectToAction("ListIndustry");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        public ActionResult EditIndustry(int id)
        {
            var a = db.INDUSTRY.FirstOrDefault(n => n.ID == id);
            return View(a);
        }
        [HttpPost]
        public ActionResult EditIndustry(int id , FormCollection f)
        {
            try
            {
                var a = db.INDUSTRY.FirstOrDefault(n => n.ID == id);
                a.CODE = f["CODE"];
                a.NAMEINDUSTRY = f["NAMEINDUSTRY"];
                a.DISCRIBLR = f["DISCRIBLR"];
                a.CREATEAT = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ListIndustry");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        public ActionResult Details(int id) 
        { 
            var details = db.INDUSTRY.FirstOrDefault(a => a.ID == id);
            return View(details);
        }

        [HttpPost]
        public JsonResult DeleteIndustry(int id)
        {
            try
            {
                var delete = db.INDUSTRY.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0; // Cập nhật trạng thái để ẩn người dùng
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa ngành nghề thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Ngành nghề không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa ngành nghề!" + ex });
            }
        }
        #endregion

        #region Khóa học
        public ActionResult ListCourse()
        {
            var listcoures = db.COURSE.Where(n => n.STT == 1).ToList();
            return View(listcoures);
        }

        public ActionResult CreateCourse()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCourse(FormCollection f)
        {
            try
            {
                string name = f["NAMECOURSE"];
                var checkname = db.COURSE.FirstOrDefault(n => n.NAMECOURSE == name);
                if (checkname != null)
                {
                    ViewBag.ErrorMessage = "Mã khóa đã tồn tại. Vui lòng sử dụng mã khóa khác.";
                    return View();
                }
                Course course = new Course();
                course.NAMECOURSE = name;
                course.DESCRIBLE = f["DESCRIBLE"];
                course.STT = 1;
                course.CREATEAT = DateTime.Now;
                db.COURSE.Add(course);
                db.SaveChanges();
                return RedirectToAction("ListCourse");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return View();
        }


        public ActionResult EditCourse(int id)
        {
            var edit = db.COURSE.FirstOrDefault(n => n.ID == id);
            return View(edit);
        }
        [HttpPost]
        public ActionResult EditCourse(int id , FormCollection f)
        {
            try
            {
                var edit = db.COURSE.FirstOrDefault(n => n.ID == id);
                edit.NAMECOURSE = f["NAMECOURSE"];
                edit.DESCRIBLE = f["DESCRIBLE"];
                edit.CREATEAT = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ListCourse");
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return View();
        }

        [HttpPost]
        public JsonResult DeleteCourse(int id)
        {
            try
            {
                var delete = db.COURSE.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0; // Cập nhật trạng thái để ẩn người dùng
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa khóa học thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Khóa học không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa khóa học!" + ex });
            }
        }
        #endregion

        #region Học kỳ
        public ActionResult ListSemester()
        {
            var listCourse = db.COURSE.Select(i => new { ID = i.ID, Name = i.NAMECOURSE}).ToList();
            var listsemester = db.SEMESTER.Where(n => n.STT == 1).ToList();
            ViewBag.Course = new SelectList(listCourse, "ID", "Name");
            return View(listsemester);
        }

        public ActionResult CreateSemester()
        {
            var listCourse = db.COURSE.Where(n => n.STT == 1).Select(i => new { ID = i.ID, Name = i.NAMECOURSE }).ToList();
            ViewBag.Course = new SelectList(listCourse, "ID", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateSemester(FormCollection f)
        {
            try
            {
                Semester semester = new Semester();
                semester.COURSEID = int.Parse(f["COURSEID"]);
                semester.NAMESEMESTER = f["NAMESEMESTER"];
                semester.STARTDAY = DateTime.Parse(f["STARTDAY"]);
                semester.ENDDAY = DateTime.Parse(f["ENDDAY"]);
                semester.CREATEAT = DateTime.Now;
                semester.STT = 1;

                db.SEMESTER.Add(semester);
                db.SaveChanges();
                return RedirectToAction("ListSemester");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View();
        }

        public ActionResult EditSemester(int id)
        {
            var edit = db.SEMESTER.FirstOrDefault(i => i.ID == id);
            var listCourse = db.COURSE.Where(n => n.STT == 1).Select(i => new { ID = i.ID, Name = i.NAMECOURSE }).ToList();
            ViewBag.Course = new SelectList(listCourse, "ID", "Name");
            return View(edit);
        }
        [HttpPost]
        public ActionResult EditSemester(int id , FormCollection f)
        {
            try
            {
                var edit = db.SEMESTER.FirstOrDefault(i => i.ID == id);
                edit.COURSEID = int.Parse(f["COURSEID"]);
                edit.NAMESEMESTER = f["NAMESEMESTER"];
                edit.STARTDAY = DateTime.Parse(f["STARTDAY"]);
                edit.ENDDAY = DateTime.Parse(f["ENDDAY"]);
                edit.CREATEAT = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ListSemester");
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return View();
        }


        [HttpPost]
        public JsonResult DeleteSemester(int id)
        {
            try
            {
                var delete = db.SEMESTER.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0; // Cập nhật trạng thái để ẩn người dùng
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa học kỳ thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Học kỳ không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa học kỳ!" + ex });
            }
        }

        #endregion
    }
}
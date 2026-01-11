using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Areas.Admin.Data;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    [AuthorizeRole("Teacher")]
    public class ChapterController : BaseController
    {

        public async Task<ActionResult> ListSubjectByIdTeacher()
        {
            var tk = Session["UserName"] as AccountUser;
            var createdSubjectIds = await db.SUBJECTBYTEACHER
                                   .Where(st => st.IDTEACHER == tk.ID && st.STT == 1)
                                   .Select(st => st.IDSUBJECT)
                                   .ToListAsync();
            // Lấy các môn học mà người dùng chưa tạo
            ViewBag.AvailableSubjects = await db.SUBJECT
                                                .Where(sub => sub.STT == 1 && !createdSubjectIds.Contains(sub.ID))
                                                .ToListAsync();
            var subjects = await (from s in db.SUBJECT
                                  join st in db.SUBJECTBYTEACHER on s.ID equals st.IDSUBJECT
                                  where st.IDTEACHER == tk.ID && st.STT == 1
                                  select new SubjectByTeacherViewModel{
                                        Id = st.ID,
                                        Stt = st.STT,
                                        CreateAt = st.CREATEAT,
                                        Code = s.CODE,
                                        Name = s.NAMESUBJECT
                                  }).ToListAsync();

            return View(subjects);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSubjectForTeacher(int subjectId)
        {
            var tk = Session["UserName"] as AccountUser;

            var newSubject = new SubjectByTeacher
            {
                IDSUBJECT = subjectId,
                IDTEACHER = tk.ID,
                CREATEAT = DateTime.Now,
                STT = 1 // Hoặc giá trị trạng thái phù hợp
            };

            db.SUBJECTBYTEACHER.Add(newSubject);
            await db.SaveChangesAsync();

            return RedirectToAction("ListSubjectByIdTeacher");
        }

        public async Task<ActionResult> ListChapters(int id)
        {
            var chapters = await db.CHAPTER
                                   .Where(c => c.IDSUBBYTEACHER == id && c.STT == 1)
                                   .OrderByDescending(c => c.CREATEAT)
                                   .ToListAsync();
            ViewBag.SubjectId = id;
            return View(chapters);
        }

        // Action để hiển thị form tạo chương mới
        public ActionResult CreateChapter(int id)
        {
            ViewBag.SubjectId = id;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateChapter(int id, FormCollection f)
        {
            try
            {
                var tk = Session["UserName"] as AccountUser;
                Chapters sb = new Chapters
                {
                    NAMECHAPTER = f["NAMECHAPTER"],
                    IDSUBBYTEACHER = id,
                    CREATEAT = DateTime.Now,
                    STT = 1
                };
                db.CHAPTER.Add(sb);
                await db.SaveChangesAsync();
                return RedirectToAction("ListChapters", new { id = id });
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> DeleteChapter(int id)
        {
            try
            {
                var delete = await db.CHAPTER.FirstOrDefaultAsync(n => n.IDCHAPTER == id);
                if (delete != null)
                {
                    delete.STT = 0; 
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa ngành chương thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Chương không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa chương !" + ex });
            }
        }

        [HttpPost]
        public JsonResult DeleteSubjectByTeacher(int id)
        {
            try
            {
                var delete = db.SUBJECTBYTEACHER.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa môn học thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Môn học không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa môn học!" + ex });
            }
        }

    }
}

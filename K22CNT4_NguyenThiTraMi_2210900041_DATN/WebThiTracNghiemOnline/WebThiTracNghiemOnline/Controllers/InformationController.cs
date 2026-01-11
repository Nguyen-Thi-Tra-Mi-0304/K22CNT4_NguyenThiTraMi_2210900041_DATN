using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Models;
using WebThiTracNghiemOnline.Models.ViewModels;


namespace WebThiTracNghiemOnline.Controllers
{
    public class InformationController : BaseController
    {
        // GET: Information
        [Route("info/listexam")]
        public async Task<ActionResult> ExamComplete(int page = 1)
        {
            var tk = Session["User"] as AccountStudents;
            int pageSize = 10;

            var examRecords = await db.EXAMRECORDS
                .Where(a => a.STUDENTID == tk.ID)
                .OrderByDescending(a => a.RECORDID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Khởi tạo danh sách ViewModel
            var examRecordViewModels = new List<ExamRecordViewModel>();

            foreach (var record in examRecords)
            {
                //var subjectName = await GetSubject(record.EXAMID);
                var examName = await GetExamDetails(record.EXAMID);
                var className = await GetClass(record.EXAMID);

                examRecordViewModels.Add(new ExamRecordViewModel
                {
                    //SubjectName = subjectName,
                    ExamName = examName.NameExam,
                    TimeEnd = record.ENDTIME,
                    ShowAnswers = examName.ShowAnswers,
                    ClassName = className,
                    Score = record.TOTALSCORE,
                    Attempt = record.ATTEMPTCOUNT,
                    ExamRecordId = record.RECORDID,
                    TimeStart = record.STARTTIME,
                });
            }

            // Tổng số lượng bản ghi để tính tổng số trang
            int totalRecords = await db.EXAMRECORDS.CountAsync(a => a.STUDENTID == tk.ID);
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = page;

            return View(examRecordViewModels);
        }

        private async Task<string> GetSubject(int examId)
        {
            return await db.EXAM
                .Where(e => e.IDEXAM == examId)
                .Join(db.SUBJECT, e => e.IDSUBJECT, s => s.ID, (e, s) => s.NAMESUBJECT)
                .FirstOrDefaultAsync();
        }

        private async Task<ExamDetailsViewModel> GetExamDetails(int examId)
        {
            return await db.EXAM
                .Where(e => e.IDEXAM == examId)
                .Select(e => new ExamDetailsViewModel
                {
                    NameExam = e.NAMEEXAM,
                    CreateEnd = e.CREATEEND,
                    ShowAnswers = e.SHOWANSWERS
                })
                .FirstOrDefaultAsync();
        }
        private async Task<string> GetClass(int examId)
        {
            return await (from e in db.EXAM
                          join c in db.CLASS on e.IDCLASS equals c.ID
                          where e.IDEXAM == examId
                          select c.NAMECLASS).FirstOrDefaultAsync();
        }

        private void getInfo()
        {
            var listIndustry = db.INDUSTRY.Select(i => new { ID = i.ID, Name = i.NAMEINDUSTRY  }).ToList();
            var listCourse = db.COURSE.Select(i => new { ID = i.ID, Name = i.NAMECOURSE }).ToList();
            var listTeacher = db.ACCOUNTUSER.Select(i => new { ID = i.ID, Name = i.NAMEUSER }).ToList();
            ViewBag.Teacher = new SelectList(listTeacher, "ID", "Name");
            ViewBag.Industry = new SelectList(listIndustry, "ID", "Name");
            ViewBag.Course = new SelectList(listCourse, "ID", "Name");
        }

        [Route("info/info-pesional")]
        public async Task<ActionResult> PesionalInfo()
        {
            var tk = Session["User"] as AccountStudents;
            var student = await db.ACCOUNTSTUDENTS.Where(n => n.ID == tk.ID).FirstOrDefaultAsync();
            getInfo();
            var classes = await (from socl in db.STUDENTONCLASS
                                 join cl in db.CLASS on socl.IDCLASS equals cl.ID
                                 join teacher in db.ACCOUNTUSER on cl.IDTEACHER equals teacher.ID
                                 where socl.IDSTUDENT == tk.ID  && teacher.ROLE == 2
                                 select new ClassToStudentViewModel
                                 {
                                     ID = cl.ID,
                                     KEYCLASS = cl.KEYCLASS,
                                     NAMECLASS = cl.NAMECLASS,
                                     TeacherName = teacher.NAMEUSER
                                 }).ToListAsync();
            ViewBag.Class = classes;
            return View(student);
        }

        [HttpPost]
        public JsonResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin!" });
                }
                if (newPassword != confirmPassword)
                {
                    return Json(new { success = false, message = "Mật khẩu mới và xác nhận không khớp!" });
                }
                var tk = Session["User"] as AccountStudents;
                var user = db.ACCOUNTSTUDENTS.FirstOrDefault(u => u.ID == tk.ID);
                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PASS))
                {
                    return Json(new { success = false, message = "Mật khẩu hiện tại không đúng!" });
                }
                user.PASS = BCrypt.Net.BCrypt.HashPassword(newPassword);
                db.SaveChanges();

                return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateStudentInfo(AccountStudents model, HttpPostedFileBase imageUpload)
        {
            if (ModelState.IsValid)
            {
                var tk = Session["User"] as AccountStudents;
                var student = db.ACCOUNTSTUDENTS.Find(tk.ID);
                if (student != null)
                {
                    student.NAMESTUDENT = model.NAMESTUDENT;
                    student.SEX = model.SEX;
                    student.BIRTHDAY = model.BIRTHDAY;
                    student.EMAIL = model.EMAIL;

                    // Xử lý hình ảnh
                    if (imageUpload != null && imageUpload.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(imageUpload.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Theme/img/Info"), fileName);
                        imageUpload.SaveAs(filePath);
                        student.IMAGE = fileName;
                    }

                    db.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
                }
                return Json(new { success = false, message = "Không tìm thấy học sinh." });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
        }


        public ActionResult Contact()
        {
            return View();
        }

    }
}
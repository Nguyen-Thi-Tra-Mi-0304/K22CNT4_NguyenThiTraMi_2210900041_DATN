using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        [Route("Home/ViewIndex")]
        public async Task<ActionResult> ViewIndex()
        {
            var tk = Session["User"] as AccountStudents;

            // Lấy danh sách ID lớp mà sinh viên thuộc về
            var classIds = await db.STUDENTONCLASS
                            .Where(n => n.IDSTUDENT == tk.ID)
                            .Select(n => n.IDCLASS)
                            .ToListAsync();

            // Lấy danh sách các kỳ thi và ánh xạ thành ExamModel
            var exams = await db.EXAM
                         .Where(exam => classIds.Contains((int)exam.IDCLASS))
                         .Select(exam => new ExamModel
                         {
                             NameExam = exam.NAMEEXAM,
                             DateTime = (DateTime)exam.CREATESTART,
                             ASSIGNMENTTIME = exam.ASSIGNMENTTIME,
                             SubjectName = db.SUBJECT
                                             .Where(s => s.ID == exam.IDSUBJECT)
                                             .Select(s => s.NAMESUBJECT)
                                             .FirstOrDefault(),
                             Status = exam.STATUS
                         }).ToListAsync();

            // Truyền dữ liệu vào ViewBag
            ViewBag.Exams = exams;

            return View();
        }
    }
}
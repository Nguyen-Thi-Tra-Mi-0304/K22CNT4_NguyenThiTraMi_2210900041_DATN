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
    [AuthorizeRole("Teacher")]
    public class ClassToTeacherController : BaseController
    {
        public async Task<ActionResult> ListClassToTeacher()
        {
            var tk = Session["UserName"] as AccountUser;
            var listclass = await db.CLASS.Where(n => n.IDTEACHER == tk.ID).ToListAsync();
            getInfo();
            return View(listclass);
        }

        private void getInfo()
        {
            var listSemeter = db.SEMESTER
                               .Where(n => n.STT == 1)
                               .Select(s => new { ID = s.ID, Name = s.NAMESEMESTER })
                               .ToList();
            var listIndustry = db.INDUSTRY
                               .Where(n => n.STT == 1)
                               .Select(s => new { ID = s.ID, Name = s.NAMEINDUSTRY })
                               .ToList();
            var listCourse= db.COURSE
                               .Where(n => n.STT == 1)
                               .Select(s => new { ID = s.ID, Name = s.NAMECOURSE })
                               .ToList();
            ViewBag.Semester = new SelectList(listSemeter, "ID", "Name");
            ViewBag.Industry = new SelectList(listIndustry, "ID", "Name");
            ViewBag.Course = new SelectList(listCourse, "ID", "Name");
        }
        public async Task<ActionResult> DetailsStudentOnTeacher(int id)
        {
            var detailsClass = await (from cl in db.CLASS
                                      join soc in db.STUDENTONCLASS on cl.ID equals soc.IDCLASS
                                      join ast in db.ACCOUNTSTUDENTS on soc.IDSTUDENT equals ast.ID
                                      where cl.ID == id
                                      select new StudentViewModel
                                      {
                                          IDINDUSTRY = ast.IDINDUSTRY,
                                          IDCOURSE = ast.IDCOURSE,
                                          NAMESTUDENT = ast.NAMESTUDENT,
                                          ACCOUNT = ast.ACCOUNT,
                                          SEX = ast.SEX,
                                          BIRTHDAY = ast.BIRTHDAY,
                                          EMAIL = ast.EMAIL,
                                          STT = ast.STT
                                      }).ToListAsync();

            getInfo();
            return View(detailsClass);
        }


    }
}
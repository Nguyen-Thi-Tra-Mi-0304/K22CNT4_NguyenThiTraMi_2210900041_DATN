using OfficeOpenXml;
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
    public class ClassController : BaseController
    {
        // GET: Admin/Class
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
            var listTeacher = db.ACCOUNTUSER
                               .Where(n => n.ROLE == 2 && n.STT == 1)
                               .Select(s => new { ID = s.ID, Name = s.NAMEUSER })
                               .ToList();
            ViewBag.Semester = new SelectList(listSemeter, "ID", "Name");
            ViewBag.Industry = new SelectList(listIndustry, "ID", "Name");
            ViewBag.Teacher = new SelectList(listTeacher, "ID", "Name");
        }

        private int CountStudentsInClass(int classId)
        {
            return db.STUDENTONCLASS.Count(s => s.IDCLASS == classId);
        }

        [HttpGet]
        public async Task<ActionResult> ListClass()
        {
            getInfo();
            var classes = await db.CLASS.Where(n => n.STT == 1).OrderByDescending(n => n.CREATEAT).ToListAsync();
            ViewBag.StudentCounts = classes.ToDictionary(c => c.ID, c => CountStudentsInClass(c.ID));
            return View(classes);
        }

        [HttpGet]
        public ActionResult CreateClass()
        {
            getInfo();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateClass(FormCollection f)
        {
            try
            {
                string code = f["KEYCLASS"];
                var existingClass = await db.CLASS.FirstOrDefaultAsync(s => s.KEYCLASS == code);
                if (existingClass != null)
                {
                    getInfo();
                    ViewBag.ErrorMessage = "Mã lớp đã tồn tại. Vui lòng sử dụng mã lớp khác.";
                    return View();
                }
                Class cl = new Class
                {
                    IDINDUSTRY = int.Parse(f["IDINDUSTRY"]),
                    IDSEMESTER = int.Parse(f["IDSEMESTER"]),
                    IDTEACHER = int.Parse(f["IDTEACHER"]),
                    KEYCLASS = f["KEYCLASS"],
                    NAMECLASS = f["NAMECLASS"],
                    DESCRIBE = f["DESCRIBE"],
                    CREATEAT = DateTime.Now,
                    CREATEUPDATE= DateTime.Now,
                    STT = 1
                };
                db.CLASS.Add(cl);
                db.SaveChanges();
                return RedirectToAction("ListClass");
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditClass (int id)
        {
            getInfo();
            var edit = await db.CLASS.FirstOrDefaultAsync(n => n.ID == id);
            return View(edit);
        }
        [HttpPost]
        public async Task<ActionResult> EditClass(int id, FormCollection f)
        {
            try
            {
                var classToEdit = await db.CLASS.FirstOrDefaultAsync(c => c.ID == id);
                if (classToEdit == null)
                {
                    ViewBag.ErrorMessage = "Lớp học không tồn tại.";
                    return RedirectToAction("ListClass");
                }
                string newKeyClass = f["KEYCLASS"];
                if (classToEdit.KEYCLASS != newKeyClass)
                {
                    var existingClass = await db.CLASS.FirstOrDefaultAsync(c => c.KEYCLASS == newKeyClass);
                    if (existingClass != null)
                    {
                        getInfo();
                        ViewBag.ErrorMessage = "Mã lớp đã tồn tại. Vui lòng sử dụng mã lớp khác.";
                        return View(classToEdit);
                    }
                }

                classToEdit.IDINDUSTRY = int.Parse(f["IDINDUSTRY"]);
                classToEdit.IDSEMESTER = int.Parse(f["IDSEMESTER"]);
                classToEdit.IDTEACHER = int.Parse(f["IDTEACHER"]);
                classToEdit.KEYCLASS = newKeyClass;
                classToEdit.NAMECLASS = f["NAMECLASS"];
                classToEdit.DESCRIBE = f["DESCRIBE"];
                classToEdit.CREATEUPDATE = DateTime.Now;

                await db.SaveChangesAsync();

                return RedirectToAction("ListClass");
            }
            catch (Exception ex)
            {
                getInfo();
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JsonResult DeleteClass(int id)
        {
            try
            {
                var delete = db.CLASS.FirstOrDefault(n => n.ID == id);
                if (delete != null)
                {
                    delete.STT = 0; // Cập nhật trạng thái để ẩn người dùng
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa lớp học thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Lớp học không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa lớp học!" + ex });
            }
        }

        public async Task<ActionResult> AddStudentOnClass(int id)
        {
            var ClassName = await db.CLASS
                              .Where(n => n.ID == id)
                              .Select(s => new { ID = s.KEYCLASS, Name = s.NAMECLASS })
                              .FirstOrDefaultAsync();
            var liststudent = await db.STUDENTONCLASS.Where(n => n.IDCLASS == id).Select(n => n.IDSTUDENT).ToListAsync();
            var students = await db.ACCOUNTSTUDENTS.Where(s => liststudent.Contains(s.ID)).ToListAsync();
            ViewBag.ClassId = id;
            ViewBag.NameClass = $"Mã lớp: {ClassName.ID} _ Tên lớp: {ClassName.Name}";
            return View(students);
        }


        [HttpGet]
        public JsonResult GetStudentInfo(string id)
        {
            var infohs = db.ACCOUNTSTUDENTS.Where(n => n.ACCOUNT == id).SingleOrDefault();
            var nganh = db.INDUSTRY.Where(m => m.ID == infohs.IDINDUSTRY).SingleOrDefault();
            var name = nganh.NAMEINDUSTRY.ToString();
            var studentInfo = new
            {
                ID = infohs.ID,
                FullName = infohs.NAMESTUDENT,
                BirthYear = infohs.BIRTHDAY,
                Gender = infohs.SEX,
                Industry = name,
                Email = infohs.EMAIL,
            };

            return Json(studentInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddStudentToClass(string studentId, int classId)
        {
            try
            {
                var id = db.ACCOUNTSTUDENTS.Where(n => n.ACCOUNT == studentId).SingleOrDefault();
                var existingEntry = db.STUDENTONCLASS
                                      .FirstOrDefault(s => s.IDSTUDENT == id.ID && s.IDCLASS == classId);

                if (existingEntry == null)
                {
                    var newEntry = new StudentOnClass
                    {
                        IDSTUDENT = id.ID,
                        IDCLASS = classId
                    };
                    db.STUDENTONCLASS.Add(newEntry);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Đã thêm học sinh vào lớp." });
                }
                else
                {
                    return Json(new { success = false, message = "Học sinh đã có trong lớp." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteStudentToClass(int idstudent, int idclass)
        {
            try
            {
                var delete = db.STUDENTONCLASS.FirstOrDefault(s => s.IDSTUDENT == idstudent && s.IDCLASS == idclass);
                db.STUDENTONCLASS.Remove(delete);
                db.SaveChanges();
                return RedirectToAction("AddStudentOnClass", new { id = idclass });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> ImportExcel(HttpPostedFileBase excelFile, int classId)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn file để upload." });
            }

            try
            {
                using (var package = new ExcelPackage(excelFile.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        return Json(new { success = false, message = "File Excel không có nội dung." });
                    }
                    var accountList = new List<string>();
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var account = worksheet.Cells[row, 3].Text; 
                        if (!string.IsNullOrWhiteSpace(account))
                        {
                            accountList.Add(account);
                        }
                    }
                    var existingAccounts = await db.STUDENTONCLASS
                                                   .Where(s => s.IDCLASS == classId)
                                                   .Select(s => s.IDSTUDENT)
                                                   .ToListAsync();
                    var newStudents = new List<StudentOnClass>();
                    foreach (var account in accountList)
                    {
                        var studentAccount = await db.ACCOUNTSTUDENTS.FirstOrDefaultAsync(n => n.ACCOUNT == account);
                        if (studentAccount != null && !existingAccounts.Contains(studentAccount.ID))
                        {
                            var student = new StudentOnClass
                            {
                                IDCLASS = classId,
                                IDSTUDENT = studentAccount.ID,
                            };
                            newStudents.Add(student);
                        }
                    }
                    if (newStudents.Count > 0)
                    {
                        db.STUDENTONCLASS.AddRange(newStudents);
                        await db.SaveChangesAsync();
                        return Json(new { success = true, message = "Nhập file Excel thành công!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Tất cả học sinh trong file đã tồn tại trong lớp học." });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý file: " + ex.Message });
            }
        }



    }
}
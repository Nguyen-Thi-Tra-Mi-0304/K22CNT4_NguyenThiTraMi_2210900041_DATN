
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Areas.Admin.Data;
using WebThiTracNghiemOnline.Models;
using WebThiTracNghiemOnline.Models.ViewModels;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    [AuthorizeRole("Teacher")]
    public class ExamController : BaseController
    {
        // GET: Admin/Exam
        private void GetInfo()
        {
            var tk = Session["UserName"] as AccountUser;

            // Lấy danh sách subject by teacher
            var subjectByTeacher = (from sbt in db.SUBJECTBYTEACHER
                                    join sub in db.SUBJECT on sbt.IDSUBJECT equals sub.ID
                                    where sbt.IDTEACHER == tk.ID && sbt.STT == 1
                                    select new
                                    {
                                        ID = sbt.ID, 
                                        Name = sub.NAMESUBJECT
                                    }).ToList();
            var listclass = db.CLASS
                .Where(n => n.STT == 1 && n.IDTEACHER == tk.ID)
                .Select(c => new { ID = c.ID, Name = c.NAMECLASS })
                .ToList();

            ViewBag.Mon = new SelectList(subjectByTeacher, "ID", "Name");
            ViewBag.Lop = new SelectList(listclass, "ID", "Name");
        }

        public ActionResult ListExam()
        {
            var tk = Session["UserName"] as AccountUser;
            var ListExam = db.EXAM.Where(n => n.IDTEACHER == tk.ID).OrderByDescending(n => n.CREATEAT).ToList();
            GetInfo();
            ViewBag.QuestionCounts = ListExam.ToDictionary(c => c.IDEXAM, c => CountQuestionsToExam(c.IDEXAM));
            return View(ListExam);
        }
        private int CountQuestionsToExam(int idexam)
        {
            return db.QUESTIONTOEXAM.Count(s => s.IDEXAM == idexam);
        }

        private int CountStudentsInClass(int classId)
        {
            return db.STUDENTONCLASS.Count(s => s.IDCLASS == classId);
        }

        public async Task<ActionResult> CreateExam()
        {
            var tk = Session["UserName"] as AccountUser;
            var listsubject = await (from s in db.SUBJECT
                                 join st in db.SUBJECTBYTEACHER on s.ID equals st.IDSUBJECT
                                 where st.IDTEACHER == tk.ID && st.STT == 1
                                 select new
                                 {
                                     ID = st.ID,
                                     Code = s.CODE,
                                     Name = s.NAMESUBJECT
                                 }).ToListAsync();
            var listsemeter = await db.SEMESTER.Where(n => n.STT == 1).Select(s => new { ID = s.ID, Name = s.NAMESEMESTER }).ToListAsync();
            var listClass = await (from c in db.CLASS
                             join s in db.SEMESTER on c.IDSEMESTER equals s.ID
                             where c.IDTEACHER == tk.ID && c.STT == 1
                             select new
                             {
                                 ID = c.ID,
                                 KEYCLASS = c.KEYCLASS,
                                 NAMECLASS = c.NAMECLASS,
                                 NAMESEMESTER = s.NAMESEMESTER
                             }).ToListAsync();

            var resultsub = listsubject.Select(c => new
            {
                ID = c.ID,
                DisplayClass = $"{c.Code} - {c.Name}"
            }).ToList();
            var resultclass = listClass.Select(c => new
            {
                ID = c.ID,
                DisplayClass = $"{c.KEYCLASS} - {c.NAMECLASS} - {c.NAMESEMESTER}"
            }).ToList();

            ViewBag.Subject = new SelectList(resultsub, "ID", "DisplayClass");
            ViewBag.Semester = new SelectList(listsemeter, "ID", "Name");
            ViewBag.Class = new SelectList(resultclass, "ID", "DisplayClass");
            return View();
        }

        [HttpPost]
        public ActionResult CreateExam(string examName, string description, DateTime startDate, DateTime endDate, int timetoexam, int attempts, string password, int subjectId, int semesterId, int classId, List<int> questionIds)
        {
            // Lấy thông tin tài khoản người dùng từ Session
            var tk = Session["UserName"] as AccountUser;
            var timenow = DateTime.Now;

            // Tạo đối tượng Exam mới
            var exam = new Exam
            {
                IDSUBJECT = subjectId,
                IDSEMESTER = semesterId,
                IDCLASS = classId,
                IDTEACHER = tk.ID,
                NAMEEXAM = examName,
                DESCRIBLE = description,
                CREATEAT = timenow,
                CREATEUPDATE = timenow,
                CREATESTART = startDate,
                CREATEEND = endDate,
                ASSIGNMENTTIME = timetoexam,
                NUMBER = attempts,
                PASSWORD = password,
                MIXQUESTION = false,
                MIXANSWERS = false,
                SHOWPOINT = false,
                SHOWANSWERS = false,
                STATUS = DetermineExamStatus(startDate, endDate, timenow) 
            };

            // Thêm exam vào cơ sở dữ liệu
            db.EXAM.Add(exam);
            db.SaveChanges();

            // Thêm các câu hỏi vào bài thi
            foreach (var questionId in questionIds)
            {
                var examQuestion = new QuestionsToExam
                {
                    IDEXAM = exam.IDEXAM,
                    IDQUESTION = questionId,
                    STATUS = 1
                };
                db.QUESTIONTOEXAM.Add(examQuestion);
            }
            db.SaveChanges();

            // Trả về JSON với URL chuyển hướng
            return Json(new { redirectUrl = Url.Action("ListExam", "Exam") });
        }

        // GET: Edit Exam
        public async Task<ActionResult> EditExam(int idExam)
        {
            var tk = Session["UserName"] as AccountUser;
            var exam = await db.EXAM.FirstOrDefaultAsync(e => e.IDEXAM == idExam && e.IDTEACHER == tk.ID);
            if (exam == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách môn học, lớp học, và học kỳ
            var listsubject = await (from s in db.SUBJECT
                                     join st in db.SUBJECTBYTEACHER on s.ID equals st.IDSUBJECT
                                     where st.IDTEACHER == tk.ID && st.STT == 1
                                     select new { st.ID, s.CODE, s.NAMESUBJECT }).ToListAsync();

            var listsemeter = await db.SEMESTER.Where(n => n.STT == 1).Select(s => new { s.ID, s.NAMESEMESTER }).ToListAsync();

            var listClass = await (from c in db.CLASS
                                   join s in db.SEMESTER on c.IDSEMESTER equals s.ID
                                   where c.IDTEACHER == tk.ID && c.STT == 1
                                   select new { c.ID, c.KEYCLASS, c.NAMECLASS, s.NAMESEMESTER }).ToListAsync();

            var resultsub = listsubject.Select(c => new { c.ID, DisplayClass = $"{c.CODE} - {c.NAMESUBJECT}" }).ToList();
            var resultclass = listClass.Select(c => new { c.ID, DisplayClass = $"{c.KEYCLASS} - {c.NAMECLASS} - {c.NAMESEMESTER}" }).ToList();

            // Lấy danh sách câu hỏi
            var selectedQuestions = await GetQuestionsByExam(exam.IDEXAM);

            // Tạo ViewModel
            var viewModel = new EditExamViewModel
            {
                Exam = exam,
                SelectedQuestions = selectedQuestions,
                SubjectList = new SelectList(resultsub, "ID", "DisplayClass", exam.IDSUBJECT),
                SemesterList = new SelectList(listsemeter, "ID", "NAMESEMESTER", exam.IDSEMESTER),
                ClassList = new SelectList(resultclass, "ID", "DisplayClass", exam.IDCLASS)
            };

            return View(viewModel);
        }


        public async Task<List<QuestionViewModel>> GetQuestionsByExam(int id)
        {
            var tk = Session["UserName"] as AccountUser;
            try
            {
                var questions = await (from ex in db.EXAM
                                       join qte in db.QUESTIONTOEXAM on ex.IDEXAM equals qte.IDEXAM
                                       join q in db.QUESTION on qte.IDQUESTION equals q.IDQUESTION
                                       where ex.IDEXAM == id && ex.IDTEACHER == tk.ID
                                       select new QuestionViewModel
                                       {
                                           QuestionID = q.IDQUESTION,
                                           QuestionText = q.QUESTIONTEXT,
                                           QuestionType = q.QUESTIONTYPE,
                                           Difficulty = q.DIFFICULTY == 1 ? "Dễ" : q.DIFFICULTY == 2 ? "Trung bình" : "Khó",
                                           Answers = db.ANSWERS
                                               .Where(a => a.IDQUESTION == q.IDQUESTION)
                                               .Select(a => new AnswerViewModel
                                               {
                                                   AnswerID = a.IDANSWER,
                                                   AnswerText = a.ANSWERTEXT,
                                                   IsCorrect = a.ISCORRECT
                                               }).ToList(),
                                           CorrectAnswersTextList = db.ANSWERS
                                               .Where(a => a.IDQUESTION == q.IDQUESTION && a.ISCORRECT)
                                               .Select(a => a.ANSWERTEXT)
                                               .ToList()
                                       }).ToListAsync();
                return questions;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching questions: " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult UpdateExam(int ID, string NAMEEXAM, string DESCRIBLE,DateTime TIMESTART, DateTime TIMEEND, int TIME, int SOLAN, string PASSWORD)
        {
            try
            {
                var timenow = DateTime.Now;
                var exam = db.EXAM.FirstOrDefault(e => e.IDEXAM == ID);  

                if (exam != null)
                {
                    exam.NAMEEXAM = NAMEEXAM;
                    exam.DESCRIBLE = DESCRIBLE;
                    exam.CREATESTART = TIMESTART;
                    exam.CREATEEND = TIMEEND;
                    exam.ASSIGNMENTTIME = TIME;
                    exam.NUMBER = SOLAN;
                    exam.PASSWORD = PASSWORD;
                    exam.STATUS = DetermineExamStatus(TIMESTART, TIMEEND, timenow);
                    db.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        private int DetermineExamStatus(DateTime startDate, DateTime endDate, DateTime currentTime)
        {
            if (startDate > currentTime)
            {
                return 1; // Chưa bắt đầu
            }
            else if (startDate <= currentTime && endDate >= currentTime)
            {
                return 2; // Đang làm
            }
            else
            {
                return 3; // Đã kết thúc
            }
        }

        [HttpGet]
        public JsonResult GetExamSettings(int id)
        {
            var exam = db.EXAM.Find(id);
            var settings = new
            {
                id = exam.IDEXAM,
                randomQuestions = exam.MIXQUESTION,
                randomAnswers = exam.MIXANSWERS,
                showScore = exam.SHOWPOINT,
                showResult = exam.SHOWANSWERS
            };
            return Json(settings, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveExamSettings(int id, bool mixQuestions, bool mixAnswers, bool showScore, bool showResult)
        {
            var exam = db.EXAM.Find(id);
            if (exam != null)
            {
                exam.MIXQUESTION = mixQuestions;
                exam.MIXANSWERS = mixAnswers;
                exam.SHOWPOINT = showScore;
                exam.SHOWANSWERS = showResult;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        [HttpGet]
        public ActionResult DetailsExam(int id)
        {
            var tk = Session["UserName"] as AccountUser;
            var exam = db.EXAM.FirstOrDefault(n => n.IDTEACHER == tk.ID && n.IDEXAM == id);

            if (exam == null)
            {
                return HttpNotFound();
            }

            // Tính số học sinh trong lớp
            ViewBag.StudentCount = CountStudentsInClass(exam.IDCLASS);

            // Lấy danh sách tất cả học sinh trong lớp
            var studentsInClass = db.ACCOUNTSTUDENTS
                .Join(db.STUDENTONCLASS,
                      student => student.ID,
                      studentOnClass => studentOnClass.IDSTUDENT,
                      (student, studentOnClass) => new { student, studentOnClass })
                .Where(s => s.studentOnClass.IDCLASS == exam.IDCLASS)
                .Select(s => s.student)
                .ToList();

            // Lấy số lần thi từ bảng EXAM
            var examEntity = db.EXAM.Where(e => e.IDEXAM == id).FirstOrDefault();
            var attemptCounts = examEntity != null ? GetAttemptRange(examEntity.NUMBER) : new List<int>();

            // Tạo danh sách ExamAttemptViewModel
            var examAttempts = attemptCounts
                .Select(attempt => new ExamAttemptViewModel
                {
                    AttemptCount = attempt,
                    Records = GetExamRecordsForAttempt(id, attempt, studentsInClass)  
                }).ToList();

            // Tính tổng số lượng câu hỏi trong đề thi và phân loại theo độ khó
            var questionCounts = (from qte in db.QUESTIONTOEXAM
                                  join q in db.QUESTION on qte.IDQUESTION equals q.IDQUESTION
                                  where qte.IDEXAM == exam.IDEXAM
                                  select q.DIFFICULTY).ToList();

            var easyCount = questionCounts.Count(d => d == 1);
            var mediumCount = questionCounts.Count(d => d == 2);
            var hardCount = questionCounts.Count(d => d == 3);

            // Gán số lượng câu hỏi vào ViewBag để hiển thị trong view
            ViewBag.TotalQuestionCount = questionCounts.Count;
            ViewBag.EasyCount = easyCount;
            ViewBag.MediumCount = mediumCount;
            ViewBag.HardCount = hardCount;


            return View(new ExamDetailsViewModel
            {
                Exam = exam,
                ExamAttempts = examAttempts
            });
        }

        // Hàm để lấy dãy số lần thi
        private List<int> GetAttemptRange(int numberOfAttempts)
        {
            var attempts = new List<int>();
            for (int i = 1; i <= numberOfAttempts; i++)
            {
                attempts.Add(i);
            }
            return attempts;
        }

        // Hàm để lấy bản ghi điểm của học sinh cho mỗi lần thi
        private List<ExamRecord> GetExamRecordsForAttempt(int examId, int attemptCount, List<AccountStudents> studentsInClass)
        {
            var examRecords = db.EXAMRECORDS
                .Where(r => r.EXAMID == examId && r.ATTEMPTCOUNT == attemptCount)
                .ToList();

            return studentsInClass.Select(student =>
            {
                var record = examRecords.FirstOrDefault(r => r.STUDENTID == student.ID);
                return new ExamRecord
                {
                    AccountStudent = student.ACCOUNT,
                    StudentName = student.NAMESTUDENT,
                    Sex = student.SEX,
                    Birthday = student.BIRTHDAY,
                    TotalScore = (record != null) ? record.TOTALSCORE : 0,
                    IsCompleted = (record != null) ? record.ISCOMPLETED : false
                };
            }).ToList();
        }

        public ActionResult ExportExamAttemptToExcel(int id, int att)
        {
            try
            {
                var exam = db.EXAM.Find(id);
                var studentsInClass = db.ACCOUNTSTUDENTS
                    .Join(db.STUDENTONCLASS, student => student.ID, studentOnClass => studentOnClass.IDSTUDENT, (student, studentOnClass) => new { student, studentOnClass })
                    .Where(s => s.studentOnClass.IDCLASS == exam.IDCLASS)
                    .Select(s => s.student)
                    .ToList();

                using (var package = new ExcelPackage())
                {
                    // Lấy danh sách tất cả lần thi (attempts) của bài thi
                    var attemptCounts = db.EXAMRECORDS
                        .Where(r => r.EXAMID == id)
                        .Select(r => r.ATTEMPTCOUNT)
                        .Distinct()
                        .ToList();

                    foreach (var attemptCount in attemptCounts)
                    {
                        var attemptRecords = studentsInClass.Select(student =>
                        {
                            var record = db.EXAMRECORDS.FirstOrDefault(r => r.EXAMID == id && r.ATTEMPTCOUNT == att && r.STUDENTID == student.ID);
                            return new
                            {
                                StudentId = student.ACCOUNT,
                                StudentName = student.NAMESTUDENT,
                                Gender = student.SEX,
                                Birthday = student.BIRTHDAY,
                                TotalScore = record?.TOTALSCORE ?? 0,
                                IsCompleted = record?.ISCOMPLETED ?? false,
                                Grade = record?.TOTALSCORE >= 8 ? "Giỏi" :
                                        record?.TOTALSCORE >= 6 ? "Khá" :
                                        record?.TOTALSCORE >= 4 ? "Trung bình" : "Yếu"
                            };
                        }).ToList();

                        var worksheet = package.Workbook.Worksheets.Add($"Lần thi {attemptCount}");

                        // Tạo tiêu đề cột
                        worksheet.Cells[1,1].Value = "Tên bài thi:" + exam.NAMEEXAM;
                        worksheet.Cells[2, 1].Value = "Lần thi thứ:" + att;
                        worksheet.Cells[4, 1].Value = "Mã học sinh";
                        worksheet.Cells[4, 2].Value = "Tên học sinh";
                        worksheet.Cells[4, 3].Value = "Giới tính";
                        worksheet.Cells[4, 4].Value = "Ngày sinh";
                        worksheet.Cells[4, 5].Value = "Điểm";
                        worksheet.Cells[4, 6].Value = "Xếp loại";

                        var headerCells = new[] { worksheet.Cells[1, 1], worksheet.Cells[2, 1], worksheet.Cells[4, 1], worksheet.Cells[4, 2], worksheet.Cells[4, 3], worksheet.Cells[4, 4], worksheet.Cells[4, 5], worksheet.Cells[4, 6] };

                        // Áp dụng in đậm cho tất cả các ô tiêu đề
                        foreach (var cell in headerCells)
                        {
                            cell.Style.Font.Bold = true;
                        }
                        // Dữ liệu từ attemptRecords
                        int row = 5;
                        foreach (var record in attemptRecords)
                        {
                            worksheet.Cells[row, 1].Value = record.StudentId;
                            worksheet.Cells[row, 2].Value = record.StudentName;
                            worksheet.Cells[row, 3].Value = record.Gender;
                            worksheet.Cells[row, 4].Value = record.Birthday.ToString("dd/MM/yyyy");
                            worksheet.Cells[row, 5].Value = record.TotalScore;
                            worksheet.Cells[row, 6].Value = record.Grade;
                            row++;
                        }
                    }

                    // Lưu file vào MemoryStream
                    using (var memoryStream = new MemoryStream())
                    {
                        package.SaveAs(memoryStream);
                        memoryStream.Position = 0; // Đặt lại vị trí của stream để có thể đọc từ đầu

                        // Trả về file Excel để người dùng tải về
                        var fileName = $"Mon_Thi_{exam.NAMEEXAM}_{Guid.NewGuid()}.xlsx";
                        Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                        return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetChaptersBySubject(int subjectId)
        {
            var tk = Session["UserName"] as AccountUser;
            var chapters = await (from st in db.SUBJECTBYTEACHER
                                  join c in db.CHAPTER on st.ID equals c.IDSUBBYTEACHER
                                  where st.ID == subjectId && st.IDTEACHER == tk.ID
                                  select new
                                  {
                                      ID = c.IDCHAPTER,
                                      Name = c.NAMECHAPTER
                                  }).ToListAsync(); 

            var allOption = new { ID = 0, Name = "Tất cả" };
            chapters.Insert(0, allOption);

            return Json(chapters, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetQuestionCounts(int subjectId, int chapterId)
        {
            var tk = Session["UserName"] as AccountUser;
            var questions = db.QUESTION.AsQueryable();

            if (chapterId == 0)
            {
                questions = from sbt in db.SUBJECTBYTEACHER
                            join c in db.CHAPTER on sbt.ID equals c.IDSUBBYTEACHER
                            join q in questions on c.IDCHAPTER equals q.IDCHAPTER
                            where sbt.ID == subjectId && sbt.IDTEACHER == tk.ID
                            select q;
                            
            }
            else
            {
                questions = questions.Where(q => q.IDCHAPTER == chapterId);
            }

            var easyCount = questions.Count(q => q.DIFFICULTY == 1);
            var mediumCount = questions.Count(q => q.DIFFICULTY == 2);
            var hardCount = questions.Count(q => q.DIFFICULTY == 3);


            return Json(new { easy = easyCount, medium = mediumCount, hard = hardCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetRandomQuestionsByChapterAndDifficulty(int subId, int chapterId, int easyCount, int mediumCount, int hardCount)
        {
            try
            {
                var tk = Session["UserName"] as AccountUser;

                // Truy vấn cơ bản cho các câu hỏi theo môn học và giáo viên
                var questionsQuery = db.QUESTION.AsQueryable();

                if (chapterId == 0)
                {
                    questionsQuery = from sbt in db.SUBJECTBYTEACHER
                                     join c in db.CHAPTER on sbt.ID equals c.IDSUBBYTEACHER
                                     join q in db.QUESTION on c.IDCHAPTER equals q.IDCHAPTER
                                     where sbt.ID == subId && sbt.IDTEACHER == tk.ID
                                     select q;
                }
                else
                {
                    questionsQuery = questionsQuery.Where(q => q.IDCHAPTER == chapterId);
                }

                // Hàm lấy câu hỏi ngẫu nhiên theo độ khó
                async Task<List<Question>> GetRandomQuestions(int difficulty, int count) =>
                    await questionsQuery
                        .Where(q => q.DIFFICULTY == difficulty)
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(count)
                        .ToListAsync();

                // Lấy danh sách câu hỏi theo độ khó
                var easyQuestions = await GetRandomQuestions(1, easyCount);
                var mediumQuestions = await GetRandomQuestions(2, mediumCount);
                var hardQuestions = await GetRandomQuestions(3, hardCount);

                // Tạo danh sách câu hỏi với thông tin chi tiết
                var selectedQuestions = easyQuestions
                    .Concat(mediumQuestions)
                    .Concat(hardQuestions)
                    .Select(q => new
                    {
                        q.IDQUESTION,
                        q.QUESTIONTEXT,
                        QuestionType = q.QUESTIONTYPE == 1 ? "Một đáp án" : q.QUESTIONTYPE == 2 ? "Nhiều đáp án" : "Điền vào ô trống",
                        Difficulty = q.DIFFICULTY == 1 ? "Dễ" : q.DIFFICULTY == 2 ? "Trung bình" : "Khó",
                        CorrectAnswers = string.Join(" || ", db.ANSWERS
                                            .Where(a => a.IDQUESTION == q.IDQUESTION && a.ISCORRECT)
                                            .Select(a => a.ANSWERTEXT))
                    })
                    .ToList();

                return Json(selectedQuestions, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetQuestionToSubject(int subId )
        {
            var tk = Session["UserName"] as AccountUser; 
            try
            {
                var questions = await (from stb in db.SUBJECTBYTEACHER
                                       join c in db.CHAPTER on stb.ID equals c.IDSUBBYTEACHER
                                       join q in db.QUESTION on c.IDCHAPTER equals q.IDCHAPTER
                                       where stb.ID == subId && stb.IDTEACHER == tk.ID
                                       select new
                                       {
                                           Id = q.IDQUESTION,
                                           QuestionText = q.QUESTIONTEXT,
                                           QuestionType = q.QUESTIONTYPE == 1 ? "Một đáp án" : q.QUESTIONTYPE == 2 ? "Nhiều đáp án" : "Điền vào ô trống",
                                           Difficulty = q.DIFFICULTY == 1 ? "Dễ" : q.DIFFICULTY == 2 ? "Trung bình" : "Khó"
                                       }).ToListAsync();

                var result = questions.Select(q => new
                {
                    q.Id,
                    q.QuestionText,
                    q.QuestionType,
                    q.Difficulty,
                    CorrectAnswers = string.Join(" || ", db.ANSWERS
                        .Where(a => a.IDQUESTION == q.Id && a.ISCORRECT)
                        .Select(a => a.ANSWERTEXT).ToList())
                }).ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
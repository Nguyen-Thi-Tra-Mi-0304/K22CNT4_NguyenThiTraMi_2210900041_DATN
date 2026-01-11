using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Models;
using WebThiTracNghiemOnline.Models.ViewModels;

namespace WebThiTracNghiemOnline.Controllers
{
    public class ViewExamController : BaseController
    {

        // GET: ViewExam
        [Route("list-exam-to-user")]
        public async Task<ActionResult> ViewListExam()
        {
            var tk = Session["User"] as AccountStudents;

            // Lấy danh sách ID lớp mà sinh viên thuộc về
            var classIds = await db.STUDENTONCLASS
                            .Where(n => n.IDSTUDENT == tk.ID)
                            .Select(n => n.IDCLASS)
                            .ToListAsync();

            // Thông tin chi tiết của lớp học
            var detailsClass = await (from c in db.CLASS
                                      join s in db.SEMESTER on c.IDSEMESTER equals s.ID
                                      where classIds.Contains(c.ID)
                                      select new ClassInfo
                                      {
                                          ID = c.ID,
                                          DisplayClass = c.KEYCLASS + " - " + c.NAMECLASS + " - " + s.NAMESEMESTER
                                      }).ToListAsync();

            // Lấy danh sách các kỳ thi của các lớp mà sinh viên thuộc về
            var exams = await db.EXAM
                         .Where(exam => classIds.Contains((int)exam.IDCLASS) && (exam.STATUS == 1 || exam.STATUS == 2))
                         .ToListAsync();

            // Lấy tên giáo viên
            var teacherIds = exams.Select(e => e.IDTEACHER).Distinct();
            var NameTeacher = await db.ACCOUNTUSER
                                .Where(teacher => teacherIds.Contains(teacher.ID))
                                .ToListAsync();

            // Lấy tên môn học
            var subjectIds = exams.Select(s => s.IDSUBJECT).Distinct();
            var NameSubject = await db.SUBJECT.Where(sub => subjectIds.Contains(sub.ID)).ToListAsync();

            ViewBag.NameSubject = NameSubject;
            ViewBag.ClassList = detailsClass;
            ViewBag.NameList = NameTeacher;

            return View(exams);
        }


        [Route("pre-exam")]
        public async Task<ActionResult> PreExam(int id)
        {
            var tk = Session["User"] as AccountStudents;
            var exam = await db.EXAM.FirstOrDefaultAsync(e => e.IDEXAM == id);
            if (exam == null)
            {
                return Content("Không tìm thấy kỳ thi.");
            }

            var classInfo = 
                await (from c in db.CLASS
                       join s in db.SEMESTER on c.IDSEMESTER equals s.ID
                       where c.ID == exam.IDCLASS
                       select new ClassInfo
                       {
                            ID = c.ID,
                            DisplayClass = c.KEYCLASS + " - " + c.NAMECLASS + " - " + s.NAMESEMESTER
                       }).FirstOrDefaultAsync();

            var subject = await db.SUBJECT.FirstOrDefaultAsync(sub => sub.ID == exam.IDSUBJECT);
            var teacher = await db.ACCOUNTUSER.FirstOrDefaultAsync(t => t.ID == exam.IDTEACHER);
            var isStudentInClass = await db.STUDENTONCLASS.AnyAsync(sc => sc.IDSTUDENT == tk.ID && sc.IDCLASS == exam.IDCLASS);

            if (!isStudentInClass)
            {
                return Content("Bạn không có quyền truy cập thông tin này.");
            }

            var countQuestion = await db.QUESTIONTOEXAM.Where(q => q.IDEXAM == exam.IDEXAM).CountAsync();
            var countNumber = await db.EXAMRECORDS.Where(q => q.EXAMID == id && q.STUDENTID == tk.ID).CountAsync();
            var attemptRecords = await db.EXAMRECORDS.Where(er => er.EXAMID == id && er.STUDENTID == tk.ID).CountAsync();

            ViewBag.ClassInfo = classInfo;
            ViewBag.Subject = subject;
            ViewBag.Teacher = teacher;
            ViewBag.Question = countQuestion;
            ViewBag.Number = exam.NUMBER - countNumber;
            ViewBag.AttemptRecords = attemptRecords;
            return View(exam);
        }


        [Route("password-exam")]
        [HttpPost]
        public JsonResult ValidatePassword(int id, string examPassword)
        {
            var exam = db.EXAM.Find(id);
            if (exam != null && exam.PASSWORD == examPassword)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Mật khẩu không đúng!" });
            }
        }

        [Route("start-exam")]
        [HttpGet]
        public async Task<ActionResult> StartExam(int id, int page = 1)
        {
            var tk = Session["User"] as AccountStudents;
            var exam = db.EXAM.Find(id);
            var classInfo = await (from c in db.CLASS
                                   join s in db.SEMESTER on c.IDSEMESTER equals s.ID
                                   where c.ID == exam.IDCLASS
                                   select new ClassInfo
                                   {
                                       ID = c.ID,
                                       DisplayClass = c.KEYCLASS + " - " + c.NAMECLASS + " - " + s.NAMESEMESTER
                                   }).FirstOrDefaultAsync();

            var nameExam = exam.NAMEEXAM;
            int maxAttemptCount = exam.NUMBER;
            var existingRecord = await db.EXAMRECORDS
                                .Where(r => r.STUDENTID == tk.ID && r.EXAMID == id)
                                .OrderByDescending(r => r.ATTEMPTCOUNT)
                                .FirstOrDefaultAsync();

            if (existingRecord == null)
            {
                ViewBag.classInfo = classInfo;
                ViewBag.name = nameExam;
                // Khởi tạo bài thi mới
                var result = StartNewAttempt(id, tk.ID, exam, 1) as ViewResult;

                // Lấy danh sách câu hỏi từ ViewBag trong StartNewAttempt
                var questionList = result?.ViewBag.AllQuestions as List<QuestionViewModel> ?? new List<QuestionViewModel>();

                // Áp dụng phân trang
                const int PageSize = 10;
                int totalQuestions = questionList.Count;
                var paginatedQuestions = questionList
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                ViewBag.TotalPages = (int)Math.Ceiling(totalQuestions / (double)PageSize);
                ViewBag.CurrentPage = page;
                ViewBag.AllQuestions = questionList;

                return View(paginatedQuestions);
            }

            var remainingTime = (existingRecord.ENDTIME - DateTime.Now).TotalMinutes;
            if (remainingTime > 0 && existingRecord.ISCOMPLETED == false)
            {
                ViewBag.Count = existingRecord.ATTEMPTCOUNT;
                ViewBag.TimeExamInMinutes = remainingTime;
                ViewBag.ExamID = exam.IDEXAM;
                ViewBag.classInfo = classInfo;
                ViewBag.name = nameExam;

                List<QuestionViewModel> questionList;
                if (Session[$"ExamQuestions_{id}_{tk.ID}"] == null)
                {
                    var questionsInExam = GetQuestionsInExamRandom(id, exam.MIXQUESTION, exam.MIXANSWERS);
                    questionList = PrepareQuestionList(questionsInExam, tk.ID, id, existingRecord.ATTEMPTCOUNT);
                    Session[$"ExamQuestions_{id}_{tk.ID}"] = questionList;
                }
                else
                {
                    questionList = (List<QuestionViewModel>)Session[$"ExamQuestions_{id}_{tk.ID}"];

                    var userAnswers = db.TEMPANSWERS
                        .Where(ua => ua.STUDENTID == tk.ID && ua.EXAMID == id && ua.ATTEMPTCOUNT == existingRecord.ATTEMPTCOUNT)
                        .ToList();

                    foreach (var question in questionList)
                    {
                        question.SelectedAnswersId = userAnswers
                            .Where(ua => ua.QUESTIONID == question.QuestionID)
                            .Select(ua => ua.ANSWERSID)
                            .ToList();

                        question.UserAnswerTextList = userAnswers
                            .Where(ua => ua.QUESTIONID == question.QuestionID)
                            .Select(ua => ua.ANSWERSTEXT)
                            .ToList();
                    }
                }

                const int PageSize = 10;
                int totalQuestions = questionList.Count;
                var paginatedQuestions = questionList
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                ViewBag.TotalPages = (int)Math.Ceiling(totalQuestions / (double)PageSize);
                ViewBag.CurrentPage = page;
                ViewBag.AllQuestions = questionList;

                return View(paginatedQuestions);
            }

            if (existingRecord.ATTEMPTCOUNT < maxAttemptCount)
            {
                ViewBag.classInfo = classInfo;
                ViewBag.name = nameExam;
                return StartNewAttempt(id, tk.ID, exam, existingRecord.ATTEMPTCOUNT + 1);
            }

            return RedirectToAction("PreExam", new { id = id });
        }


        private ActionResult StartNewAttempt(int examId, int studentId, Exam exam, int attemptCount)
        {
            var time = DateTime.Now;

            var newExamRecord = new ExamRecords
            {
                STUDENTID = studentId,
                EXAMID = examId,
                STARTTIME = time,
                ENDTIME = time.AddMinutes(exam.ASSIGNMENTTIME),
                ATTEMPTCOUNT = attemptCount,
                TOTALSCORE = 0,
                ISCOMPLETED = false,
            };

            db.EXAMRECORDS.Add(newExamRecord);
            db.SaveChanges();

            // Xóa danh sách câu hỏi cũ trong Session
            string sessionKey = $"ExamQuestions_{examId}_{studentId}";
            Session.Remove(sessionKey);

            ViewBag.Count = newExamRecord.ATTEMPTCOUNT;
            ViewBag.TimeExamInMinutes = exam.ASSIGNMENTTIME;
            ViewBag.ExamID = exam.IDEXAM;

            var questionsInExam = GetQuestionsInExamRandom(examId, exam.MIXQUESTION, exam.MIXANSWERS);
            var questionList = PrepareQuestionList(questionsInExam, studentId, examId, attemptCount);

            ViewBag.AllQuestions = questionList;
            Session[sessionKey] = questionList;

            return View(questionList);
        }

        // Hàm lấy câu hỏi và đáp án trong bài thi
        private List<QuestionViewModel> GetQuestionsInExamRandom(int examId, bool mixQuestions, bool mixAnswer)
        {
            var questions = (from q in db.QUESTION
                             join qt in db.QUESTIONTOEXAM on q.IDQUESTION equals qt.IDQUESTION
                             where qt.IDEXAM == examId
                             select new
                             {
                                 QuestionID = q.IDQUESTION,
                                 QuestionText = q.QUESTIONTEXT,
                                 QuestionType = q.QUESTIONTYPE,
                                 Answers = db.ANSWERS.Where(ans => ans.IDQUESTION == q.IDQUESTION).ToList()
                             }).ToList()
                             .Select(q => new QuestionViewModel
                             {
                                 QuestionID = q.QuestionID,
                                 QuestionText = q.QuestionText,
                                 QuestionType = q.QuestionType,
                                 Answers = q.Answers.Select(a => new AnswerViewModel
                                 {
                                     AnswerID = a.IDANSWER,
                                     AnswerText = a.ANSWERTEXT,
                                     IsCorrect = a.ISCORRECT,
                                 }).ToList(),
                             }).ToList();

            // Đảo câu hỏi nếu mixQuestions = true
            if (mixQuestions)
            {
                questions = questions.OrderBy(q => Guid.NewGuid()).ToList();
            }

            // Đảo đáp án nếu mixAnswer = true
            if (mixAnswer)
            {
                foreach (var question in questions)
                {
                    question.Answers = question.Answers.OrderBy(a => Guid.NewGuid()).ToList();
                }
            }

            return questions;
        }

        // Hàm lấy câu hỏi và đáp án trong bài thi
        private List<QuestionViewModel> GetQuestionsInExam(int examId)
        {
            return (from q in db.QUESTION
                    join qt in db.QUESTIONTOEXAM on q.IDQUESTION equals qt.IDQUESTION
                    where qt.IDEXAM == examId
                    select new
                    {
                        QuestionID = q.IDQUESTION,
                        QuestionText = q.QUESTIONTEXT,
                        QuestionType = q.QUESTIONTYPE,
                        Answers = db.ANSWERS.Where(ans => ans.IDQUESTION == q.IDQUESTION).ToList()
                    }).ToList()
                    .Select(q => new QuestionViewModel
                    {
                        QuestionID = q.QuestionID,
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        Answers = q.Answers.Select(a => new AnswerViewModel
                        {
                            AnswerID = a.IDANSWER,
                            AnswerText = a.ANSWERTEXT,
                            IsCorrect = a.ISCORRECT,

                        }).ToList(),
                    }).ToList();
        }

        // Hàm chuẩn bị danh sách câu trả lời
        private List<QuestionViewModel> PrepareQuestionList(List<QuestionViewModel> questionsInExam, int studentId, int examId, int count)
        {
            var userAnswers = db.TEMPANSWERS
                .Where(ua => ua.STUDENTID == studentId && ua.EXAMID == examId && ua.ATTEMPTCOUNT == count)
                .ToList();
            int questionCount = questionsInExam.Count;
            double questionScore = 10.0 / questionCount;
            foreach (var question in questionsInExam)
            {
                if (question.QuestionType == 3)
                {
                    // Lấy danh sách đáp án đúng cho câu hỏi điền vào chỗ trống
                    question.CorrectAnswersTextList = db.ANSWERS // Thay bằng bảng đáp án đúng
                        .Where(ca => ca.IDQUESTION == question.QuestionID)
                        .Select(ca => ca.ANSWERTEXT)
                        .ToList();
                }
                question.Score = questionScore;
                // Trả về câu trả lời đã chọn bằng click
                question.SelectedAnswersId = userAnswers
                    .Where(ua => ua.QUESTIONID == question.QuestionID)
                    .Select(ua => ua.ANSWERSID)
                    .ToList();
                // Trả về câu trả lời đã chọn bằng textbox
                question.UserAnswerTextList = userAnswers
                    .Where(ua => ua.QUESTIONID == question.QuestionID)
                    .Select(ua => ua.ANSWERSTEXT)
                    .ToList();
            }

            return questionsInExam;
        }

        private List<QuestionViewModel> PrepareQuestionListDetail(List<QuestionViewModel> questionsInExam, int studentId, int examId, int count)
        {
            var userAnswers = db.TEMPANSWERS
                .Where(ua => ua.STUDENTID == studentId && ua.EXAMID == examId && ua.ATTEMPTCOUNT == count)
                .ToList();
            int questionCount = questionsInExam.Count;
            double questionScore = 10.0 / questionCount;

            foreach (var question in questionsInExam)
            {
                // Lấy danh sách đáp án đúng
                question.CorrectAnswersTextList = db.ANSWERS
                    .Where(ca => ca.IDQUESTION == question.QuestionID && ca.ISCORRECT)
                    .Select(ca => ca.ANSWERTEXT)
                    .ToList();

                // Gán điểm mặc định
                question.Score = questionScore;

                // Trả về danh sách ID các đáp án đã chọn
                question.SelectedAnswersId = userAnswers
                    .Where(ua => ua.QUESTIONID == question.QuestionID)
                    .Select(ua => ua.ANSWERSID)
                    .ToList();

                // Trả về danh sách các câu trả lời bằng text (cho loại điền vào chỗ trống)
                question.UserAnswerTextList = userAnswers
                    .Where(ua => ua.QUESTIONID == question.QuestionID)
                    .Select(ua => ua.ANSWERSTEXT)
                    .ToList();

                // Đánh giá đúng/sai
                if (question.QuestionType == 1 || question.QuestionType == 2) // Câu hỏi chọn đáp án
                {
                    var correctAnswerIds = db.ANSWERS
                        .Where(a => a.IDQUESTION == question.QuestionID && a.ISCORRECT)
                        .Select(a => a.IDANSWER)
                        .ToList();

                    // So sánh danh sách đáp án đúng và đáp án đã chọn
                    question.IsCorrect = !correctAnswerIds.Except(question.SelectedAnswersId).Any() &&
                                         !question.SelectedAnswersId.Except(correctAnswerIds).Any();
                }
                else if (question.QuestionType == 3) // Câu hỏi điền vào chỗ trống
                {
                    question.IsCorrect = question.UserAnswerTextList
                        .Zip(question.CorrectAnswersTextList, (userAnswer, correctAnswer) =>
                            userAnswer?.Trim().Equals(correctAnswer?.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                        .All(result => result);
                }

                // Gán điểm nếu câu hỏi đúng
                if (question.IsCorrect)
                {
                    question.Score = questionScore;
                }
                else
                {
                    question.Score = 0;
                }
            }

            return questionsInExam;
        }


        [HttpPost]
        public async Task<JsonResult> SaveTempAnswer1(int EXAMID, int Count, int QUESTIONID, int? ANSWERSID)
        {
            try
            {
                var tk = Session["User"] as AccountStudents;
                if (tk == null)
                {
                    return Json(new { success = false, message = "Session expired. Please log in again." });
                }

                var sessionKey = $"ExamQuestions_{EXAMID}_{tk.ID}";
                var questionList = (List<QuestionViewModel>)Session[sessionKey];

                // Cập nhật trạng thái câu trả lời trong session
                var questionInSession = questionList?.FirstOrDefault(q => q.QuestionID == QUESTIONID);
                if (questionInSession != null)
                {
                    questionInSession.SelectedAnswersId = ANSWERSID.HasValue ? new List<int> { ANSWERSID.Value } : new List<int>();
                }

                // Kiểm tra câu trả lời trong cơ sở dữ liệu
                var existingTempAnswer = await db.TEMPANSWERS
                    .FirstOrDefaultAsync(ta => ta.EXAMID == EXAMID && ta.STUDENTID == tk.ID && ta.QUESTIONID == QUESTIONID && ta.ATTEMPTCOUNT == Count);

                if (existingTempAnswer != null)
                {
                    if (ANSWERSID.HasValue)
                    {
                        // Cập nhật đáp án
                        existingTempAnswer.ANSWERSID = ANSWERSID.Value;
                        existingTempAnswer.DATECREATE = DateTime.Now;
                    }
                    else
                    {
                        // Xóa đáp án nếu không có ANSWERSID
                        db.TEMPANSWERS.Remove(existingTempAnswer);
                    }
                }
                else if (ANSWERSID.HasValue)
                {
                    // Thêm đáp án mới
                    var newTempAnswer = new TempAnswers
                    {
                        EXAMID = EXAMID,
                        STUDENTID = tk.ID,
                        QUESTIONID = QUESTIONID,
                        ANSWERSID = ANSWERSID.Value,
                        ATTEMPTCOUNT = Count,
                        ISSUBMITTED = false,
                        DATECREATE = DateTime.Now
                    };
                    db.TEMPANSWERS.Add(newTempAnswer);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await db.SaveChangesAsync();

                // Cập nhật session sau khi lưu
                Session[sessionKey] = questionList;

                return Json(new { success = true, message = "Answer saved successfully" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<JsonResult> SaveTempAnswer2(int EXAMID,int Count, int QUESTIONID, int? ANSWERSID)
        {
            try
            {
                var tk = Session["User"] as AccountStudents;

                // Lấy danh sách đáp án tạm thời cho sinh viên và câu hỏi hiện tại
                var tempAnswers = await db.TEMPANSWERS
                    .Where(ta => ta.EXAMID == EXAMID && ta.STUDENTID == tk.ID && ta.QUESTIONID == QUESTIONID && ta.ATTEMPTCOUNT == Count)
                    .ToListAsync();

                // Nếu ANSWERSID có giá trị (người dùng chọn hoặc bỏ chọn một đáp án)
                if (ANSWERSID.HasValue)
                {
                    // Tìm đáp án đã tồn tại với ANSWERSID
                    var existingAnswer =  tempAnswers.FirstOrDefault(ta => ta.ANSWERSID == ANSWERSID);

                    if (existingAnswer != null)
                    {
                        // Nếu đáp án đã tồn tại, xóa nó (bỏ chọn checkbox)
                        db.TEMPANSWERS.Remove(existingAnswer);
                    }
                    else
                    {
                        // Nếu đáp án chưa tồn tại, thêm mới đáp án
                        var tempAnswer = new TempAnswers
                        {
                            EXAMID = EXAMID,
                            STUDENTID = tk.ID,
                            QUESTIONID = QUESTIONID,
                            ANSWERSID = ANSWERSID.Value,
                            ATTEMPTCOUNT = Count,
                            ISSUBMITTED = false,
                            DATECREATE = DateTime.Now
                        };
                        db.TEMPANSWERS.Add(tempAnswer);
                    }
                }
                else
                {
                    // Nếu không có giá trị ANSWERSID, chỉ xóa tất cả đáp án của câu hỏi đó
                    if (tempAnswers.Any())
                    {
                         db.TEMPANSWERS.RemoveRange(tempAnswers);
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return Json(new { success = true, message = "Answer updated successfully" });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task <ActionResult> SaveTempAnswer3(int examId,int count, List<AnswerSubmission> answers)
        {
            try
            {
                var tk = Session["User"] as AccountStudents;

                if (tk == null)
                {
                    return Json(new { success = false, message = "Người dùng chưa đăng nhập!" });
                }

                foreach (var answer in answers)
                {
                    var existingAnswer = await db.TEMPANSWERS
                        .FirstOrDefaultAsync(x => x.EXAMID == examId
                                             && x.STUDENTID == tk.ID
                                             && x.QUESTIONID == answer.QuestionID
                                             && x.ANSWERSID == answer.AnswerID
                                             && x.ATTEMPTCOUNT == count);

                    if (existingAnswer != null)
                    {
                        existingAnswer.ANSWERSTEXT = answer.AnswerText;
                        existingAnswer.DATECREATE = DateTime.Now;
                    }
                    else
                    {
                        var tempAnswer = new TempAnswers
                        {
                            EXAMID = examId,
                            STUDENTID = tk.ID,
                            QUESTIONID = answer.QuestionID,
                            ANSWERSID = answer.AnswerID,
                            ANSWERSTEXT = answer.AnswerText,
                            ATTEMPTCOUNT = count,
                            ISSUBMITTED = false,
                            DATECREATE = DateTime.Now
                        };
                        db.TEMPANSWERS.Add(tempAnswer);
                    }
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("exam/submit")]
        public async Task<ActionResult> ShowResult(int examId, int attemptCount)
        {
            var tk = Session["User"] as AccountStudents;
            var ex = await db.EXAM.FindAsync(examId);
            // Lấy bản ghi thi hiện tại theo ID bài thi và số lần làm
            var currentExamRecord = await db.EXAMRECORDS
                .FirstOrDefaultAsync(r => r.STUDENTID == tk.ID && r.EXAMID == examId && r.ATTEMPTCOUNT == attemptCount);
            if (currentExamRecord == null)
            {
                return Content("Không tìm thấy bản ghi bài thi.");
            }
            // Tính điểm cho bài thi
            var totalScore = CalculateScore(currentExamRecord, examId, attemptCount, out int correctCount, out int incorrectCount);
            currentExamRecord.TOTALSCORE = Math.Round(totalScore, 2);
            currentExamRecord.ISCOMPLETED = true;
            db.SaveChanges();
            ViewBag.ShowAnswer = ex.SHOWANSWERS;
            ViewBag.Name = ex.NAMEEXAM;
            ViewBag.CountQuestion = await db.QUESTIONTOEXAM.Where(q => q.IDEXAM == examId).CountAsync();
            ViewBag.CorrectCount = correctCount; // Số câu đúng
            ViewBag.IncorrectCount = incorrectCount; // Số câu sai
            return View(currentExamRecord);
        }

        // Hàm tính điểm
        private double CalculateScore(ExamRecords currentExamRecord, int examId, int attemptCount, out int correctCount, out int incorrectCount)
        {
            var questionsInExam = GetQuestionsInExam(examId);
            var userAnswers = db.TEMPANSWERS
                .Where(ua => ua.STUDENTID == currentExamRecord.STUDENTID && ua.EXAMID == examId && ua.ATTEMPTCOUNT == attemptCount)
                .ToList();

            double totalScore = 0;
            int questionCount = questionsInExam.Count;
            double questionScore = 10.0 / questionCount;

            correctCount = 0; // Khởi tạo biến đếm câu đúng
            incorrectCount = 0; // Khởi tạo biến đếm câu sai

            foreach (var question in questionsInExam)
            {
                var correctAnswers = question.Answers
                    .Where(a => a.IsCorrect == true)
                    .Select(a => a.AnswerID)
                    .ToList();

                var userAnswerIds = userAnswers
                    .Where(ua => ua.QUESTIONID == question.QuestionID)
                    .Select(ua => ua.ANSWERSID)
                    .ToList();

                // Loại câu hỏi 1: Chỉ có một đáp án đúng
                if (question.QuestionType == 1)
                {
                    if (correctAnswers.SequenceEqual(userAnswerIds))
                    {
                        totalScore += questionScore;
                        correctCount++; // Tăng số câu đúng
                    }
                    else
                    {
                        incorrectCount++; // Tăng số câu sai
                    }
                }
                // Loại câu hỏi 2: Nhiều đáp án đúng
                else if (question.QuestionType == 2)
                {
                    int correctAnswerCount = correctAnswers.Count; // Đổi tên biến ở đây

                    if (userAnswerIds.Count != correctAnswerCount)
                    {
                        incorrectCount++; // Không khớp số lượng đáp án, tăng câu sai
                        continue;
                    }

                    int userCorrectCount = userAnswerIds.Count(id => correctAnswers.Contains(id));
                    if (userCorrectCount == correctAnswerCount)
                    {
                        totalScore += questionScore;
                        correctCount++; // Tăng số câu đúng
                    }
                    else
                    {
                        incorrectCount++; // Tăng số câu sai
                    }
                }
                // Loại câu hỏi 3: Câu trả lời dạng văn bản
                else if (question.QuestionType == 3)
                {
                    var correctTextAnswers = question.Answers
                        .Where(a => a.IsCorrect == true)
                        .Select(a => a.AnswerText)
                        .ToList();

                    var userTextAnswers = userAnswers
                        .Where(ua => ua.QUESTIONID == question.QuestionID)
                        .Select(ua => ua.ANSWERSTEXT)
                        .ToList();

                    int correctTextCount = correctTextAnswers.Count; // Đổi tên biến ở đây
                    double partialScorePerBlank = questionScore / correctTextCount;
                    int matchingAnswers = 0;

                    for (int i = 0; i < correctTextCount; i++)
                    {
                        if (i < userTextAnswers.Count && string.Equals(correctTextAnswers[i], userTextAnswers[i], StringComparison.OrdinalIgnoreCase))
                        {
                            matchingAnswers++;
                        }
                    }

                    totalScore += matchingAnswers * partialScorePerBlank;

                    // Kiểm tra nếu có câu đúng
                    if (matchingAnswers > 0)
                    {
                        correctCount++; // Tăng số câu đúng
                    }
                    else
                    {
                        incorrectCount++; // Tăng số câu sai
                    }
                }
            }
            return totalScore;
        }


        [Route("exam/details-exam")]
        [HttpGet]
        public async Task<ActionResult> DetailsExam(int id)
        {
            var tk = Session["User"] as AccountStudents; 
            var examRecord = await db.EXAMRECORDS
                .FirstOrDefaultAsync(e => e.RECORDID == id);

            var exam = await db.EXAM.FirstOrDefaultAsync(e => e.IDEXAM == examRecord.EXAMID);

            var classInfo = await (from c in db.CLASS
                                   join s in db.SEMESTER on c.IDSEMESTER equals s.ID
                                   where c.ID == exam.IDCLASS
                                   select new ClassInfo
                                   {
                                       ID = c.ID,
                                       DisplayClass = c.KEYCLASS + " - " + c.NAMECLASS + " - " + s.NAMESEMESTER
                                   }).FirstOrDefaultAsync();

            ViewBag.ClassInfo = classInfo;
            ViewBag.ExamName = exam.NAMEEXAM;

            // Lấy danh sách câu hỏi của bài thi
            var questionsInExam = GetQuestionsInExam(exam.IDEXAM);
            var questionList = PrepareQuestionListDetail(questionsInExam, tk.ID, exam.IDEXAM, examRecord.ATTEMPTCOUNT);

            // Tính điểm cho bài thi
            var totalScore = CalculateScore( examRecord, exam.IDEXAM, examRecord.ATTEMPTCOUNT, out int correctCount, out int incorrectCount);

            ViewBag.CountQuestion = await db.QUESTIONTOEXAM.Where(q => q.IDEXAM == exam.IDEXAM).CountAsync();
            ViewBag.CorrectCount = correctCount; // Số câu đúng
            ViewBag.IncorrectCount = incorrectCount; // Số câu sai
            ViewBag.TotalScore = examRecord.TOTALSCORE;
            ViewBag.AttemptCount = examRecord.ATTEMPTCOUNT;
            ViewBag.TimeStart = examRecord.STARTTIME;
            ViewBag.TimeEnd = examRecord.ENDTIME;


            return View(questionList); 
        }
    }
}
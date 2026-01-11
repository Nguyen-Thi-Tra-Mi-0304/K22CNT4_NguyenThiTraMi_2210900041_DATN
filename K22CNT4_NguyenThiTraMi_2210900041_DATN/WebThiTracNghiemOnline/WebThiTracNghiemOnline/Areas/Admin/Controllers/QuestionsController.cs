using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Areas.Admin.Data;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    [AuthorizeRole("Teacher")]
    public class QuestionsController : BaseController
    {

        [HttpGet]
        public JsonResult GetSubjects()
        {
            var tk = Session["UserName"] as AccountUser;
            var subjects = (from s in db.SUBJECTBYTEACHER
                            join sj in db.SUBJECT on s.IDSUBJECT equals sj.ID
                            join sem in db.SEMESTER on sj.IDSEMESTER equals sem.ID
                            where s.IDTEACHER == tk.ID && s.STT == 1
                            select new
                            {
                                ID = s.ID,
                                SubjectID = s.IDSUBJECT,
                                SubjectName = sj.NAMESUBJECT,
                                SemesterName = sem.NAMESEMESTER
                            }).ToList();

            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetChaptersToSubjects(int subjectId)
        {
            var chapters = db.CHAPTER
                .Where(c => c.IDSUBBYTEACHER == subjectId && c.STT == 1)
                .Select(c => new
                {
                    ChapterID = c.IDCHAPTER,
                    ChapterName = c.NAMECHAPTER
                })
                .ToList();

            return Json(chapters, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> GetQuestionsByChapter(int chapterId)
        {
            // Truy vấn dữ liệu từ cơ sở dữ liệu
            var questions = await db.QUESTION
                .Where(q => q.IDCHAPTER == chapterId && q.STT == 0)
                .OrderByDescending(q => q.CREATEAT)
                .ToListAsync();

            // Xây dựng danh sách kết quả với dữ liệu đồng bộ
            var result = new List<object>();

            foreach (var q in questions)
            {
                var correctAnswers = await db.ANSWERS
                    .Where(a => a.IDQUESTION == q.IDQUESTION && a.ISCORRECT)
                    .Select(a => a.ANSWERTEXT)
                    .ToListAsync();

                result.Add(new
                {
                    q.IDQUESTION,
                    q.QUESTIONTEXT,
                    CorrectAnswers = correctAnswers,
                    QUESTIONTYPE = GetQuestionsType(q.QUESTIONTYPE),
                    DIFFICULTY = GetDifficultyLevel(q.DIFFICULTY),
                    q.NOEDIT
                });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string GetDifficultyLevel(int difficulty)
        {
            switch (difficulty)
            {
                case 1:
                    return "Dễ";
                case 2:
                    return "Trung Bình";
                case 3:
                    return "Khó";
                default:
                    return "Không xác định";
            }
        }

        private string GetQuestionsType(int type)
        {
            switch (type)
            {
                case 1:
                    return "single-choice";
                case 2:
                    return "multiple-choice";
                case 3:
                    return "fill-in-the-blank";
                default:
                    return "Không xác định";
            }
        }

        public ActionResult CreateQuestions()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SaveQuestionAndAnswers(Question question, List<Answers> answers)
        {
            try
            {
                // Lưu câu hỏi vào bảng Question
                question.CREATEAT = DateTime.Now;
                question.CREATEUPDATE = DateTime.Now;
                question.STT = 0;
                question.NOEDIT = 0;
                db.QUESTION.Add(question);
                await db.SaveChangesAsync();

                // Lấy IDQUESTION sau khi lưu để dùng cho các câu trả lời
                int questionId = question.IDQUESTION;

                // Lưu các đáp án vào bảng Answers
                foreach (var answer in answers)
                {
                    answer.IDQUESTION = questionId;
                    db.ANSWERS.Add(answer);
                }

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveQuestionAndAnswersMutichoice(Question question, List<Answers> answers)
        {
            try
            {
                question.CREATEAT = DateTime.Now;
                question.CREATEUPDATE = DateTime.Now;
                question.STT = 0;
                question.NOEDIT = 0;
                db.QUESTION.Add(question);
                await db.SaveChangesAsync();

                // Lấy IDQUESTION sau khi lưu để dùng cho các câu trả lời
                int questionId = question.IDQUESTION;

                // Lưu các đáp án vào bảng Answers
                foreach (var answer in answers)
                {
                    answer.IDQUESTION = questionId;
                    db.ANSWERS.Add(answer);
                }

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveFillInBlankQuestion(Question question, List<Answers> answers)
        {
            try
            {
                question.CREATEAT = DateTime.Now;
                question.CREATEUPDATE = DateTime.Now;
                db.QUESTION.Add(question);
                await db.SaveChangesAsync();

                // Lấy IDQUESTION sau khi lưu để dùng cho các câu trả lời
                int questionId = question.IDQUESTION;

                // Lưu các đáp án vào bảng Answers
                foreach (var answer in answers)
                {
                    answer.IDQUESTION = questionId;
                    db.ANSWERS.Add(answer);
                }

                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ImportQuestionsFromExcel1(HttpPostedFileBase excelFile, int chapterId)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn một file Excel hợp lệ." });
            }

            try
            {
                // Khởi tạo danh sách câu hỏi và đáp án
                var questions = new List<Question>();
                var allAnswers = new List<Answers>();

                // Đọc dữ liệu từ file Excel
                using (var package = new ExcelPackage(excelFile.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var worksheet = package.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên
                    int rowCount = worksheet.Dimension.Rows;


                    // Duyệt qua các hàng, bắt đầu từ hàng 2 để bỏ qua tiêu đề
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var questionText = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrEmpty(questionText))
                        {
                            continue; // Bỏ qua hàng nếu ô nội dung câu hỏi trống
                        }

                        // Kiểm tra câu hỏi đã tồn tại trong cơ sở dữ liệu
                        var existingQuestion = await db.QUESTION
                            .FirstOrDefaultAsync(q => q.QUESTIONTEXT == questionText && q.IDCHAPTER == chapterId);

                        if (existingQuestion != null)
                        {
                            continue; // Bỏ qua câu hỏi nếu đã tồn tại
                        }

                        // Tạo câu hỏi mới
                        var question = new Question
                        {
                            IDCHAPTER = chapterId,
                            QUESTIONTEXT = questionText, // Cột "Nội dung"
                            QUESTIONTYPE = int.Parse(worksheet.Cells[row, 3].Text), // Cột "Loại câu hỏi"
                            DIFFICULTY = int.Parse(worksheet.Cells[row, 4].Text), // Cột "Độ khó"
                            STT = 0,
                            NOEDIT = 0
                        };

                        // Lưu câu hỏi vào danh sách
                        questions.Add(question);

                        // Tạo danh sách đáp án
                        for (int col = 5; col <= 8; col++) // Duyệt các cột Đáp án 1 đến Đáp án 4
                        {
                            var answerText = worksheet.Cells[row, col].Text;
                            if (!string.IsNullOrEmpty(answerText))
                            {
                                var answer = new Answers
                                {
                                    ANSWERTEXT = answerText,
                                    ANSWERTYPE = "1",
                                    BLANKPOSITION = 0,
                                    ISCORRECT = worksheet.Cells[row, col].Style.Font.Bold
                                };
                                allAnswers.Add(answer);
                            }
                        }
                    }
                }
                foreach (var question in questions)
                {
                    var relatedAnswers = allAnswers.Take(4).ToList();
                    allAnswers = allAnswers.Skip(4).ToList();
                    await SaveQuestionAndAnswers(question, relatedAnswers);
                }

                return Json(new { success = true, message = "Đã tải dữ liệu thành công từ file Excel." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xử lý file: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ImportQuestionsFromExcel2(HttpPostedFileBase excelFile, int chapterId)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn một file Excel hợp lệ." });
            }

            try
            {
                // Khởi tạo danh sách câu hỏi và đáp án
                var questions = new List<Question>();
                var allAnswers = new List<Answers>();

                // Đọc dữ liệu từ file Excel
                using (var package = new ExcelPackage(excelFile.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var worksheet = package.Workbook.Worksheets[0]; // Lấy worksheet đầu tiên
                    int rowCount = worksheet.Dimension.Rows;


                    // Duyệt qua các hàng, bắt đầu từ hàng 2 để bỏ qua tiêu đề
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var questionText = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrEmpty(questionText))
                        {
                            continue;
                        }

                        var existingQuestion = await db.QUESTION
                            .FirstOrDefaultAsync(q => q.QUESTIONTEXT == questionText && q.IDCHAPTER == chapterId);

                        if (existingQuestion != null)
                        {
                            continue;
                        }
                        // Tạo câu hỏi mới
                        var question = new Question
                        {
                            IDCHAPTER = chapterId,
                            QUESTIONTEXT = questionText, // Cột "Nội dung"
                            QUESTIONTYPE = int.Parse(worksheet.Cells[row, 3].Text), // Cột "Loại câu hỏi"
                            DIFFICULTY = int.Parse(worksheet.Cells[row, 4].Text), // Cột "Độ khó"
                            STT = 0,
                            NOEDIT = 0
                        };

                        // Lưu câu hỏi vào danh sách
                        questions.Add(question);

                        // Tạo danh sách đáp án cho câu hỏi hiện tại
                        var questionAnswers = new List<Answers>();

                        // Xác định số đáp án của câu hỏi hiện tại
                        for (int col = 5; col <= worksheet.Dimension.Columns; col++) // Duyệt các cột từ Đáp án 1 trở đi
                        {
                            var answerText = worksheet.Cells[row, col].Text;
                            if (!string.IsNullOrEmpty(answerText))
                            {
                                var answer = new Answers
                                {
                                    ANSWERTEXT = answerText,
                                    ANSWERTYPE = "2", // Tùy chỉnh loại đáp án nếu cần
                                    BLANKPOSITION = 0, // Tùy chỉnh nếu cần
                                    ISCORRECT = worksheet.Cells[row, col].Style.Font.Bold // Đánh dấu đúng nếu chữ đậm
                                };
                                questionAnswers.Add(answer);
                            }
                        }
                        allAnswers.AddRange(questionAnswers);

                        // Lưu câu hỏi và danh sách đáp án tương ứng
                        await SaveQuestionAndAnswersMutichoice(question, questionAnswers);
                    }
                }
                return Json(new { success = true, message = "Đã tải dữ liệu thành công từ file Excel." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xử lý file: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> ImportQuestionsFromExcel3(HttpPostedFileBase excelFile, int chapterId)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "Vui lòng chọn một file Excel hợp lệ." });
            }

            try
            {
                var questions = new List<Question>();
                var allAnswers = new List<Answers>();
                using (var package = new ExcelPackage(excelFile.InputStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var questionText = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrEmpty(questionText))
                        {
                            continue;
                        }

                        var existingQuestion = await db.QUESTION
                            .FirstOrDefaultAsync(q => q.QUESTIONTEXT == questionText && q.IDCHAPTER == chapterId);

                        if (existingQuestion != null)
                        {
                            continue;
                        }
                        // Tạo câu hỏi mới
                        var question = new Question
                        {
                            IDCHAPTER = chapterId,
                            QUESTIONTEXT = questionText, // Cột "Nội dung"
                            QUESTIONTYPE = int.Parse(worksheet.Cells[row, 3].Text), // Cột "Loại câu hỏi"
                            DIFFICULTY = int.Parse(worksheet.Cells[row, 4].Text), // Cột "Độ khó"
                            STT = 0,
                            NOEDIT = 0
                        };

                        questions.Add(question);

                        var questionAnswers = new List<Answers>();
                        int position = 1;
                        for (int col = 5; col <= worksheet.Dimension.Columns; col++)
                        {
                            var answerText = worksheet.Cells[row, col].Text;
                            if (!string.IsNullOrEmpty(answerText))
                            {
                                var answer = new Answers
                                {
                                    ANSWERTEXT = answerText,
                                    ANSWERTYPE = "3", 
                                    BLANKPOSITION = position, 
                                    ISCORRECT = true,
                                };
                                questionAnswers.Add(answer);
                                position++;
                            }
                        }
                        allAnswers.AddRange(questionAnswers);
                        await SaveFillInBlankQuestion(question, questionAnswers);
                    }
                }
                return Json(new { success = true, message = "Đã tải dữ liệu thành công từ file Excel." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xử lý file: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteQuestion(int id)
        {
            var question = await db.QUESTION.FindAsync(id);
            if (question == null)
            {
                return Json(new { success = false, message = "Không tìm thấy câu hỏi." });
            }
            question.STT = 1;
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<JsonResult> GetQuestionDetails1(int id)
        {
            try
            {
                // Lấy câu hỏi dựa trên IDQUESTION
                var question = await db.QUESTION
                    .Where(q => q.IDQUESTION == id)
                    .Select(q => new
                    {
                        q.IDQUESTION,
                        q.IDCHAPTER,
                        q.QUESTIONTEXT,
                        q.QUESTIONTYPE,
                        q.DIFFICULTY,
                        Answers = db.ANSWERS
                            .Where(a => a.IDQUESTION == q.IDQUESTION)
                            .Select(a => new
                            {
                                a.IDANSWER,
                                a.ANSWERTEXT,
                                a.ISCORRECT,
                                a.ANSWERTYPE,
                            }).ToList() // Danh sách câu trả lời
                    }).FirstOrDefaultAsync();

                // Nếu không tìm thấy câu hỏi
                if (question == null)
                {
                    return Json(new { success = false, message = "Câu hỏi không tồn tại." }, JsonRequestBehavior.AllowGet);
                }

                // Trả về dữ liệu câu hỏi và danh sách câu trả lời
                return Json(new { success = true, data = question }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lấy dữ liệu." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateQuestionAndAnswers(Question question, List<Answers> answers)
        {
            try
            {
                // Tìm câu hỏi trong cơ sở dữ liệu
                var existingQuestion = await db.QUESTION.FindAsync(question.IDQUESTION);
                if (existingQuestion == null)
                {
                    return Json(new { success = false, message = "Câu hỏi không tồn tại." });
                }

                // Cập nhật thông tin câu hỏi
                existingQuestion.QUESTIONTEXT = question.QUESTIONTEXT;
                existingQuestion.DIFFICULTY = question.DIFFICULTY;
                existingQuestion.CREATEUPDATE = DateTime.Now;

                // Cập nhật hoặc thêm mới đáp án
                foreach (var answer in answers)
                {
                    if (answer.IDANSWER > 0) 
                    {
                        var existingAnswer = await db.ANSWERS.FindAsync(answer.IDANSWER);
                        if (existingAnswer != null)
                        {
                            existingAnswer.ANSWERTEXT = answer.ANSWERTEXT;
                            existingAnswer.ISCORRECT = answer.ISCORRECT;
                           
                        }
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetQuestionDetails2(int id)
        {
            try
            {
                // Lấy thông tin câu hỏi dựa trên ID và kiểm tra loại câu hỏi
                var question = await db.QUESTION
                    .Where(q => q.IDQUESTION == id)
                    .Select(q => new
                    {
                        q.IDQUESTION,
                        q.QUESTIONTEXT,
                        q.DIFFICULTY,
                        q.QUESTIONTYPE
                    })
                    .FirstOrDefaultAsync();

                if (question == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy câu hỏi hoặc loại câu hỏi không phù hợp." }, JsonRequestBehavior.AllowGet);
                }

                // Lấy danh sách đáp án liên quan đến câu hỏi
                var answers = await db.ANSWERS
                    .Where(a => a.IDQUESTION == id)
                    .Select(a => new
                    {
                        a.IDANSWER,
                        a.ANSWERTEXT,
                        a.ISCORRECT
                    })
                    .ToListAsync();

                // Trả về dữ liệu câu hỏi và danh sách đáp án
                return Json(new
                {
                    success = true,
                    question,
                    answers
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateQuestionAndAnswersMultipleCorrect(Question question, List<Answers> answers)
        {
            try
            {
                var existingQuestion = await db.QUESTION.FindAsync(question.IDQUESTION);
                if (existingQuestion == null)
                {
                    return Json(new { success = false, message = "Câu hỏi không tồn tại." });
                }
                existingQuestion.QUESTIONTEXT = question.QUESTIONTEXT;
                existingQuestion.DIFFICULTY = question.DIFFICULTY;
                existingQuestion.CREATEUPDATE = DateTime.Now;

                var existingAnswers = db.ANSWERS.Where(a => a.IDQUESTION == question.IDQUESTION).ToList();
                var answerIdsFromModal = answers.Where(a => a.IDANSWER > 0).Select(a => a.IDANSWER).ToList();
                var answersToDelete = existingAnswers.Where(a => !answerIdsFromModal.Contains(a.IDANSWER)).ToList();
                db.ANSWERS.RemoveRange(answersToDelete);

                foreach (var answer in answers)
                {
                    if (answer.IDANSWER > 0) 
                    {
                        var existingAnswer = existingAnswers.FirstOrDefault(a => a.IDANSWER == answer.IDANSWER);
                        if (existingAnswer != null)
                        {
                            existingAnswer.ANSWERTEXT = answer.ANSWERTEXT;
                            existingAnswer.ISCORRECT = answer.ISCORRECT;
                            existingAnswer.ANSWERTYPE = answer.ANSWERTYPE;
                        }
                    }
                    else 
                    {
                        answer.IDQUESTION = question.IDQUESTION;
                        db.ANSWERS.Add(answer);
                    }
                }
                await db.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateFillInBlankQuestion(Question question, List<Answers> answers)
        {
            try
            {
                var existingQuestion = await db.QUESTION.FirstOrDefaultAsync(q => q.IDQUESTION == question.IDQUESTION);
                if (existingQuestion == null)
                {
                    return Json(new { success = false, message = "Câu hỏi không tồn tại." });
                }

                existingQuestion.QUESTIONTEXT = question.QUESTIONTEXT;
                existingQuestion.DIFFICULTY = question.DIFFICULTY;
                existingQuestion.CREATEUPDATE = DateTime.Now;
                var existingAnswers = await db.ANSWERS.Where(a => a.IDQUESTION == question.IDQUESTION).ToListAsync();
                foreach (var answer in answers)
                {
                    var existingAnswer = existingAnswers.FirstOrDefault(a => a.IDANSWER == answer.IDANSWER);
                    if (existingAnswer != null)
                    {
                        existingAnswer.ANSWERTEXT = answer.ANSWERTEXT;
                        // Các thuộc tính khác như IS_CORRECT, ANSWERTYPE, BLANKPOSITION không thay đổi
                    }
                }
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
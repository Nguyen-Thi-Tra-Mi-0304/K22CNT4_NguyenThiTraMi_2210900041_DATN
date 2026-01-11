using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Hangfire;
using Hangfire.SqlServer;
using WebThiTracNghiemOnline.Context;
using System.Linq;
using Owin;
using WebMatrix.WebData;
using System.Threading.Tasks;
using WebThiTracNghiemOnline.Models;
using System.Diagnostics;
using System.Data.Entity;
using System.Web.Security;
using System.Web;
namespace WebThiTracNghiemOnline
{
    public class Global : System.Web.HttpApplication
    {
        private WebsiteTracNghiemDBContext db = new WebsiteTracNghiemDBContext();
        private BackgroundJobServer backgroundJobServer;

        protected void Application_Start(object sender, EventArgs e)
        {

            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Hangfire.GlobalConfiguration.Configuration 
                .UseSqlServerStorage("DATATHITRACNGHIEMONLINEConnectionString");

            var options = new BackgroundJobServerOptions
            {
            };
            backgroundJobServer = new BackgroundJobServer(options); 

            RecurringJob.AddOrUpdate(
                "UpdateExamStatusJob",  
                () => UpdateExamStatusProcedure(), 
                Cron.Minutely);
            RecurringJob.AddOrUpdate(
                "SendExamReminderEmails",
                () => SendExamReminderEmailsAsync(),
                Cron.Minutely);

            RouteTable.Routes.MapOwinPath("/hangfire", app => app.UseHangfireDashboard());
        }

        // Phương thức cập nhật trạng thái của Exam
        public void UpdateExamStatusProcedure()
        {
            var exams = db.EXAM.ToList();

            foreach (var exam in exams)
            {
                if (exam.CREATESTART > DateTime.Now)
                {
                    exam.STATUS = 1; // Chưa bắt đầu
                }
                else if (exam.CREATESTART <= DateTime.Now && exam.CREATEEND >= DateTime.Now)
                {
                    exam.STATUS = 2; // Đang làm
                }
                else
                {
                    exam.STATUS = 3; // Đã kết thúc
                }

                if (exam.STATUS == 2 || exam.STATUS == 3)
                {
                    // Lấy danh sách ID câu hỏi liên quan đến bài thi này
                    var questionIds = db.QUESTIONTOEXAM
                        .Where(qte => qte.IDEXAM == exam.IDEXAM)
                        .Select(qte => qte.IDQUESTION)
                        .ToList();

                    // Cập nhật NOEDIT cho các câu hỏi
                    var questionsToUpdate = db.QUESTION
                        .Where(q => questionIds.Contains(q.IDQUESTION))
                        .ToList();

                    foreach (var question in questionsToUpdate)
                    {
                        question.NOEDIT = 1; // Đánh dấu câu hỏi không thể chỉnh sửa
                    }
                }
                db.SaveChanges();
            }
        }

        // Phương thức gửi email khi gần mở bài thi
        public async Task SendExamReminderEmailsAsync()
        {
            var reminderTimeStart = DateTime.Now.AddHours(12);
            var reminderTimeEnd = DateTime.Now.AddHours(13);
            var upcomingExams = await (from e in db.EXAM
                                       join cl in db.CLASS on e.IDCLASS equals cl.ID
                                       join soc in db.STUDENTONCLASS on cl.ID equals soc.IDCLASS
                                       join acc in db.ACCOUNTSTUDENTS on soc.IDSTUDENT equals acc.ID
                                       where e.CREATESTART >= reminderTimeStart && e.CREATESTART < reminderTimeEnd
                                       select new
                                       {
                                           ExamId = e.IDEXAM,
                                           ExamName = e.NAMEEXAM,
                                           ExamStartTime = e.CREATESTART,
                                           NameClass = cl.NAMECLASS,
                                           StudentEmail = acc.EMAIL,
                                           StudentName = acc.ACCOUNT
                                       }).ToListAsync();

            // Gửi email cho từng học sinh trong danh sách
            foreach (var exam in upcomingExams)
            {
                string subject = "Nhắc nhở: Bài thi sắp bắt đầu!";
                string body = $@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <style>
                                    body {{
                                        font-family: Arial, sans-serif;
                                        line-height: 1.6;
                                        color: #333;
                                        margin: 0;
                                        padding: 0;
                                        background-color: #f9f9f9;
                                    }}
                                    .email-container {{
                                        max-width: 600px;
                                        margin: 20px auto;
                                        padding: 20px;
                                        background-color: #ffffff;
                                        border: 1px solid #ddd;
                                        border-radius: 5px;
                                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                                    }}
                                    .header {{
                                        text-align: center;
                                        padding: 10px 0;
                                        background-color: #007bff;
                                        color: #ffffff;
                                        font-size: 24px;
                                        border-radius: 5px 5px 0 0;
                                    }}
                                    .content {{
                                        padding: 20px;
                                    }}
                                    .content p {{
                                        margin: 0 0 15px;
                                    }}
                                    .content strong {{
                                        color: #007bff;
                                    }}
                                    .footer {{
                                        margin-top: 20px;
                                        text-align: center;
                                        font-size: 12px;
                                        color: #999;
                                    }}
                                    .btn {{
                                        display: inline-block;
                                        padding: 10px 20px;
                                        margin: 20px 0;
                                        background-color: #007bff;
                                        color: #ffffff;
                                        text-decoration: none;
                                        border-radius: 5px;
                                        font-size: 16px;
                                    }}
                                    .btn:hover {{
                                        background-color: #0056b3;
                                    }}
                                </style>
                            </head>
                            <body>
                                <div class='email-container'>
                                    <div class='header'>
                                        Nhắc nhở: Bài thi sắp bắt đầu!
                                    </div>
                                    <div class='content'>
                                        <p>Xin chào <strong>{exam.StudentName}</strong>,</p>
                                        <p>
                                            Đây là nhắc nhở rằng bài thi <strong>{exam.ExamName}</strong> của bạn sẽ bắt đầu vào: 
                                            <strong>{exam.ExamStartTime:dd/MM/yyyy HH:mm}</strong>.
                                        </p>
                                        <p>
                                            Lớp học: <strong>{exam.NameClass}</strong>
                                        </p>
                                        <a href='#' class='btn'>Đăng nhập vào hệ thống</a>
                                        <p>Vui lòng kiểm tra hệ thống để chuẩn bị tốt nhất.</p>
                                    </div>
                                    <div class='footer'>
                                        Trân trọng,<br/>
                                        Hệ thống quản lý thi trực tuyến Trung cấp nghề Củ Chi
                                    </div>
                                </div>
                            </body>
                            </html>";
                try
                {
                    await EmailService.SendEmailAsync(exam.StudentEmail, subject, body);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Lỗi khi gửi email cho {exam.StudentEmail}: {ex.Message}");
                }
            }
        }

    }
}

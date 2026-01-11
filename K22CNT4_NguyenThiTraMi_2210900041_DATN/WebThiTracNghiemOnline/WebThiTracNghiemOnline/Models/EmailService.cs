using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Threading.Tasks;

namespace WebThiTracNghiemOnline.Models
{
    public static class EmailService
    {
        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Chức năng này yêu cầu ng dùng nhập mail của mình vào admin không hỗ trợ may chủ SMTP của Gmail, vì vậy bạn cần cấu hình SMTP client với thông tin đăng nhập hợp lệ.
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("dienmailvaoday@gmail.com", "zsbb cgsq dfsae nwwj"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("dienmailvaoday@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                // Gửi email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý lỗi tại đây nếu cần
                throw new Exception("Đã xảy ra lỗi khi gửi email: " + ex.Message);
            }
        }
    }

}
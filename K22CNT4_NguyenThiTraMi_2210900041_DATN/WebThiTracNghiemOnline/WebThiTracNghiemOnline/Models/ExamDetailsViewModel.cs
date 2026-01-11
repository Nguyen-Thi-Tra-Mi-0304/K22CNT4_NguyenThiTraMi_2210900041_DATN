using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class ExamDetailsViewModel
    {
        public Exam Exam { get; set; } // Dữ liệu của bài thi
        public List<ExamAttemptViewModel> ExamAttempts { get; set; } // Danh sách các lần thi

        public string NameExam { get; set; }
        public DateTime? CreateEnd { get; set; }
        public bool ShowAnswers { get; set; }
    }
}
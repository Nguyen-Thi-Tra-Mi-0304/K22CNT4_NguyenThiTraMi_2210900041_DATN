using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class ExamRecordViewModel
    {
        public string SubjectName { get; set; }
        public string ExamName { get; set; }
        public DateTime? TimeEnd { get; set; }
        public DateTime? TimeStart { get; set; }
        public bool ShowAnswers { get; set; }
        public string ClassName { get; set; }
        public double Score { get; set; }
        public int Attempt { get; set; } // Thêm số lần nếu cần
        public int ExamRecordId { get; set; } // Để link đến chi tiết bài thi

    }
}
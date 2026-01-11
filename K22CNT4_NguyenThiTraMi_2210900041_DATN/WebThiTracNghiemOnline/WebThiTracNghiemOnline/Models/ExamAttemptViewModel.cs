using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class ExamAttemptViewModel
    {
        public int AttemptCount { get; set; } // Số lần thi
        public List<ExamRecord> Records { get; set; } // Danh sách học sinh trong lần thi
    }
    public class ExamRecord
    {
        public string AccountStudent {  get; set; }
        public string StudentName { get; set; }
        public string Sex { get; set; }
        public DateTime Birthday { get; set; }
        public double TotalScore { get; set; }
        public bool IsCompleted { get; set; }
    }

}
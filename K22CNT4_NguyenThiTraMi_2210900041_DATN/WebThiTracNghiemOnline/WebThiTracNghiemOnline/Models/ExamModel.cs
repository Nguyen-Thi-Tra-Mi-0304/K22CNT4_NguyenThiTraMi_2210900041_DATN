using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class ExamModel
    {
        public DateTime DateTime { get; set; }
        public string SubjectName { get; set; }
        public string NameExam { get; set; }
        public int ASSIGNMENTTIME { get; set; }
        public int Status { get; set; }
    }
}
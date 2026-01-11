using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class AnswerSubmission
    {
        public int QuestionID { get; set; }
        public int AnswerID { get; set; }
        public string AnswerText { get; set; }

    }
}
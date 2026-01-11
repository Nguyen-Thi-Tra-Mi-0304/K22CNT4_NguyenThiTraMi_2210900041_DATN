using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class QuestionsToExam
    {
        [Key]   
        public int IDQUESTIONTOEXAM { get; set; }
        public int IDEXAM { get; set; }
        public int IDQUESTION {get; set; }
        public int STATUS { get; set; }

    }
}
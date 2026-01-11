using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Answers
    {
        [Key]
        public int IDANSWER { get; set; }
        public int IDQUESTION { get; set; }
        public string ANSWERTEXT { get; set; }
        public Boolean ISCORRECT { get; set; }
        public string ANSWERTYPE { get; set; }
        public int BLANKPOSITION { get; set; }
    }
}
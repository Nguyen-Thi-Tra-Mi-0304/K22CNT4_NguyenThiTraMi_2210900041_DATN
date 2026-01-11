using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Exam
    {
        [Key]
        public int IDEXAM { get; set; }
        public int IDSUBJECT { get; set; }
        public int IDSEMESTER { get; set; }
        public int IDCLASS { get; set; }
        public int IDTEACHER { get; set; }
        public string NAMEEXAM { get; set; }
        public string DESCRIBLE { get; set; }
        public DateTime CREATEAT { get; set; }
        public DateTime CREATEUPDATE { get; set; }
        public DateTime CREATESTART { get; set; }
        public DateTime CREATEEND { get; set; }
        public int ASSIGNMENTTIME { get; set; }
        public int NUMBER { get; set; }
        public string PASSWORD { get; set; }
        public int STATUS { get; set; }
        public bool MIXQUESTION { get; set; }
        public bool MIXANSWERS { get; set; }
        public bool SHOWPOINT { get; set; }
        public bool SHOWANSWERS { get; set; }
    }
}
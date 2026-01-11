using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class TempAnswers
    {
        [Key]
        public int TEMPANSWERID { get; set; }
        public int EXAMID { get; set; }
        public int STUDENTID { get; set; }
        public int QUESTIONID { get; set; }
        public int ANSWERSID { get; set; }
        public string ANSWERSTEXT { get; set; }
        public bool ISSUBMITTED { get; set; }
        public int ATTEMPTCOUNT { get; set; }
        public DateTime DATECREATE { get; set; }
    }
}
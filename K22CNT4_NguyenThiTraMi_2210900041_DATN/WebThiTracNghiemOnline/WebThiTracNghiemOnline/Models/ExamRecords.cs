using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class ExamRecords
    {
        [Key]
        public int RECORDID { get; set; }
        public int STUDENTID { get; set; }
        public int EXAMID { get; set; }
        public DateTime STARTTIME { get; set; }
        public DateTime ENDTIME { get; set; }
        public int ATTEMPTCOUNT { get; set; }
        public double TOTALSCORE { get; set; }
        public bool  ISCOMPLETED{ get; set; }

    }
}
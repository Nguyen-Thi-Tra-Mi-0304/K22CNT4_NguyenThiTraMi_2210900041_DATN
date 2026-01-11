using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Semester
    {
        [Key]
        public int ID { get; set; }
        public int COURSEID { get; set; }
        public string NAMESEMESTER { get; set; }
        public DateTime STARTDAY { get; set; }
        public DateTime ENDDAY { get; set; }
        public DateTime CREATEAT { get; set; }
        public int STT { get; set; }

    }
}
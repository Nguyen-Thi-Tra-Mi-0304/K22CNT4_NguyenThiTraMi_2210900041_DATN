using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class SubjectByTeacher
    {
        [Key]
        public int ID { get; set; }
        public int IDSUBJECT { get; set; }
        public int IDTEACHER { get; set; }
        public DateTime CREATEAT { get; set; }
        public int STT { get; set; }
    }
}
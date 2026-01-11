using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Subject
    {
        [Key]
        public int ID { get; set; }
        public int IDSEMESTER { get; set; }
        public string CODE { get; set; }
        public string NAMESUBJECT { get; set; }
        public int TINCHI { get; set; }
        public string DESCRIBE { get; set; }
        public int STT { get; set; }
        public DateTime CREATEAT { get; set; }
        public DateTime CREATEUPDATE { get; set; }
    }
}
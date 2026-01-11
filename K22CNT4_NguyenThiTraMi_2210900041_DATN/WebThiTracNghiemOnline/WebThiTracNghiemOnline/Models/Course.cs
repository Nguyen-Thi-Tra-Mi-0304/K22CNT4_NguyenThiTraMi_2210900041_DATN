using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Course
    {
        [Key]
        public int ID { get; set; }
        public string NAMECOURSE { get; set; }
        public string DESCRIBLE { get; set; }
        public int STT { get; set; }
        public DateTime CREATEAT { get; set; }

    }
}
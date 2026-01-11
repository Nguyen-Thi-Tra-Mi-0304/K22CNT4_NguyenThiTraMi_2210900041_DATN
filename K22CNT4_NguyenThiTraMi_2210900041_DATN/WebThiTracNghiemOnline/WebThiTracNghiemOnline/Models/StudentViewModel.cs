using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class StudentViewModel
    {
        public int IDINDUSTRY { get; set; }
        public int IDCOURSE { get; set; }
        public string NAMESTUDENT { get; set; }
        public string ACCOUNT { get; set; }
        public string SEX { get; set; }
        public DateTime BIRTHDAY { get; set; }
        public string EMAIL { get; set; }
        public int STT { get; set; }
    }
}
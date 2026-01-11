using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class StudentOnClass
    {
        [Key]
        public int ID { get; set; }
        public int IDSTUDENT { get; set; }
        public int IDCLASS { get; set; }
    }
}
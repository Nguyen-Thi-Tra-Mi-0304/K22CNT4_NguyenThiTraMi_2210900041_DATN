using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class SubjectByTeacherViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Stt {  get; set; }
        public DateTime CreateAt { get; set; }
    }
}
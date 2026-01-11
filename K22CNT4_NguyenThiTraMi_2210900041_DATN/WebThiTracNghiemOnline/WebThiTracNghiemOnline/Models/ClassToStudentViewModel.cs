using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class ClassToStudentViewModel
    {
        public int ID { get; set; }
        public string KEYCLASS { get; set; }
        public string NAMECLASS { get; set; }
        public string TeacherName { get; set; }
        public int STT { get; set; }
        public AccountStudents Student { get; set; }
        public List<ClassToStudentViewModel> Classes { get; set; }
    }
}
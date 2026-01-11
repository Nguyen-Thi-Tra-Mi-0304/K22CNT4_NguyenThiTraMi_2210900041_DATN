using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Models.ViewModels;

namespace WebThiTracNghiemOnline.Models
{
    public class EditExamViewModel
    {
        public Exam Exam { get; set; }
        public IEnumerable<QuestionViewModel> SelectedQuestions { get; set; }
        public SelectList SubjectList { get; set; }
        public SelectList SemesterList { get; set; }
        public SelectList ClassList { get; set; }
    }
}
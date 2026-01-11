using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }
        public double Score { get; set; }
        public bool IsCorrect { get; set; }
        public string Difficulty { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
        public List<int> SelectedAnswersId { get; set; }
        public List<string> UserAnswerTextList { get; set; }
        public List<string> CorrectAnswersTextList { get; set; } = new List<string>(); 

    }

    public class AnswerViewModel
    {
        public int AnswerID { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WebThiTracNghiemOnline.Models
{
    public class Question
    {
        [Key]
        public int IDQUESTION { get; set; }
        public int IDCHAPTER { get; set; }
        public string QUESTIONTEXT { get; set; }
        //Loại câu hỏi(single-choice, multiple-choice, fill-in-the-blank)
        public int QUESTIONTYPE { get; set; }
        public int DIFFICULTY { get; set; }
        public int STT {  get; set; }
        public int NOEDIT { get; set; }
        public DateTime CREATEAT { get; set; }
        public DateTime CREATEUPDATE { get; set; }

    }
}
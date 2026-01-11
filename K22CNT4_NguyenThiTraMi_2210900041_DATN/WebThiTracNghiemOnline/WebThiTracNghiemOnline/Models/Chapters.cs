using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Chapters
    {
        [Key]
        public int IDCHAPTER { get; set; }
        public int IDSUBBYTEACHER { get; set; }
        public string NAMECHAPTER { get; set; }
        public DateTime CREATEAT { get; set; }
        public int STT { get; set; }
    }
}
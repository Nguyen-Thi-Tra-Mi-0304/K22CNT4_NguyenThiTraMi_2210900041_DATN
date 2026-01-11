using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Privilege
    {
        [Key]
        public int ID { get; set; }
        public string NAME { get; set; }
        public string DISCRIBLE { get; set; }
        public DateTime CREATEAT { get; set; }
        public int STT { get; set; }
    }
}
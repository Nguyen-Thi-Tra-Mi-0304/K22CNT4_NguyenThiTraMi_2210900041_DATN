using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebThiTracNghiemOnline.Models
{
    public class Industry
    {
        [Key]
        public int ID { get; set; }
        public string CODE { get; set; }
        public string NAMEINDUSTRY { get; set; }
        public string DISCRIBLR { get; set; }
        public DateTime CREATEAT { get; set; }
        public int STT { get; set; }

    }
}
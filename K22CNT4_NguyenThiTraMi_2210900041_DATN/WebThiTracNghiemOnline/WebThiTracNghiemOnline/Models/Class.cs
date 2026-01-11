using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class Class
    {
        [Key]
        public int ID { get; set; }
        public int IDSEMESTER { get; set; }
        public int IDINDUSTRY {  get; set; }
        public int IDTEACHER {  get; set; }
        public string KEYCLASS { get; set; }
        public string NAMECLASS {  get; set; }
        public string DESCRIBE {  get; set; }
        public DateTime CREATEAT { get; set; }
        public DateTime CREATEUPDATE { get; set; }
        public int STT { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebThiTracNghiemOnline.Models
{
    public class AccountStudents
    {
        [Key]
        public int ID { get; set; }
        public int IDINDUSTRY { get; set; }
        public int IDCOURSE { get; set; }
        public string NAMESTUDENT { get; set; }
        public string ACCOUNT {  get; set; }    
        public string PASS { get; set; }    
        public string SEX { get; set; }
        public string EMAIL { get; set; }

        public DateTime BIRTHDAY { get; set; }
        public string IMAGE { get; set; }
        public int ROLE { get; set; }
        public DateTime CREATEAT { get; set; }
        public DateTime CREATEUPDATE { get; set; }
        public int STT { get; set; }
        public string CURRENTSESSIONID { get; set; }
        public DateTime LASTLOGINTIME   { get; set; }
        public bool ISLOGIN  { get; set; }
        public string PASSWORDRESERTTOKEN { get; set; }
        public DateTime? TOKENEXPIRY  { get; set; }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace WebThiTracNghiemOnline.Models
{
    public class AccountUser

    {
        [Key]
        public int ID { get; set; }
        public string NAMEUSER { get; set; }
        public string ACCOUNT { get; set; }
        public string PASS { get; set; }
        public string IMG { get; set; }
        public string SEX { get; set; }
        public string EMAIL { get; set; }
        public int ROLE { get; set; }
        public DateTime CREATEAT{ get; set; }
        public int STT {  get; set; }
    }
}
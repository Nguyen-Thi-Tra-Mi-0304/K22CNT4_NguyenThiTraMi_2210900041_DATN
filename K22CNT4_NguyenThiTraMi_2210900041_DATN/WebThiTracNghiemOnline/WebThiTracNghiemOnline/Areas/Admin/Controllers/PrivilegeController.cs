using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebThiTracNghiemOnline.Areas.Admin.Data;
using WebThiTracNghiemOnline.Models;

namespace WebThiTracNghiemOnline.Areas.Admin.Controllers
{
    [AuthorizeRole("Admin")]
    public class PrivilegeController : BaseController
    {
        // GET: Admin/Privilege
        public ActionResult ListPrivilege()
        {
            return View(db.PRIVILEGE.Where(n => n.STT == 1).ToList());
        }

        public ActionResult CreatePrivilege()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreatePrivilege(FormCollection f)
        {
            try
            {
                Privilege pri = new Privilege();
                pri.NAME = f["NAME"];
                pri.DISCRIBLE = f["DISCRIBLE"];
                pri.CREATEAT = DateTime.Now;
                pri.STT = 1;
                db.PRIVILEGE.Add(pri);
                db.SaveChanges();
                return RedirectToAction("ListPrivilege");
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return View();
        }

        public ActionResult EditPrivilege(int id)
        {
            var edit = db.PRIVILEGE.FirstOrDefault(n => n.ID == id);
            return View(edit);
        }
        [HttpPost]
        public ActionResult EditPrivilege(int id, FormCollection f)
        {
            try
            {
                var edit = db.PRIVILEGE.FirstOrDefault(n => n.ID == id);
                edit.NAME = f["NAME"];
                edit.DISCRIBLE = f["DISCRIBLE"];
                edit.CREATEAT = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("ListPrivilege");
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            return View();
        }
    }
}
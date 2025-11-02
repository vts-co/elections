using Election.Authorization;
using Election.Dto;
using Election.Models;
using Election.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Election.Controllers
{
    [Authorized]

    public class UsersController : Controller
    {
        ElectionsDbEntities db = new ElectionsDbEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Where(x=>!x.IsDeleted).ToList();
            return View(users);
        }

        public ActionResult Create()
        {
            // نجيب المدارس من جدول الناخبين
            var schools = db.VoterInfoes
                .Select(v => v.School)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.Schools = new SelectList(schools);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
           
                user.Role = 2;
                user.Password = Security.Encrypt(user.Password);
                db.Users.Add(user);
                db.SaveChanges();
                TempData["success"] = "تم حفظ البيانات بنجاح";

                return RedirectToAction("Create");




        }
        public ActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                TempData["warning"] = "تأكد من ادخال البيانات";

                return View(user);

            }

            var schools = db.VoterInfoes.Select(v => v.School).Distinct().OrderBy(s => s).ToList();
            ViewBag.Schools = new SelectList(schools, user.SchoolName);

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
          
                user.Role = 2;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "تم حفظ البيانات بنجاح";

                return RedirectToAction("Index");
            

        }

        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            try
            {
                var user = db.Users.Find(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "المستخدم غير موجود" });
                }

                user.IsDeleted = true;
                user.DeletedOn = DALUtility.GetDateTime();
                db.SaveChanges();

                return Json(new { success = true, message = "تم حذف البيانات بنجاح" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ، تأكد من إدخال البيانات" });
            }
        }

        public ActionResult UserAttendanceReport()
        {
            var report = db.VoterInfoes
                           .Where(v => v.IsAttent && v.AttendBy != null)
                           .GroupBy(v => v.AttendBy)
                           .Select(g => new UserVoterAttendanceReportViewModel
                           {
                               UserName = db.Users.Where(u => u.Id == g.Key).Select(u => u.UserName).FirstOrDefault(),
                               AttendedCount = g.Count()
                           })
                           .OrderByDescending(x => x.AttendedCount)
                           .ToList();

            return View(report);
        }

    }
}
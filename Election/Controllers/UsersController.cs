using Election.Authorization;
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
            var users = db.Users.ToList();
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
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
          
                user.Role = 2;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "تم حفظ البيانات بنجاح";

                return RedirectToAction("Index");
            

        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                var user = db.Users.Find(id);
                if (user == null)
                {
                    TempData["warning"] = "المستخدم غير موجود";

                    return View(user);

                }

                user.IsDeleted = true;
                user.DeletedOn = DALUtility.GetDateTime();
                db.SaveChanges();
                TempData["success"] = "تم حذف البيانات بنجاح";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["warning"] = "تأكد من ادخال البيانات";

                return RedirectToAction("Index");
            }
        }

    }
}
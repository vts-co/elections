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

    public class CandidatesController : Controller
    {
        // GET: Candidates
        ElectionsDbEntities db = new ElectionsDbEntities();

        // GET: Users
        public ActionResult Index()
        {
            var Candidates = db.Candidates.Where(x => !x.IsDeleted).ToList();
            return View(Candidates);
        }

        public ActionResult Create()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Candidate user)
        {

            db.Candidates.Add(user);
            db.SaveChanges();
            TempData["success"] = "تم حفظ البيانات بنجاح";

            return RedirectToAction("Create");




        }
        public ActionResult Edit(int id)
        {
            var Candidates = db.Candidates.Find(id);
            if (Candidates == null)
            {
                TempData["warning"] = "تأكد من ادخال البيانات";

                return View(Candidates);

            }

           

            return View(Candidates);
        }

        [HttpPost]
        public ActionResult Edit(Candidate user)
        {


            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "تم حفظ البيانات بنجاح";

            return RedirectToAction("Index");


        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var user = db.Candidates.Find(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "المرشح غير موجود" });
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

    }
}
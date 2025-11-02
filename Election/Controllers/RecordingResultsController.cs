using Election.Authorization;
using Election.Dto;
using Election.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Election.Controllers
{
    [Authorized]

    public class RecordingResultsController : Controller
    {
        ElectionsDbEntities db = new ElectionsDbEntities();

        // GET: RecordingResults
        public ActionResult Index()
        {
            var data = db.CandidatesStatistics.Include(c => c.Candidate).ToList();
            return View(data);
        }

        // GET: إضافة جديد
        public ActionResult Create()
        {
            ViewBag.CandidateId = new SelectList(db.Candidates.Where(x=>!x.IsDeleted), "Id", "Name");
            return View(new CandidatesStatistic { Date=DateTime.Now});
        }

        // POST: إضافة جديد
        [HttpPost]
        public ActionResult Create(CandidatesStatistic model)
        {
            if (ModelState.IsValid)
            {
                // التحقق من وجود سجل لنفس المرشح ونفس التاريخ
                bool exists = db.CandidatesStatistics.Any(c => c.CandidateId == model.CandidateId
                                                            &&DbFunctions.TruncateTime( c.Date) == DbFunctions.TruncateTime(model.Date));
                if (exists)
                {
                    ModelState.AddModelError("", "تم تسجيل هذا المرشح لهذا اليوم بالفعل.");
                    ViewBag.CandidateId = new SelectList(db.Candidates, "Id", "Name", model.CandidateId);
                    return View(model);
                }

                db.CandidatesStatistics.Add(model);
                db.SaveChanges();
                TempData["success"] = "تم حفظ البيانات بنجاح";
                return RedirectToAction("Index");
            }

            ViewBag.CandidateId = new SelectList(db.Candidates, "Id", "Name", model.CandidateId);
            return View(model);
        }


        // GET: تعديل
        public ActionResult Edit(int id)
        {
            var model = db.CandidatesStatistics.Find(id);
            if (model == null) return HttpNotFound();

            ViewBag.CandidateId = new SelectList(db.Candidates, "Id", "Name", model.CandidateId);
            return View(model);
        }

        // POST: تعديل
        [HttpPost]
        public ActionResult Edit(CandidatesStatistic model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "تم تعديل البيانات بنجاح";
                return RedirectToAction("Index");
            }

            ViewBag.CandidateId = new SelectList(db.Candidates, "Id", "Name", model.CandidateId);
            return View(model);
        }

        // POST: حذف
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var record = db.CandidatesStatistics.Find(id);
            if (record == null)
                return Json(new { success = false, message = "السجل غير موجود" });

            db.CandidatesStatistics.Remove(record);
            db.SaveChanges();
            return Json(new { success = true });
        }

        public ActionResult Statistics()
        {
            // جميع السجلات المضافة
            var stats = db.CandidatesStatistics
                          .Include(c => c.Candidate)
                          .ToList();

            // مجموع عدد الناخبين الكلي
            int totalVoters = stats.Sum(s => s.NumberOfVoters);

            // احسب لكل مرشح: العدد، النسبة
            var result = stats
                .GroupBy(s => s.Candidate)
                .Select(g => new CandidateDto
                {
                    CandidateName = g.Key.Name,
                    TotalVoters = g.Sum(x => x.NumberOfVoters),
                    Percentage = totalVoters == 0 ? 0 : Math.Round((double)g.Sum(x => x.NumberOfVoters) / totalVoters * 100, 2)
                })
                .OrderByDescending(x => x.TotalVoters) // ترتيب تنازلي
                .ToList();

            ViewBag.TotalVoters = totalVoters;
            return View(result);
        }

    }
}
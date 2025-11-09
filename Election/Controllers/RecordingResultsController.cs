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
            ViewBag.Centers = db.VoterInfoes.Select(v => v.Center).Distinct().OrderBy(x => x).ToList();
            ViewBag.Villages = db.VoterInfoes.Select(v => v.Village).Distinct().OrderBy(x => x).ToList();
            ViewBag.Schools = db.VoterInfoes.Select(v => v.School).Distinct().OrderBy(x => x).ToList();

            return View(new CandidatesStatistic { Date=DateTime.Now});
        }

        // POST: إضافة جديد
        [HttpPost]
        public ActionResult Create(CandidatesStatistic model)
        {
           
                // التحقق من وجود سجل لنفس المرشح ونفس التاريخ
                bool exists = db.CandidatesStatistics.Any(c => c.CandidateId == model.CandidateId&&c.School==model.School);
                if (exists)
                {
                    ModelState.AddModelError("", "تم تسجيل هذا المرشح لهذا اليوم بالفعل.");
                    ViewBag.CandidateId = new SelectList(db.Candidates, "Id", "Name", model.CandidateId);
                ViewBag.Centers = db.VoterInfoes.Select(v => v.Center).Distinct().OrderBy(x => x).ToList();
                ViewBag.Villages = db.VoterInfoes.Select(v => v.Village).Distinct().OrderBy(x => x).ToList();
                ViewBag.Schools = db.VoterInfoes.Select(v => v.School).Distinct().OrderBy(x => x).ToList();

                return View(model);
                }

                db.CandidatesStatistics.Add(model);
                db.SaveChanges();
                TempData["success"] = "تم حفظ البيانات بنجاح";
                return RedirectToAction("Index");
            

        }


        // GET: تعديل
        public ActionResult Edit(int id)
        {
            var model = db.CandidatesStatistics.Find(id);
            if (model == null)
                return HttpNotFound();

            // 🔹 جبنا بيانات المركز والقرية بناءً على المدرسة
            var voterInfo = db.VoterInfoes.FirstOrDefault(v => v.School == model.School);
            var selectedCenter = voterInfo?.Center;
            var selectedVillage = voterInfo?.Village;
            var selectedSchool = model.School;

            // 🔹 حفظ القيم المختارة في ViewBag
            ViewBag.SelectedCenter = selectedCenter;
            ViewBag.SelectedVillage = selectedVillage;
            ViewBag.SelectedSchool = selectedSchool;

            // 🔹 تعبئة القوائم
            ViewBag.Centers = db.VoterInfoes.Select(v => v.Center).Distinct().OrderBy(x => x).ToList();
            var centers = db.VoterInfoes
    .Select(v => v.Center)
    .Distinct()
    .ToList();

            ViewBag.Centers = new SelectList(centers);
            var villages = db.VoterInfoes
                .Select(v => v.Village)
                .Distinct()
                .ToList();
            ViewBag.Villages = new SelectList(villages);

            var schools = db.VoterInfoes
                .Select(v => v.School)
                .Distinct()
                .ToList();
            ViewBag.Schools = new SelectList(schools);
            ViewBag.CandidateId = new SelectList(
                db.Candidates
                  .OrderBy(c => c.Name)
                  .ToList(),
                "Id",
                "Name",
                model.CandidateId // ← القيمة اللي المفروض تتعلَّم
            );

            // مش بنحتاج نجيب القرى والمدارس هنا، لأن الـ JS هيجيبهم ديناميكيًا
            return View(model);
        }


        // POST: تعديل
        [HttpPost]
        public ActionResult Edit(CandidatesStatistic model)
        {
            bool exists = db.CandidatesStatistics.Any(c => c.CandidateId == model.CandidateId && c.School == model.School&&c.Id!=model.Id);
            if (exists)
            {
                ModelState.AddModelError("", "تم تسجيل هذا المرشح لهذا اليوم بالفعل.");
                // 🔹 جبنا بيانات المركز والقرية بناءً على المدرسة
                var voterInfo = db.VoterInfoes.FirstOrDefault(v => v.School == model.School);
                var selectedCenter = voterInfo?.Center;
                var selectedVillage = voterInfo?.Village;
                var selectedSchool = model.School;

                // 🔹 حفظ القيم المختارة في ViewBag
                ViewBag.SelectedCenter = selectedCenter;
                ViewBag.SelectedVillage = selectedVillage;
                ViewBag.SelectedSchool = selectedSchool;

                // 🔹 تعبئة القوائم
                ViewBag.Centers = db.VoterInfoes.Select(v => v.Center).Distinct().OrderBy(x => x).ToList();
                var centers = db.VoterInfoes
        .Select(v => v.Center)
        .Distinct()
        .ToList();

                ViewBag.Centers = new SelectList(centers);
                var villages = db.VoterInfoes
                    .Select(v => v.Village)
                    .Distinct()
                    .ToList();
                ViewBag.Villages = new SelectList(villages);

                var schools = db.VoterInfoes
                    .Select(v => v.School)
                    .Distinct()
                    .ToList();
                ViewBag.Schools = new SelectList(schools);
                ViewBag.CandidateId = new SelectList(
                    db.Candidates
                      .OrderBy(c => c.Name)
                      .ToList(),
                    "Id",
                    "Name",
                    model.CandidateId // ← القيمة اللي المفروض تتعلَّم
                );

                return View(model);
            }


            db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["success"] = "تم تعديل البيانات بنجاح";
                return RedirectToAction("Index");
            

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
        public JsonResult GetCandidatesVotes(string center = null, string village = null, string school = null)
        {
            // جلب البيانات مع الفلترة
            var query = db.CandidatesStatistics.AsQueryable();

            if (!string.IsNullOrEmpty(center))
                query = query.Where(x => x.School != null && db.VoterInfoes.Any(v => v.School == x.School && v.Center == center));

            if (!string.IsNullOrEmpty(village))
                query = query.Where(x => x.School != null && db.VoterInfoes.Any(v => v.School == x.School && v.Village == village));

            if (!string.IsNullOrEmpty(school))
                query = query.Where(x => x.School == school);

            var data = query
                .GroupBy(x => x.Candidate.Name)
                .Select(g => new
                {
                    CandidateName = g.Key,
                    Votes = g.Sum(x => x.NumberOfVoters)
                })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Statistic()
        {
            var centers = db.VoterInfoes.Select(v => v.Center).Distinct().OrderBy(x => x).ToList();
            var villages = db.VoterInfoes.Select(v => v.Village).Distinct().OrderBy(x => x).ToList();
            var schools = db.VoterInfoes.Select(v => v.School).Distinct().OrderBy(x => x).ToList();

            ViewBag.Centers = new SelectList(centers);
            ViewBag.Villages = new SelectList(villages);
            ViewBag.Schools = new SelectList(schools);

            return View();
        }
    }
}
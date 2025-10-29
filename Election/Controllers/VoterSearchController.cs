using Election.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Election.Controllers
{
    public class VoterSearchController : Controller
    {
        ElectionsDbEntities db = new ElectionsDbEntities();
        // GET: VoterSearch
        public ActionResult Index()
        {
            ViewBag.Centers = db.VoterInfoes.Select(v => v.Center).Distinct().OrderBy(x => x).ToList();
            ViewBag.Villages = db.VoterInfoes.Select(v => v.Village).Distinct().OrderBy(x => x).ToList();
            ViewBag.Schools = db.VoterInfoes.Select(v => v.School).Distinct().OrderBy(x => x).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult GetVoters()
        {
            var draw = Request.Form["draw"];
            var start = Convert.ToInt32(Request.Form["start"] ?? "0");
            var length = Convert.ToInt32(Request.Form["length"] ?? "10");
            var searchValue = Request.Form["search[value]"] ?? string.Empty;

            var filterCenter = Request.Form["center"] ?? string.Empty;
            var filterVillage = Request.Form["village"] ?? string.Empty;
            var filterSchool = Request.Form["school"] ?? string.Empty;
            var filterName = Request.Form["name"] ?? string.Empty;
            var filterSubcommittee = Request.Form["Subcommittee"] ?? string.Empty;
            var AttendFilter = Request.Form["AttendFilter"] ?? string.Empty;

            var query = db.VoterInfoes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterCenter))
                query = query.Where(v => v.Center == filterCenter);

            if (!string.IsNullOrWhiteSpace(filterVillage))
                query = query.Where(v => v.Village == filterVillage);

            if (!string.IsNullOrWhiteSpace(filterSchool))
                query = query.Where(v => v.School == filterSchool);

            if (!string.IsNullOrWhiteSpace(filterName))
                query = query.Where(v => v.Name.Contains(filterName));

            if (!string.IsNullOrWhiteSpace(filterSubcommittee))
                query = query.Where(v => v.SubCommitteeNumber.Contains(filterSubcommittee));

            if (!string.IsNullOrWhiteSpace(AttendFilter))
            {
                if (AttendFilter == "1")
                    query = query.Where(v => v.IsAttent);
                else if (AttendFilter == "2")
                    query = query.Where(v =>! v.IsAttent);

            }

            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(v =>
                    v.Name.Contains(searchValue) ||
                    v.School.Contains(searchValue) ||
                    v.Village.Contains(searchValue) ||
                    v.Center.Contains(searchValue));
            }

            var recordsTotal = db.VoterInfoes.Count();
            var recordsFiltered = query.Count();


            query = query.OrderBy(v => v.Name);


            var data = query.Skip(start).Take(length)
                .Select(v => new
                {
                    v.Id,
                    v.Serial,
                    v.Name,
                    v.Center,
                    v.Village,
                    v.School,
                    Subcommittee = v.SubCommitteeNumber,
                    Attent = v.IsAttent ? "تم الحضور" : "لم يتم الحضور"
                })
                .ToList();


            return Json(new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsFiltered,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVoterDetails(int id)
        {
            var voter = db.VoterInfoes.FirstOrDefault(v => v.Id == id);
            if (voter == null)
                return Content("<div class='alert alert-warning'>الناخب غير موجود</div>");

            return PartialView("_VoterDetails", voter);
        }

        public JsonResult GetVillages(string center)
        {
            var villages = db.VoterInfoes
                .Where(v => v.Center == center)
                .Select(v => v.Village)
                .Distinct()
                .OrderBy(v => v)
                .ToList();

            return Json(villages, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSchools(string village)
        {
            var schools = db.VoterInfoes
                .Where(v => v.Village == village)
                .Select(v => v.School)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return Json(schools, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult MarkAsAttended(int id)
        {
            try
            {
                var voter = db.VoterInfoes.FirstOrDefault(v => v.Id == id);
                if (voter == null)
                    return Json(new { success = false, message = "الناخب غير موجود" });

                voter.IsAttent = true;
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
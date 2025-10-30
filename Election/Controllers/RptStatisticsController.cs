using Election.Authorization;
using Election.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Election.Controllers
{
    [Authorized]

    public class RptStatisticsController : Controller
    {
        ElectionsDbEntities db = new ElectionsDbEntities();

        // GET: RptStatistics
        public ActionResult Index()
        {
            ViewBag.Centers = db.VoterInfoes.Select(v => v.Center).Distinct().OrderBy(x => x).ToList();
            ViewBag.Villages = db.VoterInfoes.Select(v => v.Village).Distinct().OrderBy(x => x).ToList();
            ViewBag.Schools = db.VoterInfoes.Select(v => v.School).Distinct().OrderBy(x => x).ToList();
            return View();
        }
        public JsonResult GetStatistics(string center, string village, string school)
        {
            var query = db.VoterInfoes.AsQueryable();

            if (!string.IsNullOrEmpty(center))
                query = query.Where(v => v.Center == center);
            if (!string.IsNullOrEmpty(village))
                query = query.Where(v => v.Village == village);
            if (!string.IsNullOrEmpty(school))
                query = query.Where(v => v.School == school);

            var total = query.Count();
            var present = query.Count(v => v.IsAttent == true);
            var absent = total - present;

            double attendanceRate = 0;
            double absenceRate = 0;

            if (total > 0)
            {
                attendanceRate = Math.Round((double)present / total * 100, 2);
                absenceRate = Math.Round((double)absent / total * 100, 2);
            }

            return Json(new
            {
                present,
                absent,
                attendanceRate,
                absenceRate
            }, JsonRequestBehavior.AllowGet);
        }

    }
}

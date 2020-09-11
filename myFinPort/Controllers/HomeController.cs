using myFinPort.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myFinPort.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Dashboard() 
        {
            ViewBag.TimePeriod = "Year-to-date";
            var model = new DashboardVM();
            return View(model);
        }

        public ActionResult DashboardMtd()
        {
            ViewBag.TimePeriod = "Month-to-date";
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = DateTime.Now;
            var model = new DashboardVM(startDate, endDate);
            return View("Dashboard", model);
        }

        public ActionResult Dashboardt30d()
        {
            ViewBag.TimePeriod = "Trailing 30 days";
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;
            var model = new DashboardVM(startDate, endDate);
            return View("Dashboard", model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
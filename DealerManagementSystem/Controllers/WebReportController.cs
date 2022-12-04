using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DealerManagementSystem.Controllers
{
    public class WebReportController : Controller
    {
        // GET: WebReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ValueReport()
        {
            return View();
        }
    }
}
using Cj.web.Models;
using Cj.web.protal.App_Start;
using Cj.web.protal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Cj.web.Controllers
{
    [AuthenticationFilter]
    public class VisitController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        // GET: Visit
        public ActionResult Index()
        {
                return View();
        }
        public ActionResult List()
        {

                ViewBag.region = _db.Database.SqlQuery<DropdownText>(@"Select distinct RegionName as Text,RegionName as Id From t_MarketGroup where ChannelID in
                (Select ChannelID from t_Channel where ChannelID=3)
                and RegionName is not null order By RegionName").ToList();
                ViewBag.fromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.toDate = DateTime.Now.ToString("MM/dd/yyyy");
                //List<DistributionPlanDetails> data = new List<DistributionPlanDetails>();
                return View();
        }
        public ActionResult loadVisitData(string region, string area,
            string zone, string cust,
            DateTime? fromDate, DateTime? toDate)
        {

                if (fromDate == null | toDate == null)
                {
                    fromDate = DateTime.Now.Date;
                    toDate = DateTime.Now.AddDays(1).Date;
                }
                ViewBag.fromDate = Convert.ToDateTime(fromDate).Date;
                toDate = Convert.ToDateTime(toDate).AddDays(1).Date;
            ViewBag.toDate = toDate;
            string sQuery = @"select EmployeeCode,EmployeeName,DesignationName, SUM(NoOfPlan) as NoOfPlan,
            SUM(NoOfVisit) as NoOfVisit,SUM(NoOfInvoice) as NoOfInvoice,SUM(round(NetSales,0)) as NetSales,
            SUM(SalesQty) as SalesQty from V_DistributionPlanDetails 
            WHERE PlanDate between '" + fromDate + "' and '" + toDate + "' and PlanDate < '" + toDate + "'";
            if (!String.IsNullOrWhiteSpace(region) | !String.IsNullOrWhiteSpace(area) |
                    !String.IsNullOrWhiteSpace(zone) |
                    !String.IsNullOrWhiteSpace(cust))
                {
                    if (!String.IsNullOrWhiteSpace(region))
                    {
                        sQuery = sQuery + " and RegionName='" + region + "'";
                    }
                    if (!String.IsNullOrWhiteSpace(area))
                    {
                        sQuery = sQuery + " and Areaid=" + area;
                    }
                    if (!String.IsNullOrWhiteSpace(zone))
                    {
                        sQuery = sQuery + " and TerritoryID=" + zone;
                    }
                     if (!String.IsNullOrWhiteSpace(cust))
                    {
                        sQuery = sQuery + " and CustomerID=" + cust;
                    }
                }
                sQuery = sQuery + " Group by EmployeeCode, EmployeeName, DesignationName ORDER BY EmployeeName DESC;";
                var details = _db.DistributionPlanDetails.SqlQuery(sQuery).ToList();
                return Json(new { data = details }, JsonRequestBehavior.AllowGet);
            

        }

        //public ActionResult loadCustTypes(string custType)
        //{
        //    var sQuery = "Select * From t_CustomerType";
        //    var details = _db.custType.SqlQuery(sQuery).ToList();
        //    return Json(new { data = details }, JsonRequestBehavior.AllowGet);
        //}
        [PermishonFilter(Code = "M47.2.2")]
        public ActionResult DistributionList()
        {

                ViewBag.fromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.toDate = DateTime.Now.ToString("MM/dd/yyyy");
                return View();
        }
        public ActionResult loadDistributionData(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null | toDate == null)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            string sQuery = @"EXEC GetDistributionDayPlanDetail '" + fromDate + @"','" + toDate + @"';";
            var details = _db.Database.SqlQuery<DistributionDayPlanDetail>(sQuery).ToList();
            return Json(new { data = details }, JsonRequestBehavior.AllowGet);
        }
    }
}
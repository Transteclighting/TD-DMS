using Cj.web.Models;
using Cj.web.protal.App_Start;
using Cj.web.protal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cj.web.protal.Controllers
{
    [PermishonFilter(Code = "M47.1.5")]
    [AuthenticationFilter]
    public class ExecutiveController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        // GET: Executive
        public ActionResult Index()
        {
            if (Session["LoggedUserName"] != null)
            {
                var query = @"select distinct EmployeeID as Id ,EmployeeName + '(' +EmployeeCode+ ')' as Text,DesignationName,JobLocationName 
                from v_EmployeeDetails a, t_OutletCommissionEmployeeList b where a.EmployeeID=b.OutletEmployeeID and a.LocationID =(
                select LocationID from t_employee where employeeid=(select EmployeeID from t_user where username='"+ Session["LoggedUserName"] + @"'))
                and EMPStatus in(1,2) and OutletEmployeeType=2 and year(getdate())=b.TYear and month(getdate())=b.TMonth";
                ViewBag.emp = _db.Database.SqlQuery<Dropdown>(query).ToList();
            }
            ViewBag.year = _db.Database.SqlQuery<Dropdown>(@"select distinct CYear as Id,convert(varchar, CYear) as Text  from t_CalendarWeek where cyear>=year(getdate())-1").ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(List<ExecutiveLoad> executive)
        {
            try
            {
               var sql = @"INSERT INTO t_PlanExecutiveDayTargetLoading VALUES";
                foreach (var load in executive)
                {
                    sql = sql + @"  (" + load.cyear + @", " + load.cmonth + @", " + load.weekno + @", '" + load.theDate + @"'," + load.EmployeeID + @"," + load.Load + @"," + load.Amount + @", 0), ";
                }
                var index = sql.LastIndexOf(',');
                sql = sql.Remove(index, 1);
                var status = _db.Database.SqlQuery<List<string>>(sql).ToList();
                TempData["ValidationStatus"] = 1;
                TempData["ValidationMsg"] = "Loading Successfully added.";
            }
            catch (Exception ex)
            {
                TempData["ValidationStatus"] = 2;
                TempData["ValidationMsg"] = ex.Message;
            }
            return RedirectToAction("Index", "Executive");
        }
        public ActionResult loadList(string week="0",string month = "0", string year = "0", string emp = "0")
        {

            var sQuery = @"DECLARE @StartDate datetime,@EndDate datetime,@weekno int,@cmonth int,@cyear int,@emp int;";
            if (!String.IsNullOrWhiteSpace(week) && week != "All")
            {
                sQuery = sQuery + " set @weekno = " + week + ";";
            }
            if (!String.IsNullOrWhiteSpace(month) && month != "All")
            {
                sQuery = sQuery + " set @cmonth = " + month + ";";
            }
            if (!String.IsNullOrWhiteSpace(year) && year != "All")
            {
                sQuery = sQuery + " set @cyear = " + year + ";";
            }
            if (!String.IsNullOrWhiteSpace(emp) && emp != "All")
            {
                sQuery = sQuery + "  set @emp = " + emp + ";";
            }
            sQuery = sQuery + @"select @StartDate = fromdate, @EndDate = todate from t_CalendarWeek where weekno = @weekno and cmonth = @cmonth and cyear = @cyear;
            WITH theDates AS
            (SELECT @StartDate as theDate
            UNION ALL
            SELECT DATEADD(day, 1, theDate)
            FROM theDates
            WHERE DATEADD(day, 1, theDate) <= @EndDate
            )
            SELECT EmployeeID,EmployeeCode,EmployeeName + '(' +EmployeeCode+ ')' as EmployeeName, @weekno as weekno, @cmonth as cmonth, @cyear as cyear,
            REPLACE(CONVERT(NVARCHAR,theDate, 106), ' ', '-') as theDate
            FROM theDates x
            , (select EmployeeID,EmployeeCode, EmployeeName from t_Employee where EmployeeID= @emp) y
            where 1 = 1
            OPTION(MAXRECURSION 0);";
            var details = _db.Database.SqlQuery<ExecutiveLoad>(sQuery).ToList();
            return Json(new { data = details }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadTarget(string week = "0", string month = "0", string year = "0", string emp = "0")
        {
            var sQuery = @"select ROUND(sum(TargetAmount),0) as TargetAmount  from  t_PlanExecutiveLeadTarget
            where TargetType=6";
            //and weekno=" + week + " and TMonth=" + month + " and TYear= " + year + " and SalesPersonID=" + emp;
            if (!String.IsNullOrWhiteSpace(week) && week != "All")
            {
                sQuery = sQuery + " and weekno = " + week;
            }
            if (!String.IsNullOrWhiteSpace(month) && month != "All")
            {
                sQuery = sQuery + " and TMonth = " + month;
            }
            if (!String.IsNullOrWhiteSpace(year) && year != "All")
            {
                sQuery = sQuery + " and TYear = " + year;
            }
            if (!String.IsNullOrWhiteSpace(emp) && emp != "All")
            {
                sQuery = sQuery + "  and SalesPersonID = " + emp;
            }
            double? target = _db.Database.SqlQuery<double?>(sQuery).FirstOrDefault();
            return Json(target, JsonRequestBehavior.AllowGet);
        }
    }
}
using System.Web.Mvc;
using Cj.web.Models;
using Cj.web.protal.Models;
using System.Linq;
using System;
using Cj.web.protal.App_Start;
using System.Web.Script.Serialization;

namespace Cj.web.Controllers
{
    [AuthenticationFilter]
    public class HomeController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        // GET: /Home/
        public ActionResult Index()
        {

            if (TempData["ChangePassMsg"] != null)
            {
                ViewBag.ChangePassStatus = (int)TempData["ChangePassStatus"];
                ViewBag.ChangePassMsg = TempData["ChangePassMsg"].ToString();
            }
            ViewBag.TD = _db.Database.SqlQuery<BUWiseSales>(@"select Description,	Round(Today,0) as Today, Round(LastDay,0) as LastDay,Round(CMTarget,0) as CMTarget,	
                Round(MTDTarget,0) as MTDTarget,	
                Round(ThisMonth,0) as ThisMonth, CASE When CMTarget >0 then round(ThisMonth/ CMTarget *100,0) else 0 end as CMPer,
                CASE When MTDTarget >0 then round(ThisMonth/ MTDTarget *100,0) else 0 end as MTDPer,
                Round(LMTarget,0) as LMTarget,	Round(LastMonth,0) as LastMonth,
                CASE When LMTarget >0 then round(LastMonth/ LMTarget *100,0) else 0 end as LMPer,	
                Round(ThisYear,0) as ThisYear,	Round(LYYTD,0) as LYYTD,	
                CASE When LYYTD >0 then round((ThisYear/ LYYTD *100)-100,0) else 0 end as YTDGth,
                round(LastYear,0) as LastYear,round(YBLY,0) as YBLY,Sort from dwdb.dbo.t_BUWiseSalesNew where Type = 'BU' and Description = 'Transcom Digital'
                ").FirstOrDefault();
            return View();
        }
        //common actions
        [AuthenticationFilter]
        public ActionResult About()
        {
            return View();
        }
        [AuthenticationFilter]
        public ActionResult Contact()
        {
            return View();
        }
        [AuthenticationFilter]
        public ActionResult loadDailySales()
        {
            var sQuery = @"select date,DayofMonth,sum(TargetValue) as Target,sum(Netsale) as Actul,case when sum(TargetValue)<=0 then 0 else (round((sum(Netsale)/sum(TargetValue)*100),0)) end as ValPer from (
                select day(tdate) as Date,cast(day(tdate) as varchar)+'-'+LEFT(DATENAME(MONTH, tdate), 3) as DayofMonth, round(sum(TargetValue),0) as TargetValue , 0 as Netsale
                from TELAddDB.dbo.t_tempPlanMAGTargetQty
                where tdate between getdate()-7 and getdate()
                group by day(tdate),LEFT(DATENAME(MONTH, tdate), 3)
                union all
                select day(invoicedate) as Date,cast(day(invoicedate) as varchar)+'-'+LEFT(DATENAME(MONTH, invoicedate), 3) as DayofMonth,
                0 as TargetValue , round(sum(netsale),0) as Netsale
                from DWDB.dbo.t_salesdatacustomerproduct a, v_sbu b
                where a.customertypeid=b.custtypeid and companyname='TEL'
                and invoicedate between getdate()-7 and getdate()
                group by day(invoicedate),LEFT(DATENAME(MONTH, invoicedate), 3)) x group by date,DayofMonth
                order by date";
            var details = _db.Database.SqlQuery<SalesModel>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        [AuthenticationFilter]
        public ActionResult loadTyd()
        {
            var sQuery = @"Select case when MAGID in (791,792) then 'TV' 
                when MAGID in (811) then 'FRZ' 
                when MAGID in (22) then 'REF' 
                when MAGID in (23) then 'WM' 
                when MAGID in (951) then 'Mobile Phone' 
                when MAGID in (25) then 'RAC' 
                else 'Other' end as MAG,sum(SalesQty) as YTDQty,sum(NetSales) as YTDValue 
                From dwdb.DBO.t_InvoiceWiseSalesDetail a,v_ProductDetails b
                where Company='TEL' and a.ProductID=b.ProductID 
                and Year(InvoiceDate)=year(GETDATE()) and BUID=2
                group by case when MAGID in (791,792) then 'TV' 
                when MAGID in (811) then 'FRZ' 
                when MAGID in (22) then 'REF' 
                when MAGID in (23) then 'WM' 
                when MAGID in (951) then 'Mobile Phone' 
                when MAGID in (25) then 'RAC' 
                else 'Other' end";
            var details = _db.Database.SqlQuery<InvoiceWiseSales>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
    }
}



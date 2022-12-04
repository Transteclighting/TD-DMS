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
    [AuthenticationFilter]
    public class SalesController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        // GET: Sales
        public ActionResult Index()
        {
                return View();
        }
        [PermishonFilter(Code = "M47.1.1")]
        public ActionResult List()
        {
                ViewBag.areas = _db.Database.SqlQuery<Dropdown>(@"Select a.MarketGroupID as Id, MarketGroupDesc as Text 
                from t_MarketGroup a, DWDB.dbo.t_ActiveArea b Where a.MarketGroupID = b.MarketGroupId and 
                Channel = 'TD' order by a.Pos").ToList();
                ViewBag.brand = _db.Database.SqlQuery<Dropdown>(@"Select BrandId as Id, BrandDesc as Text from t_Brand Where BrandLevel = 1 
                and IsActive = 1 order by BrandPos").ToList();
                ViewBag.pg = _db.Database.SqlQuery<Dropdown>(@"select PdtGroupID as Id, PdtGroupName as Text from t_ProductGroup 
                where PdtGroupType = 1 and IsActive = 1 order by POS").ToList();
                ViewBag.channel = _db.Database.SqlQuery<Dropdown>(@"Select distinct SortOrder as Id,ChannelShortName as Text From t_Channel where SBUID=2 order by SortOrder").ToList();
                ViewBag.fromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.toDate = DateTime.Now.ToString("MM/dd/yyyy");
                return View();
        }
        [PermishonFilter(Code = "M47.1.2")]
        public ActionResult SkuWiseSalesList()
        {
            //ViewBag.areas = _db.Database.SqlQuery<Dropdown>(@"Select a.MarketGroupID as Id, MarketGroupDesc as Text 
            //    from t_MarketGroup a, DWDB.dbo.t_ActiveArea b Where a.MarketGroupID = b.MarketGroupId and 
            //    Channel = 'TD' order by a.Pos").ToList();
            //ViewBag.brand = _db.Database.SqlQuery<Dropdown>(@"Select BrandId as Id, BrandDesc as Text from t_Brand Where BrandLevel = 1 
            //    and IsActive = 1 order by BrandPos").ToList();
            ViewBag.year = _db.Database.SqlQuery<Dropdown>(@"select distinct CYear as Id,convert(varchar, CYear) as Text  from t_CalendarWeek where cyear>=year(getdate())-1").ToList();
            ViewBag.pg = _db.Database.SqlQuery<Dropdown>(@"select PdtGroupID as Id, PdtGroupName as Text from t_ProductGroup 
                where PdtGroupType = 1 and IsActive = 1 order by POS").ToList();
            //ViewBag.channel = _db.Database.SqlQuery<Dropdown>(@"Select distinct SortOrder as Id,ChannelShortName as Text From t_Channel where SBUID=2 order by SortOrder").ToList();
            ViewBag.fromDate = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.toDate = DateTime.Now.ToString("MM/dd/yyyy");
            return View();
        }
        public ActionResult loadSKUdata(string brand, string pg, string mag, string asg, string ag, string channel,
            string week, string year, string month)
        {
            string sQuery = @"select AreaName,TerritoryName,ShowroomCode,TYear,TMonth,WeekNo,
            PGName,MAGName,ASGNAme,AGName,Branddesc,ProductCode,ProductName,sum(TargetQty) as TargetQty,sum(SalesQty) as SalesQty from 
            (
            select tgt.TYear,tgt.TMonth,tgt.WeekNo,tgt.warehouseid,tgt.customerid,tgt.ShowroomCode,tgt.ProductID,
            isnull(TargetQty,0) as TargetQty,isnull(SalesQty,0) as SalesQty from
            --TGT--
            (select TYear,TMonth,WeekNo,warehouseid,a.customerid,ShowroomCode,ProductID,
            TargetQty 
            from TELSysDB.dbo.t_PlanSKUWeekTargetQty a, TELSysDB.dbo.t_Showroom b 
            where a.CustomerID=b.customerid and IsDepot=0 and IsPOSActive=1) tgt
            --end TGT--
            left outer join
            --Sales--
            (select year(invoicedate) as TYear,month(invoicedate) as TMonth,weekno,a.warehouseid,b.customerid,showroomcode,ProductID, SalesQty 
            from DWDB.dbo.t_salesdatacustomerproduct a, TELSysDB.dbo.t_showroom b, TELSysDB.dbo.t_CalendarWeek c 
            where a.InvoiceDate between fromdate and todate and a.warehouseid=b.warehouseid and companyname='TEL' 
            and IsDepot=0 and IsPOSActive=1) sls on(tgt.TYear=sls.TYear
            and tgt.TMonth=sls.Tmonth and tgt.WeekNo=sls.WeekNo and tgt.warehouseid=sls.warehouseid and tgt.productid=sls.productid)
            --end Sales--
            ) TGTACH join TELSysDB.dbo.v_CustomerDetails cd
            on(TGTACH.customerid=cd.customerid)
            join TELSysDB.dbo.v_ProductDetails pd on(TGTACH.productid=pd.productid)
            where  1=1 
            ";
            if (!String.IsNullOrWhiteSpace(brand) && brand != "All")
            {
                sQuery = sQuery + "  and BrandID='" + brand + "'";
            }

            if (!String.IsNullOrWhiteSpace(pg) && pg != "All")
            {
                sQuery = sQuery + " and PGName='" + pg + "'";
            }
            if (!String.IsNullOrWhiteSpace(brand) && brand != "All")
            {
                sQuery = sQuery + " and BrandName='" + brand + "'";
            }
            if (!String.IsNullOrWhiteSpace(pg) && pg != "All")
            {
                sQuery = sQuery + " and PGName='" + pg + "'";
            }
            if (!String.IsNullOrWhiteSpace(mag) && mag != "All")
            {
                sQuery = sQuery + " and MAGName='" + mag + "'";
            }
            if (!String.IsNullOrWhiteSpace(asg) && asg != "All")
            {
                sQuery = sQuery + " and ASGName='" + asg + "'";
            }
            if (!String.IsNullOrWhiteSpace(ag) && ag != "All")
            {
                sQuery = sQuery + " and AGName='" + ag + "'";
            }

            if (!String.IsNullOrWhiteSpace(week) && week != "All")
            {
                sQuery = sQuery + " and WeekNo=" + week;
            }
            if (!String.IsNullOrWhiteSpace(month) && month != "All")
            {
                sQuery = sQuery + " and TMonth=" + month;
            }
            if (!String.IsNullOrWhiteSpace(year) && year != "All")
            {
                sQuery = sQuery + " and TYear=" + year;
            }
            sQuery = sQuery + " group by AreaName,TerritoryName,ShowroomCode,TYear,TMonth,WeekNo,PGName,MAGName,ASGNAme,AGName,Branddesc,ProductCode,ProductName";
            var details = _db.Database.SqlQuery<SkuWiseSales>(sQuery).ToList();
            return Json(new { data = details }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loaddata(string area,
        string zone,  string outlet, string brand, string pg, string channel,
        DateTime? fromDate, DateTime? toDate)
            {

                if (fromDate == null | toDate == null)
                {
                    fromDate = DateTime.Now;
                    toDate = DateTime.Now.AddDays(1);
                }
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate =  Convert.ToDateTime(toDate).AddDays(1);
            string sQuery = @"Select (MAGName+'-'+BrandName) MAGName, round(sum(TargetQty),0) as TargetQty,
            sum(SalesQty) as SalesQty,
            round(CASE When sum(TargetQty)>0 then (cast(sum(SalesQty)as float)/sum(TargetQty)*100) else 0 end,0) as 'QtyPer',
            round(sum(TargetAmount),0) as TargetValue, round(sum(NetSales),0) as NetSale,
            round(CASE When sum(TargetAmount)>0 then (sum(NetSales)/sum(TargetAmount)*100) else 0 end,0) as 'ValPer'
            From t_DailySalesPersonWiseTGTvsActual
            where BUName='Transcom Digital' and
            AllDays between '"+fromDate+@"' and '"+toDate+ @"'
            and AllDays<'" + toDate + @"'
            and (TargetQty + SalesQty + TargetAmount + NetSales) <> 0";
            if (!String.IsNullOrWhiteSpace(area) |
                        !String.IsNullOrWhiteSpace(zone) |
                        !String.IsNullOrWhiteSpace(outlet) |
                        !String.IsNullOrWhiteSpace(brand) |
                        !String.IsNullOrWhiteSpace(pg) |
                        !String.IsNullOrWhiteSpace(channel))
                {
                    //sQuery = sQuery + " where ";
                    if (!String.IsNullOrWhiteSpace(area) && area != "All")
                    {
                        sQuery = sQuery + " and AreaName='" + area + "'";
                    }

                    if (!String.IsNullOrWhiteSpace(zone) && zone != "All")
                    {
                        sQuery = sQuery + " and TerritoryName='" + zone + "'";
                    }
                    if (!String.IsNullOrWhiteSpace(brand) && brand != "All")
                    {
                        sQuery = sQuery + " and BrandName='" + brand + "'";
                    }
                    if (!String.IsNullOrWhiteSpace(pg) && pg != "All")
                    {
                        sQuery = sQuery + " and PGName='" + pg + "'";
                    }
                    if (!String.IsNullOrWhiteSpace(outlet) && outlet != "All" && outlet !="null")
                    {
                        sQuery = sQuery + " and Location='" + outlet + "'";
                    }
                    if (!String.IsNullOrWhiteSpace(channel) && channel != "All")
                    {
                        sQuery = sQuery + " and SubChannelShortName='" + channel + "'";
                    }
                }
                sQuery = sQuery + " group by MAGName+'-'+BrandName order by MAGName + '-' + BrandName desc";
                var details = _db.Sales.SqlQuery(sQuery).ToList();
                return Json(new { data = details }, JsonRequestBehavior.AllowGet);


            }

    }
}
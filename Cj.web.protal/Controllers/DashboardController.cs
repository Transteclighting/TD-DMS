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
    public class DashboardController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        private readonly  Library _library = new Library();

        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult WeeklyReviewDashboard()
        {
            return View();
        }
        public ActionResult OutletDashboard()
        {
            ViewBag.areas = _db.Database.SqlQuery<Dropdown>(@"Select a.MarketGroupID as Id, MarketGroupDesc as Text 
                from t_MarketGroup a, DWDB.dbo.t_ActiveArea b Where a.MarketGroupID = b.MarketGroupId and 
                Channel = 'TD' order by a.Pos").ToList();
            return View();
        }
        public ActionResult SalesDashboard()
        {
            ViewBag.brand = _db.Database.SqlQuery<Dropdown>(@"Select BrandId as Id, BrandDesc as Text from t_Brand Where BrandLevel = 1 
                and IsActive = 1 order by BrandPos").ToList();
            ViewBag.PGCat = _db.Database.SqlQuery<DropdownText>(
                @"select distinct PGCategory as Id, PGCategory as Text 
            from t_ProductGroup where PdtGroupType=1 and PGCategory<>'Service Charges'"
            ).ToList();
            return View();
        }
        public ActionResult StockDashboard()
        {
            ViewBag.brand = _db.Database.SqlQuery<Dropdown>(@"Select BrandId as Id, BrandDesc as Text from t_Brand Where BrandLevel = 1 
                and IsActive = 1 order by BrandPos").ToList();
            ViewBag.PGCat = _db.Database.SqlQuery<DropdownText>(
                @"select distinct PGCategory as Id, PGCategory as Text 
            from t_ProductGroup where PdtGroupType=1 and PGCategory<>'Service Charges'"
            ).ToList();
            var stockQuery = @"Select PGCategory, PGName, MAGName,
            ROUND(Sum(CASE When StockType = 'Saleable' then(StockQty - DisplayStock) else 0 end), 0) as 'Saleable', 
            ROUND(Sum(DisplayStock), 0) as DisplayStock,
            ROUND(Sum(CASE When StockType != 'Saleable' then StockQty else 0 end), 0) as 'Defective',
            ROUND(Sum(StockQty), 0) as StockQty, 
            ROUND(Sum(StockValue), 0) as StockValue From v_StockAging
            Group by PGCategory, PGName, MAGName";
            ViewBag.StockAging = _db.Database.SqlQuery<StockAgingChartModel>(stockQuery).ToList();
            return View();
        }
        public ActionResult OPSDashboard()
        {

            return View();
        }
        public ActionResult CATDashboard()
        {
            return View();
        }
        public ActionResult LeadDashboard()
        {
            return View();
        }

        public ActionResult loadLeadData()
        {
            var sQuery = @"select TYear,TMonth,WeekNo,TDate,ChannelShortName as Channel,ShowroomCode,EmployeeCode,EmployeeName,DesignationName,PGCategory,PGName,MAGName,BrandDesc,
            sum(TargetQty) as TargetQty,sum(TargetAmount) as TargetAmount ,
            sum(LeadQty) as LeadQty,sum(LeadAmount) as LeadAmount, sum(ConverttoSalesQty) as ConverttoSalesQty,sum(ConverttoSalesAmount) as ConverttoSalesAmount,
            sum(TotalSalesQty) as TotalSalesQty,sum(TotalNetSale) as TotalNetSale
            from (
            select TYear,TMonth,WeekNo,getdate() as TDate,ChannelShortName,ShowroomCode,SalesPersonID,PGCategory,PGName,MAGName,BrandDesc,TargetQty,TargetAmount ,
            0 as LeadQty,0 as LeadAmount, 0 as ConverttoSalesQty,0 as ConverttoSalesAmount,
            0 as TotalSalesQty,0 as TotalNetSale
            from TELSysDB.dbo.t_PlanExecutiveleadTarget a,
            (select a.PdtGroupID as MAGID,a.PdtGroupName as MAGName,b.PdtGroupName as PGName,b.PGCategory from t_ProductGroup a, t_ProductGroup b
            where a.ParentID=b.PdtGroupID and a.PdtGroupType=2) b, t_Brand c  , t_showroom d, t_Channel e
            where a.MAGID=b.MAGID and a.BrandID=c.BrandID and a.CustomerID=d.CustomerID and a.Channel=e.ChannelID and TYear=2022 and targettype=5
            union all
            select year(LeadDate) as TYear,month(LeadDate) as TMonth,WeekNo,leaddate as TDate,case CustomerType when 3 then 'Corporate' else 'Retail' end as Channel,
             ShowroomCode,SalesPersonID,PGCategory,PGName,MAGName,BrandDesc,
            0 as TargetQty,0 as TargetAmount,Qty as LeadQty,LeadAmount, 0 as ConverttosalesQty,0 as ConverttosalesAmount,
            0 as TotalSalesQty,0 as TotalNetSale
            from t_SalesLeadManagement a, v_productdetails b, t_CalendarWeekSales c, t_showroom d
            where a.ProductID=b.ProductID and a.WarehouseID=d.WarehouseID and a.LeadDate between fromdate and ToDate and year(leaddate)=2022
            union all
            select Year(b.InvoiceDate) as TYear,month(b.invoicedate) as TMonth,WeekNo,b.invoiceDate as TDate,ChannelShortName,ShowroomCode,b.SalesPersonID,PGCategory ,
            PGName,MAGName,BrandDesc,0 as TargetQty,0 as TargetAmount ,0 as LeadQty,0 as LeadAmount, 
            SalesQty+FreeQty as ConverttosalesQty,NetSales as ConverttosalesAmount,
            0 as TotalSalesQty,0 as TotalNetSale
            from t_SalesLeadManagement a, DWDB.dbo.t_InvoiceWiseSalesDetail b , t_CalendarWeekSales c, v_SBU d, v_ProductDetails e, t_Showroom f
            where a.InvoiceNo=b.InvoiceNo and b.InvoiceDate between FromDate and ToDate and year(b.invoicedate)=2022
            and b.CustomerTypeID=d.CustTypeID and b.ProductID=e.ProductID and b.WarehouseID=f.WarehouseID
            union all
            select Year(b.InvoiceDate) as TYear,month(b.invoicedate) as TMonth,WeekNo,invoicedate as TDate,ChannelShortName,ShowroomCode,b.SalesPersonID,PGCategory ,
            PGName,MAGName,BrandDesc,0 as TargetQty,0 as TargetAmount ,0 as LeadQty,0 as LeadAmount, 0 as ConverttosalesQty,0 as ConverttosalesAmount,
            SalesQty+FreeQty as TotalSalesQty,NetSales as TotalNetSale
            from  DWDB.dbo.t_InvoiceWiseSalesDetail b , t_CalendarWeekSales c, v_SBU d, v_ProductDetails e, t_Showroom f
            where b.InvoiceDate between FromDate and ToDate and year(InvoiceDate)=2022
            and b.CustomerTypeID=d.CustTypeID and b.ProductID=e.ProductID and b.WarehouseID=f.WarehouseID) x, v_EmployeeDetails y
            where x.SalesPersonID=y.EmployeeID
            group by TYear,TMonth,WeekNo,TDate,ChannelShortName,ShowroomCode,EmployeeCode,EmployeeName,DesignationName,PGCategory,PGName,MAGName,BrandDesc
            ";
            var details = _db.Database.SqlQuery<LeadChartModel>(sQuery).ToList();
            return new Library().ConvertTOJson(details);
        }
        public ActionResult loadOutletChart(string brand, string pg, string pgcag, string asg, string ag, string channel,
            string week, string year, string month, string area, string zone, string outlet)
        {
            var sQuery = @"Select AreaName,TerritoryName,ShowroomCode,EmployeeCode,EmployeeName,
            DesignationName,WeekNo,CMonth,PGCategory,PGName,MAGName,Brand,
            sum(TYearSalesQty) TYearSalesQty,sum(TYearInvoiceAmount) TYearInvoiceAmount,
            sum(TYearNetSale) TYearNetSale,sum(LYearSalesQty) LYearSalesQty,
            sum(LYearInvoiceAmount) LYearInvoiceAmount,sum(LYearNetSale) LYearNetSale,
            sum(TYearTargetQty) TYearTargetQty,sum(TYearTargetAmount) TYearTargetAmount
            From 
            (
            Select WarehouseID,PGCategory,PGName,MAGName,BrandDesc as Brand,
            WeekNo,CMonth,SalesPersonID,
            case when Year(InvoiceDate)=year(GETDATE()) then (SalesQty+FreeQty) else 0 end as TYearSalesQty, 
            case when Year(InvoiceDate)=year(GETDATE()) then (GrossSales+OtherCharge-Discount) else 0 end as TYearInvoiceAmount, 
            case when Year(InvoiceDate)=year(GETDATE()) then (NetSales) else 0 end as TYearNetSale,
            case when Year(InvoiceDate)=year(GETDATE())-1 then (SalesQty+FreeQty) else 0 end as LYearSalesQty, 
            case when Year(InvoiceDate)=year(GETDATE())-1 then (GrossSales+OtherCharge-Discount) else 0 end as LYearInvoiceAmount, 
            case when Year(InvoiceDate)=year(GETDATE())-1 then (NetSales) else 0 end as LYearNetSale,
            0 TYearTargetQty,0 TYearTargetAmount
            From DWDB.dbo.t_InvoiceWiseSalesDetail a,t_CalendarWeekSales b,
            v_ProductDetails c
            where Year(InvoiceDate) in (year(GETDATE()),year(GETDATE())-1)
            and Company='TEL' and a.InvoiceDate between b.FromDate and b.ToDate
            and a.ProductID=c.ProductID and BUID=2
            Union All
            Select WarehouseID,PGCategory,PGName,MAGName,BrandDesc as Brand,
            WeekNo,TMonth,SalesPersonID,
            0 as TYearSalesQty,0 as TYearInvoiceAmount,0 as TYearNetSale,
            0 as LYearSalesQty,0 as LYearInvoiceAmount,0 as LYearNetSale,
            a.TargetQty as TYearTargetQty,a.TargetAmount as TYearTargetAmount
            From t_PlanExecutiveLeadTarget a,t_Showroom b,
            (select distinct PGCategory,PGName,MAGName,MAGID from v_ProductGroup) c,t_Brand d
            where TargetType=6 and TYear=YEAR(GETDATE())
            and a.CustomerID=b.CustomerID and Channel in (4,13)
            and a.MAGID=c.MAGID and a.BrandID=d.BrandID
            ) Sales
            inner join v_EmployeeDetails ed on Sales.SalesPersonID=ed.EmployeeID
            inner join t_Showroom Sh on Sales.WarehouseID=Sh.WarehouseID
            inner join v_CustomerDetails CD on Sh.CustomerID=CD.CustomerID
            where 1=1 
            ";
            if (!string.IsNullOrEmpty(outlet))
            {
                sQuery += @" and ShowroomCode='DMR'";
            }

            if (!string.IsNullOrEmpty(pgcag))
            {
                sQuery += @" and PGCategory = 'Category-1' ";
            }
            if (!string.IsNullOrEmpty(pg))
            {
                sQuery += @" and PGName='Electronics' ";
            }
            if (!string.IsNullOrEmpty(brand))
            {
                sQuery += @" and Brand = 'Samsung'";
            }
            sQuery += @" group by AreaName,TerritoryName,ShowroomCode,EmployeeCode,EmployeeName,
            DesignationName,WeekNo,CMonth,PGCategory,PGName,MAGName,Brand";
            var details = _db.Database.SqlQuery<MainChartModel>(sQuery).ToList();


            var stockQuery = @"Select* From v_StockAging where 1=1 ";
            if (!string.IsNullOrEmpty(outlet))
            {
                stockQuery += @" and WarehouseCode='DGC'";
            }

            var stockAging = _db.Database.SqlQuery<MainChartModel>(stockQuery).ToList();


            var pgWise = details.GroupBy(c => c.PGName).Select(g => new PGwiseChartModel
            {
                PGName = g.Key,
                TYearNetSale = g.Sum(x => x.TYearNetSale),
                TYearTargetAmount = g.Sum(x => x.TYearTargetAmount)
            }).ToList();
            var magWise = details.GroupBy(c => c.MAGName).Select(g => new MAGwiseChartModel
            {
                MAGName = g.Key,
                TYearNetSale = g.Sum(x => x.TYearNetSale),
                TYearTargetAmount = g.Sum(x => x.TYearTargetAmount)
            }).ToList();
            var areaWise = details.GroupBy(c => c.AreaName).Select(g => new AreawiseChartModel
            {
                AreaName = g.Key,
                TYearNetSale = g.Sum(x => x.TYearNetSale),
                TYearTargetAmount = g.Sum(x => x.TYearTargetAmount)
            }).ToList();

            var catWise = details.GroupBy(c => c.PGCategory).Select(g => new CatwiseChartModel
            {
                PGCategory = g.Key,
                TYearNetSale = g.Sum(x => x.TYearNetSale),
                TYearTargetAmount = g.Sum(x => x.TYearTargetAmount)
            }).ToList();

            var natWise = new NationalwiseChartModel
            {
                NationalName = "National",
                TYearNetSale = details.Sum(x => x.TYearNetSale),
                TYearTargetAmount = details.Sum(x => x.TYearTargetAmount)
            };

            ChartViewModel chartViewModel = new ChartViewModel();
            chartViewModel.PGwiseChartModel = pgWise;
            chartViewModel.MAGwiseChartModel = magWise;
            chartViewModel.AreawiseChartModel = areaWise;
            chartViewModel.CatwiseChartModel = catWise;
            chartViewModel.NationalwiseChartModel = natWise;

            return new Library().ConvertTOJson(chartViewModel);
        }
        public ActionResult loadSailsChart(string brand, string pg, string pgcat, string month, string year)
        {
            var sQuery = @"Select AreaShortName as AreaName,TerritoryName,ShowroomCode,EmployeeCode,EmployeeName,
            Cast(AreaShortName as varchar) +'(W-'+ Cast(WeekNo as varchar) +')' AS WeekName,
            DesignationName,WeekNo,CMonth,PGCategory,PGName,MAGName,Brand,
            ROUND(sum(TYearSalesQty),0) TYearSalesQty,
            ROUND(sum(TYearInvoiceAmount),0) TYearInvoiceAmount,
            ROUND(sum(TYearNetSale),0) TYearNetSale,
            ROUND(sum(LYearSalesQty),0) LYearSalesQty,
            ROUND(sum(LYearInvoiceAmount),0) LYearInvoiceAmount,
            ROUND(sum(LYearNetSale),0) LYearNetSale,
            ROUND(sum(TYearTargetQty),0) TYearTargetQty,
            ROUND(sum(TYearTargetAmount),0) TYearTargetAmount
            From 
            (
            Select WarehouseID,PGCategory,PGName,MAGName,BrandDesc as Brand,
            WeekNo,CMonth,SalesPersonID,
            case when Year(InvoiceDate)={0} then (SalesQty+FreeQty) else 0 end as TYearSalesQty, 
            case when Year(InvoiceDate)={0} then (GrossSales+OtherCharge-Discount) else 0 end as TYearInvoiceAmount, 
            case when Year(InvoiceDate)={0} then (NetSales) else 0 end as TYearNetSale,
            case when Year(InvoiceDate)={0}-1 then (SalesQty+FreeQty) else 0 end as LYearSalesQty, 
            case when Year(InvoiceDate)={0}-1 then (GrossSales+OtherCharge-Discount) else 0 end as LYearInvoiceAmount, 
            case when Year(InvoiceDate)={0}-1 then (NetSales) else 0 end as LYearNetSale,
            0 TYearTargetQty,0 TYearTargetAmount
            From DWDB.dbo.t_InvoiceWiseSalesDetail a,t_CalendarWeekSales b,
            v_ProductDetails c
            where Year(InvoiceDate) in ({0},{0}-1)
            and Company='TEL' and a.InvoiceDate between b.FromDate and b.ToDate
            and a.ProductID=c.ProductID and BUID=2
            Union All
            Select WarehouseID,PGCategory,PGName,MAGName,BrandDesc as Brand,
            WeekNo,TMonth,SalesPersonID,
            0 as TYearSalesQty,0 as TYearInvoiceAmount,0 as TYearNetSale,
            0 as LYearSalesQty,0 as LYearInvoiceAmount,0 as LYearNetSale,
            a.TargetQty as TYearTargetQty,a.TargetAmount as TYearTargetAmount
            From t_PlanExecutiveLeadTarget a,t_Showroom b,
            (select distinct PGCategory,PGName,MAGName,MAGID from v_ProductGroup) c,t_Brand d
            where TargetType=6 and TYear={0}
            and a.CustomerID=b.CustomerID and Channel in (4,13)
            and a.MAGID=c.MAGID and a.BrandID=d.BrandID
            ) Sales
            inner join v_EmployeeDetails ed on Sales.SalesPersonID=ed.EmployeeID
            inner join t_Showroom Sh on Sales.WarehouseID=Sh.WarehouseID
            inner join v_CustomerDetails CD on Sh.CustomerID=CD.CustomerID
            where ShowroomCode!='GOS'
            ";
            if (string.IsNullOrEmpty(year))
            {
                sQuery = string.Format(sQuery, "YEAR(GETDATE())");
            }
            else
            {
                sQuery = string.Format(sQuery, year);

            }
            if (string.IsNullOrEmpty(month))
            {
                sQuery += @" and CMonth=MONTH(GETDATE()) ";
            }
            else
            {
                sQuery += @" and CMonth=" + month;

            }
            if (!string.IsNullOrEmpty(pgcat) && pgcat != "All")
            {
                sQuery += @" and PGCategory = '" + pgcat + "' ";
            }
            if (!string.IsNullOrEmpty(pg) && pg != "All")
            {
                sQuery += @" and PGName='" + pg + "' ";
            }
            if (!string.IsNullOrEmpty(brand) && brand != "All")
            {
                sQuery += @" and Brand = '" + brand + "'";
            }
            sQuery += @" group by  AreaShortName,TerritoryName,ShowroomCode,EmployeeCode,EmployeeName,
            DesignationName,WeekNo,CMonth,PGCategory,PGName,MAGName,Brand order by AreaName DESC";
            var details = _db.Database.SqlQuery<MainChartModel>(sQuery).ToList();
            var pgWise = details.GroupBy(c => c.PGName).Select(g => new PGwiseChartModel
            {
                PGName = g.Key,
                TYearNetSale = _library.ConvertTOMillion(g.Sum(x => x.TYearNetSale)),
                TYearTargetAmount = _library.ConvertTOMillion(g.Sum(x => x.TYearTargetAmount))
            }).ToList();
            var magWise = details.GroupBy(c => c.MAGName).Select(g => new MAGwiseChartModel
            {
                MAGName = g.Key,
                TYearNetSale = _library.ConvertTOMillion(g.Sum(x => x.TYearNetSale)),
                TYearTargetAmount = _library.ConvertTOMillion(g.Sum(x => x.TYearTargetAmount))
            }).ToList();
            var areaWise = details.GroupBy(c => c.AreaName).Select(g => new AreawiseChartModel
            {
                AreaName = g.Key,
                TYearNetSale = _library.ConvertTOMillion(g.Sum(x => x.TYearNetSale)),
                TYearTargetAmount = _library.ConvertTOMillion(g.Sum(x => x.TYearTargetAmount))
            }).ToList();

            var catWise = details.GroupBy(c => c.PGCategory).Select(g => new CatwiseChartModel
            {
                PGCategory = g.Key,
                TYearNetSale = _library.ConvertTOMillion(g.Sum(x => x.TYearNetSale)),
                TYearTargetAmount = _library.ConvertTOMillion(g.Sum(x => x.TYearTargetAmount))
            }).ToList();

            var natWise = new NationalwiseChartModel
            {
                NationalName = "National",
                TYearNetSale = _library.ConvertTOMillion(details.Sum(x => x.TYearNetSale)),
                TYearTargetAmount = _library.ConvertTOMillion(details.Sum(x => x.TYearTargetAmount))
            };
            var monthWise = details.GroupBy(c => c.WeekName).Select(g => new MonthwiseChartModel
            {
                WeekName = g.Key,
                TYearNetSale = _library.ConvertTOMillion(g.Sum(x => x.TYearNetSale)),
                TYearTargetAmount = _library.ConvertTOMillion(g.Sum(x => x.TYearTargetAmount))


            }).ToList();
            var dailyQuery = @"SELECT Area,
            Territory,
            ShowroomCode,
            TotalDays.WeekNo,
            TotalDays.CMonth,
            TotalDays.CYear,
            REPLACE(CONVERT(NVARCHAR,TDate, 106), ' ', '-') AS TDate,
            TDayName,
            ROUND(isnull(CASE
            WHEN TDayName=WeeklyHoliday THEN 0
            ELSE WeekDayTarget
            END, 0), 0) WeekDayTarget,
            ROUND(isnull(NetSales, 0), 0) NetSales
            FROM
            (SELECT AreaShortName AS Area,
            TerritoryShortName AS Territory,
            b.WeeklyHoliday,
            ShowroomCode,
            WarehouseID,
            WeekNo,
            CMonth,
            CYear,
            TDate,
            TDayName
            FROM
            (
            --3 start
            SELECT DATEADD(dd, a.n-1, datefromparts({0},{1} ,1)) AS TDate,
            DATENAME(dw, DATEADD(dd, a.n-1, datefromparts({0},{1} ,1))) TDayName
            FROM
            (SELECT top 31 ROW_NUMBER() OVER (
            ORDER BY a.object_id) AS n
            FROM sys.all_objects a) a
            WHERE DATEPART(mm, DATEADD(dd, a.n-1, datefromparts({0},{1} ,1)))={1} 
            --3 end

            ) a,
            t_Showroom b,
            t_CalendarWeek c,
            v_CustomerDetails d
            WHERE a.TDate BETWEEN c.FromDate AND c.ToDate
            AND IsPOSActive=1
            AND b.CustomerID=d.CustomerID ) TotalDays
            LEFT OUTER JOIN
            (SELECT a.WarehouseID,
            a.WeekNo,
            a.CMonth,
            a.CYear,
            FromDate,
            ToDate,
            TargetValue/(Weekdays-isnull(b.HolidayCount, 0)) WeekDayTarget
            FROM
            (
            --2 start
            SELECT WarehouseID,
            ShowroomCode,
            b.WeekNo,
            CMonth,
            CYear,
            FromDate,
            ToDate,
            DATEDIFF(d, FromDate, ToDate)+1 AS Weekdays,
            sum(TargetValue) TargetValue
            FROM t_PlanMAGWeekTargetQty a,
            t_CalendarWeek b,
            t_Showroom c
            WHERE TYear={0}
            AND TMonth={1} 
            AND Channel in (13,
            4)
            AND a.WeekNo=b.WeekNo
            AND a.TYear=b.CYear
            AND a.TMonth=b.CMonth
            AND a.CustomerID=c.CustomerID
            GROUP BY WarehouseID,
            ShowroomCode,
            b.WeekNo,
            CMonth,
            CYear,
            FromDate,
            ToDate 
            --2 end
            ) a
            LEFT OUTER JOIN
            (SELECT b.WarehouseID,
            WeekNo,
            count(TDayName) HolidayCount
            FROM
            (
            --1 start
            SELECT DATEADD(dd, a.n-1, datefromparts({0},{1} ,1)) AS TDate,
            DATENAME(dw, DATEADD(dd, a.n-1, datefromparts({0},{1} ,1))) TDayName
            FROM
            (SELECT top 31 ROW_NUMBER() OVER (
            ORDER BY a.object_id) AS n
            FROM sys.all_objects a) a
            WHERE DATEPART(mm, DATEADD(dd, a.n-1, datefromparts({0},{1} ,1)))={1}  
            --1 end
            ) a,
            t_Showroom b,
            t_CalendarWeek c
            WHERE a.TDayName=b.WeeklyHoliday
            AND a.TDate BETWEEN c.FromDate AND c.ToDate
            GROUP BY b.WarehouseID,
            WeekNo,
            CMonth,
            CYear) b ON a.WarehouseID=b.WarehouseID
            AND a.WeekNo=b.WeekNo) WeekDayTarget ON TotalDays.WeekNo=WeekDayTarget.WeekNo
            AND TotalDays.CMonth= WeekDayTarget.CMonth
            AND TotalDays.CYear=WeekDayTarget.CYear --TotalDays.TDate between WeekDaySales.FromDate and WeekDaySales.ToDate
            AND TotalDays.WarehouseID=WeekDayTarget.WarehouseID
            LEFT OUTER JOIN
            (SELECT WarehouseID,
            InvoiceDate,
            Sum(NetSales) NetSales
            FROM dwdb.dbo.t_InvoiceWiseSalesDetail a
            WHERE Company='TEL'
            AND year(InvoiceDate)={0}
            AND month(InvoiceDate)={1} 
            GROUP BY WarehouseID,
            InvoiceDate) WeekSales ON TotalDays.TDate=WeekSales.InvoiceDate
            AND TotalDays.WarehouseID=WeekSales.WarehouseID
            WHERE TotalDays.ShowroomCode!='GOS'";
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month))
            {
                dailyQuery = string.Format(dailyQuery, "YEAR(GETDATE())", "MONTH(GETDATE())");
            }
            else
            {
                dailyQuery = string.Format(dailyQuery, year, month);
            }
            var dailySales = _db.Database.SqlQuery<DailySalesChartModel>(dailyQuery).ToList().OrderBy(c=>c.TDate);

            var datewisedsales = dailySales.GroupBy(c => c.TDate).Select(g => new DailySalesChartModel
            {
                TDate = g.Key,
                WeekDayTarget = _library.ConvertTOMillion(g.Sum(x => x.WeekDayTarget)),
                NetSales = _library.ConvertTOMillion(g.Sum(x => x.NetSales))
            }).ToList();
            var n1wisedsales = dailySales.Where(c => c.Area.Equals("National-1")).GroupBy(c => c.TDate).Select(g => new DailySalesChartModel
            {
                TDate = g.Key,
                WeekDayTarget = _library.ConvertTOMillion(g.Sum(x => x.WeekDayTarget)),
                NetSales = _library.ConvertTOMillion(g.Sum(x => x.NetSales))
            }).ToList();
            var n2wisedsales = dailySales.Where(c => c.Area.Equals("National-2")).GroupBy(c => c.TDate).Select(g => new DailySalesChartModel
            {
                TDate = g.Key,
                WeekDayTarget = _library.ConvertTOMillion(g.Sum(x => x.WeekDayTarget)),
                NetSales = _library.ConvertTOMillion(g.Sum(x => x.NetSales))
            }).ToList();
            var estorewisedsales = dailySales.Where(c => c.Area.Equals("e-Store")).GroupBy(c => c.TDate).Select(g => new DailySalesChartModel
            {
                TDate = g.Key,
                WeekDayTarget = _library.ConvertTOMillion(g.Sum(x => x.WeekDayTarget)),
                NetSales = _library.ConvertTOMillion(g.Sum(x => x.NetSales))
            }).ToList();

            ChartViewModel chartViewModel = new ChartViewModel();
            chartViewModel.PGwiseChartModel = pgWise;
            chartViewModel.MAGwiseChartModel = magWise;
            chartViewModel.AreawiseChartModel = areaWise;
            chartViewModel.CatwiseChartModel = catWise;
            chartViewModel.NationalwiseChartModel = natWise;
            chartViewModel.MonthwiseChartModel = monthWise;
            chartViewModel.AreawiseDalysales = datewisedsales;
            chartViewModel.N1wiseDalysales = n1wisedsales;
            chartViewModel.N2wiseDalysales = n2wisedsales;
            chartViewModel.EstorewiseDalysales = estorewisedsales;
            return _library.ConvertTOJson(chartViewModel);
        }
        public ActionResult loadStockChart(string brand, string pg, string pgcat, string month, string year)
        {
            var stockQuery = @"Select PGCategory, PGName, MAGName, 
            ROUND(Sum(CASE When StockType = 'Saleable' then (StockQty-DisplayStock) else 0 end),0) as 'Saleable', 
            ROUND(Sum(DisplayStock),0) as DisplayStock,
            ROUND(Sum(CASE When StockType != 'Saleable' then StockQty else 0 end),0) as 'Defective',
            ROUND(Sum(StockQty),0) as StockQty, 
            ROUND(Sum(StockValue),0) as StockValue From v_StockAging 
            Group by PGCategory, PGName, MAGName";
            var stockAging = _db.Database.SqlQuery<StockAgingChartModel>(stockQuery).ToList();
            var castwisestock = stockAging.GroupBy(c => c.PGCategory).Select(g => new StockAgingChartModel
            {
                PGCategory = g.Key,
                Saleable = g.Sum(x => x.Saleable),
                DisplayStock = g.Sum(x => x.DisplayStock),
                Defective = g.Sum(x => x.Defective),
                StockQty = g.Sum(x => x.StockQty),
                StockValue = Math.Round(g.Sum(x => x.StockValue) / 1000000, 2)
            }).ToList();
            var pgwisestock = stockAging.GroupBy(c => c.PGName).Select(g => new StockAgingChartModel
            {
                PGName = g.Key,
                Saleable = g.Sum(x => x.Saleable),
                DisplayStock = g.Sum(x => x.DisplayStock),
                Defective = g.Sum(x => x.Defective),
                StockQty = g.Sum(x => x.StockQty),
                StockValue = Math.Round(g.Sum(x => x.StockValue) / 1000000, 2)
            }).ToList();
            var magwisestock = stockAging.GroupBy(c => c.MAGName).Select(g => new StockAgingChartModel
            {
                MAGName = g.Key,
                Saleable = g.Sum(x => x.Saleable),
                DisplayStock = g.Sum(x => x.DisplayStock),
                Defective = g.Sum(x => x.Defective),
                StockQty = g.Sum(x => x.StockQty),
                StockValue = Math.Round(g.Sum(x => x.StockValue) / 1000000, 2)
            }).ToList();

            ChartViewModel chartViewModel = new ChartViewModel();
            chartViewModel.Castwisestock = castwisestock;
            chartViewModel.PGwisestock = pgwisestock;
            chartViewModel.MAGwisestock = magwisestock;
            return new Library().ConvertTOJson(chartViewModel);
        }
        public ActionResult loadCATChart()
        {
            var catQuery = @"select TMonth,AreaName,TerritoryName,ShowroomCode,PGCategory,PGName,
            ROUND(sum(TyearTargetQty),2) as TyearTargetQty,
            ROUND(sum(TyearTargetValue),2) as TyearTargetValue,
            ROUND(sum(TyearSalesQty),2) as TyearSalesQty,
            ROUND(sum(TyearNetsale),2) as TyearNetsale,
            ROUND(sum(LyearSalesQty),2) as LyearSalesQty,
            ROUND(sum(LyearNetsale),2) as LyearNetsale ,
            ROUND(sum(TyearNOI),2) as TyearNOI ,
            ROUND(sum(LyearNOI),2) as LyearNOI
            from(
            select Tyear,Tmonth,Warehouseid,PGCategory,PGName, sum(targetQty) as TyearTargetQty,sum(TargetValue) as TyearTargetValue,0 as TyearSalesQty,0 as TyearNetsale,
            0 as LyearSalesQty, 0 as LyearNetsale ,0 as TyearNOI , 0 as LyearNOI
            from t_PlanMAGWeekTargetQty a, t_showroom b ,(select pg.PGCategory,pg.PdtGroupName as PGName,mag.PdtGroupName as MAGName,mag.PdtGroupID as MAGID from t_ProductGroup mag, t_ProductGroup pg
            where mag.ParentID=pg.PdtGroupID and mag.PdtGroupType=2) c
            where a.customerid=b.customerid and a.magid=c.MAGID and tyear=YEAR(GETDATE()) and tmonth=Month(GETDATE())
            group by Tyear,Tmonth,Warehouseid,PGCategory,PGName
            union all
            select year(Invoicedate) as TYear, month(invoicedate) as TMonth,warehouseid,PGCategory,PGName,0 as TyearTargetQty,0 as TyearTargetValue,sum(SalesQty+freeQty) as TyearSalesQty,
            sum(NetSales) as TyearNetsale ,0 as LyearSalesQty, 0 as LyearNetsale,0 as TyearNOI , 0 as LyearNOI
            from DWDB.dbo.t_InvoiceWiseSalesDetail a, v_ProductDetails b where a.productid=b.productid and year(invoicedate)=YEAR(GETDATE()) and month(invoicedate)=Month(GETDATE())
            group by  year(Invoicedate), month(invoicedate),warehouseid,PGCategory,PGName
            union all
            select year(Invoicedate) as TYear, month(invoicedate) as TMonth,warehouseid,PGCategory,PGName,0 as TyearTargetQty,0 as TyearTargetValue,0 as TyearSalesQty,0 as TyearNetsale,
            sum(SalesQty+freeQty) as LyearSalesQty, sum(NetSales) as LyearNetsale,0 as TyearNOI , 0 as LyearNOI
            from DWDB.dbo.t_InvoiceWiseSalesDetail a, v_ProductDetails b where a.productid=b.productid and year(invoicedate)=YEAR(GETDATE())-1 and month(invoicedate)=Month(GETDATE())
            group by year(Invoicedate), month(invoicedate),warehouseid,PGCategory,PGName
            union all
            select TYear, TMonth,WarehouseID,PGCategory,PGName,0 as TyearTargetQty,0 as TyearTargetValue,0 as TyearSalesQty,0 as TyearNetsale,
            0 as LyearSalesQty, 0 as LyearNetsale,case tyear when YEAR(GETDATE()) then sum(case when invoicetypeid in (6,7,9,10,12) then -1 else 1 end) else 0 end as TyearNOI ,
            case tyear when YEAR(GETDATE())-1 then sum(case when invoicetypeid in (6,7,9,10,12) then -1 else 1 end) else 0 end as LyearNOI
            from(
            select distinct year(invoicedate) as TYear, month(invoicedate) as TMonth,warehouseid,PGCategory,PGName,InvoiceNo,InvoiceTypeID
            from DWDB.dbo.t_InvoiceWiseSalesDetail a, v_ProductDetails b where a.productid=b.productid and year(invoicedate) in(YEAR(GETDATE())-1,YEAR(GETDATE())) and month(invoicedate)=Month(GETDATE())) x
            group by TYear,TMonth,WarehouseID,PGCategory,PGName
            ) Main, t_showroom b, v_CustomerDetails c
            where main.WarehouseID=b.WarehouseID and b.CustomerID=c.CustomerID
            group by TMonth,AreaName,TerritoryName,ShowroomCode,PGCategory,PGName";
            var chart = _db.Database.SqlQuery<CATChartModel>(catQuery).ToList();
            var natWise = new CATChartModel
            {
                AreaName = "National",
                TyearNetsale = _library.ConvertTOMillion(chart.Sum(x => x.TyearNetsale)),
                TyearTargetValue = _library.ConvertTOMillion(chart.Sum(x => x.TyearTargetValue)),
                TyearSalesQty = chart.Sum(x => x.TyearSalesQty),
                TyearTargetQty = chart.Sum(x => x.TyearTargetQty),
                TyearNOI = chart.Sum(x => x.TyearNOI),
                LyearNetsale = _library.ConvertTOMillion(chart.Sum(x => x.LyearNetsale)),
                LyearNOI = chart.Sum(x => x.LyearNOI),
                LyearSalesQty = chart.Sum(x => x.LyearSalesQty),
            };
            var pgWise = _library.ConvertToChartModel(chart, "PGName");
            var areawise = _library.ConvertToChartModel(chart, "AreaName");
            var cat1 = _library.ConvertToChartModel(chart.Where(c => c.PGCategory.Equals("Category-1")).ToList(), "PGCategory");
            var cat2 = _library.ConvertToChartModel(chart.Where(c => c.PGCategory.Equals("Category-2")).ToList(), "PGCategory"); ;
            CATChartViewModel chartmodel = new CATChartViewModel();
            chartmodel.AreawiseChartModel = areawise;
            chartmodel.PGwiseChartModel = pgWise;
            chartmodel.NationalwiseChartModel = natWise;
            chartmodel.Cat1ChartModel = cat1;
            chartmodel.Cat2ChartModel = cat2;
            chartmodel.Data = chart;
            return _library.ConvertTOJson(chartmodel);
        }
        public ActionResult loadOPSChart()
        {
            var catQuery = @"select TMonth,AreaName,TerritoryName,ShowroomCode,
            Round(sum(TyearTargetQty),2) as TyearTargetQty,
            Round(sum(TyearTargetValue),2) as TyearTargetValue,
            Round(sum(TyearSalesQty),2) as TyearSalesQty,
            Round(sum(TyearNetsale),2) as TyearNetsale,
            Round(sum(LyearSalesQty),2) as LyearSalesQty,
            Round(sum(LyearNetsale),2) as LyearNetsale ,
            Round(sum(TyearNOI),2) as TyearNOI ,
            Round(sum(LyearNOI),2) as LyearNOI
            from(
            select Tyear,Tmonth,Warehouseid,sum(targetQty) as TyearTargetQty,sum(TargetValue) as TyearTargetValue,0 as TyearSalesQty,0 as TyearNetsale,
            0 as LyearSalesQty, 0 as LyearNetsale ,0 as TyearNOI , 0 as LyearNOI
            from t_PlanMAGWeekTargetQty a, t_showroom b
            where a.customerid=b.customerid and tyear=Year(GETDATE()) and tmonth=Month(GETDATE())
            group by Tyear,Tmonth,Warehouseid
            union all
            select year(Invoicedate) as TYear, month(invoicedate) as TMonth,warehouseid,0 as TyearTargetQty,0 as TyearTargetValue,sum(SalesQty+freeQty) as TyearSalesQty,
            sum(NetSales) as TyearNetsale ,0 as LyearSalesQty, 0 as LyearNetsale,0 as TyearNOI , 0 as LyearNOI
            from DWDB.dbo.t_InvoiceWiseSalesDetail where year(invoicedate)=Year(GETDATE()) and month(invoicedate)=Month(GETDATE())
            group by  year(Invoicedate), month(invoicedate),warehouseid
            union all
            select year(Invoicedate) as TYear, month(invoicedate) as TMonth,warehouseid,0 as TyearTargetQty,0 as TyearTargetValue,0 as TyearSalesQty,0 as TyearNetsale,
            sum(SalesQty+freeQty) as LyearSalesQty, sum(NetSales) as LyearNetsale,0 as TyearNOI , 0 as LyearNOI
            from DWDB.dbo.t_InvoiceWiseSalesDetail where year(invoicedate)=Year(GETDATE()) and month(invoicedate)=Month(GETDATE())
            group by year(Invoicedate), month(invoicedate),warehouseid
            union all
            select year(Invoicedate) as TYear, month(invoicedate) as TMonth,WarehouseID,0 as TyearTargetQty,0 as TyearTargetValue,0 as TyearSalesQty,0 as TyearNetsale,
            0 as LyearSalesQty, 0 as LyearNetsale,sum(case when invoicetypeid in (6,7,9,10,12) then -1 else 1 end) as TyearNOI , 0 as LyearNOI
            from t_salesinvoice where year(invoicedate)=Year(GETDATE()) and month(invoicedate)=Month(GETDATE())
            group by year(Invoicedate), month(invoicedate),WarehouseID
            union all
            select year(Invoicedate) as TYear, month(invoicedate) as TMonth,WarehouseID,0 as TyearTargetQty,0 as TyearTargetValue,0 as TyearSalesQty,0 as TyearNetsale,
            0 as LyearSalesQty, 0 as LyearNetsale,0 as TyearNOI , sum(case when invoicetypeid in (6,7,9,10,12) then -1 else 1 end) as LyearNOI
            from t_salesinvoice where year(invoicedate)=Year(GETDATE())-1 and month(invoicedate)=Month(GETDATE())
            group by year(Invoicedate), month(invoicedate),WarehouseID) Main, t_showroom b, v_CustomerDetails c
            where main.WarehouseID=b.WarehouseID and b.CustomerID=c.CustomerID
            group by TMonth,AreaName,TerritoryName,ShowroomCode
             ";
            var chart = _db.Database.SqlQuery<CATChartModel>(catQuery).ToList();
            var natWise = new CATChartModel
            {
                AreaName = "National",
                TyearNetsale = _library.ConvertTOMillion(chart.Sum(x => x.TyearNetsale)),
                TyearTargetValue = _library.ConvertTOMillion(chart.Sum(x => x.TyearTargetValue)),
                TyearSalesQty = chart.Sum(x => x.TyearSalesQty),
                TyearTargetQty = chart.Sum(x => x.TyearTargetQty),
                TyearNOI = chart.Sum(x => x.TyearNOI),
                LyearNetsale = _library.ConvertTOMillion(chart.Sum(x => x.LyearNetsale)),
                LyearNOI = chart.Sum(x => x.LyearNOI),
                LyearSalesQty = chart.Sum(x => x.LyearSalesQty),
            };
            var areawise = _library.ConvertToChartModel(chart, "AreaName");
            CATChartViewModel chartmodel = new CATChartViewModel();
            chartmodel.AreawiseChartModel = areawise;
            chartmodel.NationalwiseChartModel = natWise;
            chartmodel.Data = chart;
            return _library.ConvertTOJson(chartmodel);
        }
    }
}
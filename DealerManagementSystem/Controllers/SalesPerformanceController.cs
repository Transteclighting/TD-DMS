using DealerManagementSystem.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DealerManagementSystem.Controllers
{

    public class SalesPerformanceController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        // GET: SalesPerformance
        public ActionResult Index()
        {

            string sGetRegion = @"Select distinct RegionName  as Text,'""' as Id
            from t_MarketGroup
            where MarketGroupType = 1 and ChannelID = 3 and IsActive = 1";
            //string sGetRegion = @"Select MarketGroupID as Id,MarketGroupDesc as Text 
            //from t_MarketGroup 
            //where MarketGroupType =1 and ChannelID=3 and IsActive=1";
            var allRegion = _db.Database.SqlQuery<DropdownText>(sGetRegion).ToList();
            ViewBag.RegionList = new SelectList(allRegion, "Id", "Text");

            string sGetArea = @"Select MarketGroupID as Id,MarketGroupDesc as Text 
            from t_MarketGroup 
            where MarketGroupType =1 and ChannelID=3 and IsActive=1";
            var AllArea = _db.Database.SqlQuery<Dropdown>(sGetArea).ToList();
            ViewBag.AreaList = new SelectList(AllArea, "Id", "Text");

            string GetTerritory = @"Select MarketGroupID as Id ,MarketGroupDesc as Text
            from t_MarketGroup 
            where MarketGroupType =2 and ChannelID=3 and IsActive=1";

            var All = _db.Database.SqlQuery<Dropdown>(GetTerritory).ToList();
            ViewBag.TerritoryList = new SelectList(All, "Id", "Text");

            return View();
        }
        public ActionResult GetArea(string RegionName)
        {
            //ViewBag.AreaList = IEnumerable<Nullable>;
            string sGetArea = @"Select MarketGroupID as Id,MarketGroupDesc as Text 
            from t_MarketGroup 
            where MarketGroupType =1 and ChannelID=3 and IsActive=1
            and RegionName='" + RegionName + "'";
            var AllArea = _db.Database.SqlQuery<Dropdown>(sGetArea).ToList();
            ViewBag.AreaList = "";
            return Json(AllArea);
        }
        public ActionResult GetRegion()
        {
            //ViewBag.AreaList = IEnumerable<Nullable>;
            string sGet = @"Select distinct RegionName  as Id,RegionName as Text
            from t_MarketGroup
            where MarketGroupType = 1 and ChannelID = 3 and IsActive = 1";
            var AllRegion = _db.Database.SqlQuery<DropdownText>(sGet).ToList();
            return Json(AllRegion);
        }
        //[HttpGet]
        //public ActionResult GetRegion()
        //{
        //    //ViewBag.AreaList = IEnumerable<Nullable>;
        //    string sGet = @"Select distinct RegionName  as Id,RegionName as Text
        //    from t_MarketGroup
        //    where MarketGroupType = 1 and ChannelID = 3 and IsActive = 1";
        //    var AllRegion = _db.Database.SqlQuery<DropdownText>(sGet).ToList();
        //    return Json(AllRegion);
        //}
        public ActionResult GetTerritory(int ParentID)
        {
            string GetTerritory = @"Select MarketGroupID as Id ,MarketGroupDesc as Text
            from t_MarketGroup 
            where MarketGroupType =2 and ChannelID=3 and IsActive=1
            and ParentID=" + ParentID + "";
            var All = _db.Database.SqlQuery<Dropdown>(GetTerritory).ToList();
            return Json(All);
        }
        public ActionResult AttendanceClick(string Region, string Area, string Territory)
        {
            string sql = "";
            string _Region = "";
            if (Region != string.Empty && Area == "")
            {
                _Region = Convert.ToString(Region);
                // query += " AND MAGID= '" + pdtGroupId + "' ";
                sql = "   AND RegionName = " + "'" + _Region + "'";
            }
            else if (Area != "" && Territory == "")
            {
                sql += "  AND AreaID=" + Convert.ToInt32(Area);
            }
            else if (Territory != "")
            {
                sql += "  AND TerritoryID=" + Convert.ToInt32(Territory);
            }
            string MainSQL = @"
            declare @adate date
            set @adate='16-Nov-2022'
            Select DSRCode,DSRName,TerritoryName,Designation,
            convert(char(5), TDDBCheckIn, 108) as  TDAtten,
            TD_ISLATE=case  when  convert(time,TDDBCheckIn,108)<=convert(time, '08:30:59', 108) and  
            convert(time,TDDBCheckIn,108)>convert(time, '00:00:00', 108)   then 'Present' when TDDBCheckIn>convert(char(5), '08:30:59', 108)   
            then 'Late' else 'Absent' end , 
            convert(char(5), TDFirstOrder, 108) as TD_FV, 
            convert(char(5), TDLastOrder, 108) as TD_LV,
            isnull(convert(char(5), TDMarketTime, 108) ,0) as TDWH
            --convert(char(5), LDDBCheckIn, 108) as  LDAtten,
            --LD_ISLATE=case  when  convert(time,LDDBCheckIn,108)<=convert(time, '08:30:59', 108) and  convert(time,LDDBCheckIn,108)>convert(time, '00:00:00', 	108)   then 'Present' when LDDBCheckIn>convert(char(5), '08:30:59', 108)  then 'Late' else 'Absent' end , 
            --convert(char(5), LDFirstOrder, 108) as LD_FV, 
            --convert(char(5), LDLastOrder, 108) as LD_LV,
            --isnull(convert(char(5), LDMarketTime, 108),0) as LDWH
            from
            (
            select RegionName,AreaID,AreaName,TerritoryID,TerritoryName,
            Main.DSRID,DSRCode,DSRName, Designation,
            isnull(TDDBCheckIn,0)as TDDBCheckIn,  format (convert(date,@adate), '08:30:59') as Endtime ,TDOTLatitude,TDOTLongitude,
            isnull(TDFirstOrder,0)As TDFirstOrder, isnull(TDLastOrder,0)As TDLastOrder, TDMarketTime,
            isnull(LDDBCheckIn,0)as LDDBCheckIn, LDOTLatitude,LDOTLongitude,
            isnull(LDFirstOrder,0)As LDFirstOrder, isnull(LDLastOrder,0)As LDLastOrder, LDMarketTime
            from
            (
            Select a.EmployeeID as DSRID,EmployeeCode DSRCode,EmployeeName as DSRName,b.MarketGroupID as AreaID,b.MarketGroupDesc as AreaName,
            a.MarketGroupID as TerritoryID,a.MarketGroupDesc TerritoryName,
            b.RegionName,DesignationName as Designation From t_MarketGroup a
            join t_MarketGroup b on a.ParentID=b.MarketGroupID
            join v_EmployeeDetails c on a.EmployeeID=c.EmployeeID
            where EMPStatus in (1,2) and a.IsActive=1 and a.ChannelID=3
            ) As Main
            ------------------First and Last Order Time-----------------------
            left outer join
            (
            select  SAID, TDFirstOrder, TDLastOrder, CAST(TDLastOrder - TDFirstOrder as Time) as TDMarketTime
            from
            (
            select  SAID,min(CreateDate) as TDFirstOrder, max(CreateDate) as TDLastOrder
            from t_DMSOrder
            where CreateDate  between CAST(@adate AS Date )  and   CAST(dateadd(dd,+1, @adate) AS Date) and  
            CreateDate <   CAST(dateadd(dd,+1, @adate) AS Date)
            group by SAID
            ) as TT 
            ) as FOrTime on Main.DSRID=FOrTime.SAID 
            left outer join
            (
            ---------------------DB Point Check In------------------
            select distinct DSRID,Latitude as TDOTLatitude,Longitude as TDOTLongitude, min(Datetime) as TDDBCheckIn
            from t_UserRole a,  t_DMSDailyAttendance b
            where a.UserID=b.UserID and Datetime between CAST(@adate AS Date )  and   CAST(dateadd(dd,+1,@adate ) AS Date) and  
            Datetime <   CAST(dateadd(dd,+1,@adate ) AS Date) 
            and Position  in ('First beat Visit','DB Point') and RoleID=5 and DSRID is not null
            group by DSRID,  Latitude,Longitude
            ) as DBChk on Main.DSRID=DBChk.DSRID 
            ---------------------DB Point Check In End ------------------
            left outer join
            (
            select  SAID, LDFirstOrder, LDLastOrder, CAST(LDLastOrder - LDFirstOrder as Time) as LDMarketTime
            from
            (
            select  SAID,min(CreateDate) as LDFirstOrder, max(CreateDate) as LDLastOrder
            from t_DMSOrder
            where CreateDate  between   CAST(dateadd(dd,-1,@adate ) AS Date)  and   CAST(@adate AS Date ) and  
            CreateDate <  CAST(@adate AS Date )
            group by SAID
            ) as TT 
            ) as LDTime on Main.DSRID=LDTime.SAID 
            left outer join
            (
            ---------------------DB Point Check In------------------
            Select DSRID,min(Latitude) as LDOTLatitude,min(Longitude) as LDOTLongitude, 
            min(Datetime) as LDDBCheckIn From t_DMSDailyAttendance a
            join t_UserRole b on a.UserID=b.UserID
            where Datetime between CAST(dateadd(dd,-1,@adate) AS Date)  and   CAST( @adate AS Date ) and  
            Datetime <  CAST(@adate AS Date )
            and Position  in ('First beat Visit','DB Point') and RoleID=5
            group by b.DSRID
            ) as LDDBChk on Main.DSRID=LDDBChk.DSRID 
            ---------------------DB Point Check In End ------------------
            ) as final where DSRID>0";
            MainSQL += sql;
            var All = _db.Database.SqlQuery<Attendance>(MainSQL).ToList();
            return Json(All);
        }
        public ActionResult ValueClick(string Region, string Area, string Territory)
        {
            SalesPerformance_Value VL = new SalesPerformance_Value();
            string MainSQL = VL.Sql;
            string header = "";
            string footer = "";

            string Q1 = "";
            string Q2 = "";
            if (Region == "" && Area == "")
            {
                header = " Select 'GTM' as Name,";
                footer = "";
                Q1 = header + MainSQL + footer;

                header = "  Select RegionName As Name,";
                footer = "  Group By RegionName ";
                Q2 = header + MainSQL + footer;

                MainSQL = Q1 + " Union " + Q2;
            }

            else if (Region != string.Empty && Area == "")
            {
                header = " Select RegionName as Name,";
                footer = " Where RegionName='" + Convert.ToString(Region) + "'" + "Group By RegionName ";
                Q1 = header + MainSQL + footer;

                header = " Select AreaName As Name,";
                footer = " Where RegionName='" + Convert.ToString(Region) + "'" + "Group By AreaName ";
                Q2 = header + MainSQL + footer;

                MainSQL = Q1 + " Union " + Q2;
            }
            else if (Area != "" && Territory == "")
            {
                header = " Select AreaName As Name,";
                footer = " Where AreaID='" + Convert.ToInt32(Area) + "'" + "Group By AreaName ";
                Q1 = header + MainSQL + footer;

                header = "Select TerritoryName as Name,";
                footer = " Where AreaID='" + Convert.ToInt32(Area) + "'" + "Group By TerritoryName ";
                Q2 = header + MainSQL + footer;

                MainSQL = Q1 + " Union " + Q2;
            }
            else if (Area != "" && Territory != "")
            {
                header = "Select TerritoryName as Name,";
                footer = " Where TerritoryID='" + Convert.ToInt32(Territory) + "'" + "Group By TerritoryName ";
                Q1 = header + MainSQL + footer;

                header = "Select CustomerName as Name,";
                footer = " Where TerritoryID='" + Convert.ToInt32(Territory) + "'" + "Group By CustomerName ";
                Q2 = header + MainSQL + footer;

                MainSQL = "Select * from ( " + Q1 + " Union " + Q2 + ") As FF Order by CMSales DESC";
            }
            var All = _db.Database.SqlQuery<SalesPerformance_Value>(MainSQL).ToList();
            return Json(All);
        }
    }
}
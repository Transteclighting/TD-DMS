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
    public class AttendanceController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        // GET: TD Attendance 
        [PermishonFilter(Code = "M47.1.4")]
        public ActionResult Index()
        {

                ViewBag.areas = _db.Database.SqlQuery<Dropdown>(@"Select a.MarketGroupID as Id, MarketGroupDesc as Text 
                from t_MarketGroup a, DWDB.dbo.t_ActiveArea b Where a.MarketGroupID = b.MarketGroupId and 
                Channel = 'TD' order by a.Pos").ToList();
                DateTime from = DateTime.Now.Date;
                DateTime to = DateTime.Now.Date.AddDays(1);
                var sQuery = @"Select EmployeeCode,EmployeeName,DesignationName as Designation,DepartmentName as Department,
                CompanyCode,JobLocationName,b.SBUName as BU,AreaName,TerritoryName, c.ShowroomCode, CAST(Type as varchar) as Type,CAST(a.Latitude as varchar) as Latitude,CAST(a.Longitude as varchar) as Longitude,a.Address,
                CONVERT(VARCHAR,FORMAT(CheckTime,'dd-MMM-yyyy hh:mm tt')) as LastCheckTime 
                From 
                (
                Select a.* From t_DayTracker a,
                ( 
                Select max(TrackId) trackid,EmployeeId,max(CheckTime) LastCheckTime From t_DayTracker
                where CheckTime between '" + from + @"' and '" + to + @"' and CheckTime<'" + to + @"'
                group by EmployeeId
                ) b where a.TrackId=b.TrackId and a.EmployeeId=b.EmployeeId
                ) a,v_EmployeeDetails b,t_Showroom c,v_CustomerDetails d
                where a.EmployeeId=b.EmployeeID and EMPStatus in (1,2) and b.SBUID=2 
                and b.LocationID=c.LocationID and c.CustomerID=d.CustomerID";
                ViewBag.details = _db.Database.SqlQuery<Map>(sQuery).ToList();
                return View();
        }
        [PermishonFilter(Code = "M47.1.3")]
        public ActionResult List()
        {
                ViewBag.fromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.toDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.areas = _db.Database.SqlQuery<Dropdown>(@"Select a.MarketGroupID as Id, MarketGroupDesc as Text 
                from t_MarketGroup a, DWDB.dbo.t_ActiveArea b Where a.MarketGroupID = b.MarketGroupId and 
                Channel = 'TD' order by a.Pos").ToList();
                return View();
        }
        public ActionResult loaddata(string area,string zone, string outlet,
             string status, string distance, string op_distance, DateTime? fromDate, DateTime? toDate)
        {

            if (fromDate == null | toDate == null)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date.AddDays(1);
            }
            string actualFromDate = Convert.ToDateTime(fromDate).AddDays(-1).Date.ToString("dd-MMM-yyyy");
            string actualToDate = Convert.ToDateTime(toDate).Date.ToString("dd-MMM-yyyy");

            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate = Convert.ToDateTime(toDate).AddDays(1);
            string sQuery = @"Select ROW_NUMBER() over(order by Serial) as SL,* From
            (
            select cast(SiNo as varchar)+allemp.EmployeeCode as Serial, allemp.EmployeeID, allemp.EmployeeCode,allemp.EmployeeName,allemp.DesignationName,allemp.ShowroomCode,allemp.TerritoryName,allemp.AreaName,REPLACE(CONVERT(NVARCHAR,allemp.PunchDate, 106), ' ', '-') as PunchDate,InTime,OutTime,InAddress,
            OutAddress,InLatitude,InLongitude,OutLatitude,OutLongitude,allemp.OutletLatitude,allemp.OutletLongitude,Round(Distancemeters,0) as Distancemeters,WorkingTime,

            isnull(case when left(WorkingTimeInsideShowroom,1)='-' then ''
            when left(WorkingTimeInsideShowroom,1)='*' then ''
            when WorkingTimeInsideShowroom='0 Minutes ' then ''
            else WorkingTimeInsideShowroom end,'') as WorkingTimeInsideShowroom,

            isnull(case when left(WorkingTimeOutsideShowroom,1)='-' then '0 Minutes '
            when left(WorkingTimeOutsideShowroom,1)='*' then '0 Minutes '
            else WorkingTimeOutsideShowroom end,'0 Minutes ') as WorkingTimeOutsideShowroom,
            case when InTime is not null and
            DATEDIFF(MINUTE, (Select CONVERT(VARCHAR(5), LoginTime, 108) From t_HRShift where ShiftID=26),InTime)>0 then 'Late'
            when InTime is not null and
            DATEDIFF(MINUTE, (Select CONVERT(VARCHAR(5), LoginTime, 108) From t_HRShift where ShiftID=26),InTime)<=0 then 'Present'
            when allemp.AttendanceStatus is null and InTime is null then 'Absent'
            else allemp.AttendanceStatus end as AttendanceStatus,
            isnull(FloorLeadQty,0) as FloorLeadQty,isnull(FloorLeadAmount,0) as FloorLeadAmount,isnull(CollectedLeadQty,0) as CollectedLeadQty,
            isnull(CollectedLeadAmount,0) as CollectedLeadAmount,isnull(WEBLeadAmount,0) as WEBLeadAmount,isnull(Conversion,0) as Conversion
            from
            (
            select SiNo, m.EmployeeID, EmployeeCode,EmployeeName,DesignationName,
            ShowroomCode,TerritoryName,AreaName,dt as PunchDate, AttendanceStatus,
            Latitude as OutletLatitude,Longitude as OutletLongitude
            from
            (
            select * from
            (
            select EmployeeID,EmployeeCode,EmployeeName,DesignationName,a.LocationID,
            warehouseid,ShowroomCode,TerritoryName,AreaName ,Latitude, Longitude
            from v_EmployeeDetails a, t_showroom b, v_CustomerDetails c
            where a.LocationID=b.LocationID and b.CustomerID=c.CustomerID and DepartmentCode='TD'
            and EMPStatus in (1,2) and DesignationCode<>'DES0308') x ,
            (select top (datediff(dd,  '" + actualFromDate + @"', '" + actualToDate + @"')) ROW_NUMBER()
            over(order by a.name) as SiNo,
            Dateadd(dd, ROW_NUMBER() over(order by a.name) , '" + actualFromDate + @"') as Dt from sys.all_objects a) y) m
            left outer join
            (
            select EmployeeID,a.Warehouseid,a.Date,
            case when b.Status= 2 then 'Roster' when b.Status= 3 then 'Leave' end as AttendanceStatus
            from t_OutletAttendanceInfo a,t_OutletAttendanceInfoDetail b
            where a.ID=b.ID and a.WarehouseID=b.WarehouseID and b.status in (2,3)
            ) o
            on(m.EmployeeID=o.EmployeeID and m.WarehouseID=o.WarehouseID and
            cast(o.Date as date)=cast(m.Dt as date))
            ) allemp
            ----allemp info-----
            left outer join
            --attn info----
            (
            Select EmployeeID, EmployeeCode,EmployeeName,DesignationName,ShowroomCode,TerritoryName,AreaName,PunchDate,InTime,OutTime,InAddress,
            OutAddress,InLatitude,InLongitude,OutLatitude,OutLongitude,OutletLatitude,OutletLongitude,Round(Distancemeters,0) as Distancemeters,WorkingTime,
            dbo.MinutesToDuration(TotalWorkingMinutes-WorkingMinutesOutsideShowroom) as WorkingTimeInsideShowroom,
            WorkingTimeOutsideShowroom
            From
            (
            Select b.employeeid,b.EmployeeCode, b.EmployeeName, b.DesignationName,
            c.ShowroomCode, d.TerritoryName, d.AreaName, a.PunchDate,
            a.InTime AS InTime, a.OutTime AS OutTime,
            CASE WHEN a.InTime = '' THEN '' WHEN a.OutTime = '' THEN '' ELSE
            CAST(DATEPART(HOUR, CAST(a.OutTime AS datetime) - CAST(a.InTime AS datetime)) AS nvarchar(100)) +
            ' Hours ' + CAST(DATEPART(MINUTE, CAST(a.OutTime
            AS datetime) - CAST(a.InTime AS datetime)) AS nvarchar(100)) +
            ' Minutes ' END AS WorkingTime, a.InAddress AS InAddress,
            a.OutAddress AS OutAddress,
            OutLatitude,InLatitude,OutLongitude,InLongitude,
            d.Latitude as OutletLatitude,d.Longitude as OutletLongitude,
            case when InLatitude is not null and InLongitude is not null and d.Latitude is not null and d.Longitude is not null
            then Geography::Point(InLatitude, InLongitude, 4326).STDistance(Geography::Point(d.Latitude, d.Longitude, 4326)) else 0 end Distancemeters,
            CASE WHEN a.InTime = '' THEN '' WHEN a.OutTime = '' THEN 0 ELSE
            DATEDIFF(mi,a.InTime,a.OutTime) END AS TotalWorkingMinutes,
            isnull(outside.WorkingMinutesOutsideShowroom,0) WorkingMinutesOutsideShowroom,
            dbo.MinutesToDuration(isnull(outside.WorkingMinutesOutsideShowroom,0)) as WorkingTimeOutsideShowroom,
            CONVERT(VARCHAR(10), CAST(Outside.OutsideIntime AS time), 0) as OutsideTimeFrom,
            CONVERT(VARCHAR(10), CAST(Outside.OutsideOutTime AS time), 0) as OutsideTimeTo
            From
            (
            Select EmployeeId,PunchDate,max(InTime)InTime,max(OutTime) OutTime,
            max(InAddress) InAddress,max(OutAddress) OutAddress,
            max(InLatitude) InLatitude,max(OutLatitude) OutLatitude,
            max(InLongitude) InLongitude,max(OutLongitude) OutLongitude From
            (
            SELECT EmployeeId,
            REPLACE(CONVERT(VARCHAR(11), CheckTime, 106), ' ', '-') AS PunchDate,
            CASE WHEN Type = 1 THEN CONVERT(VARCHAR(10), CAST(CheckTime AS time), 0) ELSE '' END AS InTime,
            CASE WHEN Type = 2 THEN CONVERT(VARCHAR(10), CAST(CheckTime AS time), 0) ELSE '' END AS OutTime,
            CASE WHEN Type = 1 THEN Address ELSE '' END AS InAddress,
            CASE WHEN Type = 2 THEN Address ELSE '' END AS OutAddress,
            CASE WHEN Type = 1 THEN cast(Longitude as varchar) ELSE '' END AS InLongitude,
            CASE WHEN Type = 2 THEN cast(Longitude as varchar) ELSE '' END AS OutLongitude,
            CASE WHEN Type = 1 THEN cast(Latitude as varchar) ELSE '' END AS InLatitude,
            CASE WHEN Type = 2 THEN cast(Latitude as varchar) ELSE '' END AS OutLatitude
            FROM dbo.t_DayTracker
            WHERE (Type IN (1, 2))
            ) x group by EmployeeId,PunchDate
            ) a
            INNER JOIN
            dbo.v_EmployeeDetails AS b ON a.EmployeeId = b.EmployeeID
            INNER JOIN
            dbo.t_Showroom AS c ON b.LocationID = c.LocationID
            INNER JOIN
            dbo.v_CustomerDetails AS d ON c.CustomerID = d.CustomerID
            Left Outer Join
            (
            Select x.*,
            CASE WHEN OutsideInTime = '' THEN '' WHEN OutsideOutTime = '' THEN '' ELSE
            CAST(DATEPART(HOUR, CAST(OutsideOutTime AS datetime) - CAST(OutsideInTime AS datetime)) AS nvarchar(100)) +
            ' Hours ' + CAST(DATEPART(MINUTE, CAST(OutsideOutTime
            AS datetime) - CAST(OutsideInTime AS datetime)) AS nvarchar(100)) +
            ' Minutes ' END AS WorkingTimeOutsideShowroom,
            isnull(DATEDIFF(mi,OutsideInTime,OutsideOutTime),0) as WorkingMinutesOutsideShowroom
            from
            (
            Select EmployeeId,OutsideDate,max(OutsideInTime) OutsideIntime,
            max(OutsideOutTime) OutsideOutTime
            From
            (
            Select EmployeeId,cast (CheckTime as date) OutsideDate,
            case when a.Type=3 then min(CheckTime) else NULL end OutsideInTime,
            case when a.Type=4 then max(CheckTime) else NULL end OutsideOutTime
            From t_DayTracker a where Type in (3,4)
            group by EmployeeId,Type,cast (CheckTime as date)
            ) x group by EmployeeId,OutsideDate
            ) x
            ) Outside on a.EmployeeId=Outside.EmployeeId and
            cast(a.PunchDate as date)=Outside.OutsideDate
            ) Main
            where cast(PunchDate as date) between '" + fromDate + @"' and '" + toDate + @"'
            and cast(PunchDate as date)<'" + toDate + @"') attn
            on allemp.EmployeeID=attn.EmployeeID and allemp.showroomcode=attn.showroomcode and allemp.PunchDate=cast(attn.PunchDate as date)
            left outer join
            --Lead---
            (Select SalesPersonID,cast(LeadDate as date) LeadDate,
            sum(TotalLeadAmount)-sum(CollectedLeadAmount) as FloorLeadAmount,
            sum(TotalLeadQty)-sum(CollectedLeadQty) as FloorLeadQty,
            sum(CollectedLeadAmount) CollectedLeadAmount,
            sum(CollectedLeadQty) CollectedLeadQty,
            sum(Conversion) Conversion,sum(WEBLeadAmount) WEBLeadAmount
            From
            (
            ---FloorCollected---
            Select x.SalesPersonID,LeadDate,TotalLeadAmount,TotalLeadQty,
            CollectedLeadAmount,CollectedLeadQty,0 as Conversion,0 as WEBLeadAmount
            From
            (
            Select distinct a.SalesPersonID,a.LeadNo,
            a.WarehouseID,a.LeadDate,a.LeadAmount as TotalLeadAmount,
            1 as TotalLeadQty ,0 as CollectedLeadAmount,0 as CollectedLeadQty
            From t_SalesLeadManagement a
            where a.Status not in (4)
            Union All
            Select distinct a.EmployeeId,d.LeadNo,d.WarehouseID,d.LeadDate,
            0 as TotalLead,0 as TotalLeadQty,d.LeadAmount as CollectedLead,
            1 as CollectedLeadQty From t_DayPlan a
            join t_DayPlanDetails b on a.PlanId=b.PlanId
            join t_DayPlanWiseLead c on b.ID=c.PlanDetailId
            join t_SalesLeadManagement d on c.LeadId=d.LeadID and a.EmployeeId=d.SalesPersonID
            where d.Status not in (4)
            ) x,t_Employee y where x.SalesPersonID=y.EmployeeID
            ---END Floor Collected---
            Union All
            --Conversion---
            Select SalesPersonID,InvoiceDate,0 TotalLeadAmount,
            0 TotalLeadQty,0 CollectedLeadAmount,0 CollectedLeadQty,
            NetSales as Conversion,0 as WEBLeadAmount
            From
            (
            Select distinct b.InvoiceNo,b.ProductID,b.WarehouseID,
            b.SalesPersonID,b.InvoiceDate,b.NetSales
            From t_SalesLeadManagement a,dwdb.DBO.t_InvoiceWiseSalesDetail b
            where a.InvoiceNo=b.InvoiceNo
            ) Conversion
            --END Conversion---
            Union All
            ---WEBLead--
            Select SalesPersonID,OrderDate,0 as TotalLeadAmount,
            0 as TotalLeadQty,0 as CollectedLeadAmount,
            0 as CollectedLeadQty,0 as Conversion,Amount as WEBLeadAmount
            From t_SalesInvoiceEcom a,t_Employee b
            where a.SalesPersonID=b.EmployeeID
            ---WEBLead--
            ) main
            where cast(LeadDate as date) between '" + fromDate + @"' and '" + toDate + @"'
            and cast(LeadDate as date)<'" + toDate + @"'
            group by SalesPersonID,cast(LeadDate as date)) lead
            on(allemp.EmployeeID=lead.SalesPersonID and allemp.PunchDate=cast(lead.LeadDate as date))
            ) Main where 1=1 ";


            if (!String.IsNullOrWhiteSpace(area) |
                    !String.IsNullOrWhiteSpace(zone) |
                    !String.IsNullOrWhiteSpace(outlet)
                   )
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
                if (!String.IsNullOrWhiteSpace(outlet) && outlet != "All" && outlet != "null")
                {
                    sQuery = sQuery + " and ShowroomCode='" + outlet + "'";
                }

                if (!String.IsNullOrWhiteSpace(distance) && distance != "All" && distance != "null")
                {
                    sQuery = sQuery + " and Distancemeters"+ op_distance + distance;
                }
                if (!String.IsNullOrWhiteSpace(status) && status != "All" && status != "null")
                {
                    sQuery = sQuery + " and AttendanceStatus ='" + status + "'";
                }
            }
            var details = _db.TDAttendanceDetail.SqlQuery(sQuery).ToList();
            return Json(new { data = details }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadMap(string area, string zone, string outlet)
        {
            if (zone != null)
            {
                DateTime from = DateTime.Now.Date;
                DateTime to = DateTime.Now.Date.AddDays(1);
                var sQuery = @"Select EmployeeCode,EmployeeName,DesignationName as Designation,DepartmentName as Department,
                CompanyCode,JobLocationName,b.SBUName as BU,AreaName,TerritoryName, c.ShowroomCode, CAST(Type as varchar) as Type,CAST(a.Latitude as varchar) as Latitude,CAST(a.Longitude as varchar) as Longitude,a.Address,
                CONVERT(VARCHAR,FORMAT(CheckTime,'dd-MMM-yyyy hh:mm tt')) as LastCheckTime 
                From 
                (
                Select a.* From t_DayTracker a,
                ( 
                Select max(TrackId) trackid,EmployeeId,max(CheckTime) LastCheckTime From t_DayTracker
                where CheckTime between '" + from + @"' and '" + to + @"' and CheckTime<'" + to + @"'
                group by EmployeeId
                ) b where a.TrackId=b.TrackId and a.EmployeeId=b.EmployeeId
                ) a,v_EmployeeDetails b,t_Showroom c,v_CustomerDetails d
                where a.EmployeeId=b.EmployeeID and EMPStatus in (1,2) and b.SBUID=2 
                and b.LocationID=c.LocationID and c.CustomerID=d.CustomerID";
                if (!String.IsNullOrWhiteSpace(area) |
                    !String.IsNullOrWhiteSpace(zone) |
                    !String.IsNullOrWhiteSpace(outlet))
                {
                    if (!String.IsNullOrWhiteSpace(area) && area != "All")
                    {
                        sQuery = sQuery + " and AreaName='" + area + "'";
                    }

                    if (!String.IsNullOrWhiteSpace(zone) && zone != "All")
                    {
                        sQuery = sQuery + " and TerritoryName='" + zone + "'";
                    }

                    if (!String.IsNullOrWhiteSpace(outlet) && outlet != "All")
                    {
                        sQuery = sQuery + " and c.ShowroomCode='" + outlet + "'";
                    }
                }
                var details = _db.Database.SqlQuery<Map>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult loadDetailsData(string empId, DateTime? date)
        {

            string sQuery = @"select TrackId,CONVERT(NVARCHAR,CheckTime,100) as CheckTime,EmployeeId,InstrumentId,Format( Latitude ,'N','en-US' ) as Latitude,Format( Longitude ,'N','en-US' ) as Longitude,Remarks,Address,MergeAttendanceData,
            case Type when 1 then 'CheckIn' when 2 then 'CheckOut' when 3 then 'DayPlan CheckIn' 
            when 4 then 'DayPlan CheckOut' when 5 then 'FreeCheckIn' when 6 then 'CreateLead' end as Type 
            from t_DayTracker Where Type NOT IN (5) and CheckTime between
            '11-Oct-2021' and '12-Oct-2021' and CheckTime < '12-Oct-2021'
            and EmployeeId = 85305";
            var details = _db.Database.SqlQuery<DayTracker>(sQuery).ToList();
            return Json(new { data = details }, JsonRequestBehavior.AllowGet);
        }

        // GET: Distribution Attendance 
        [PermishonFilter(Code = "M47.2.1")]
        public ActionResult DistributionList()
        {
                ViewBag.fromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.toDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.region = _db.Database.SqlQuery<DropdownText>(@"Select distinct RegionName as Text,RegionName as Id From t_MarketGroup where ChannelID in
                (Select ChannelID from t_Channel where ChannelID=3)
                and RegionName is not null order By RegionName").ToList();
                return View();
        }
        [PermishonFilter(Code = "M47.2.3")]
        public ActionResult DistributionMap()
        {
                ViewBag.region = _db.Database.SqlQuery<DropdownText>(@"Select distinct RegionName as Text,RegionName as Id From t_MarketGroup where ChannelID in
                (Select ChannelID from t_Channel where ChannelID=3)
                and RegionName is not null order By RegionName").ToList();
                DateTime from = DateTime.Now.Date;
                DateTime to = DateTime.Now.Date.AddDays(1);
                var sQuery = @"Select EmployeeCode,EmployeeName,DesignationName as Designation,DepartmentName as Department,
                CompanyCode,JobLocationName,b.SBUName as BU,
                CAST(Type as varchar) as Type,CAST(a.Latitude as varchar) as Latitude,
                CAST(a.Longitude as varchar) as Longitude,a.Address,
                CONVERT(VARCHAR,FORMAT(CheckTime,'dd-MMM-yyyy hh:mm tt')) as LastCheckTime
                From
                (
                Select a.* From t_DayTracker a,
                (
                Select max(TrackId) trackid,EmployeeId,max(CheckTime) LastCheckTime From t_DayTracker
                where CheckTime between '" + from + @"' and '" + to + @"' and CheckTime<'" + to + @"'
                group by EmployeeId
                ) b where a.TrackId=b.TrackId and a.EmployeeId=b.EmployeeId
                ) a,v_EmployeeDetails b
                where a.EmployeeId=b.EmployeeID
                and EMPStatus in (1,2) and b.SBUID=5";
                ViewBag.details = _db.Database.SqlQuery<Map>(sQuery).ToList();
                return View();
        }
        public ActionResult loadDistributionData(string region, string area,
            string zone, string cust,
            DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate == null | toDate == null)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date.AddDays(1);
            }
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate = Convert.ToDateTime(toDate).AddDays(1);
            string sQuery = @"Select REPLACE(CONVERT(NVARCHAR,a.PunchDate, 106), ' ', '-') as PunchDate,EmployeeCode,EmployeeName,
            DesignationName,DepartmentName,CompanyCode,InTime,OutTime,CAST(WorkingTime as varchar) as WorkingTime,
            CAST(InAddress as varchar) as InAddress,CAST(OutAddress as varchar) as OutAddress,
            CAST(OutLatitude as varchar) as OutLatitude,CAST(InLatitude as varchar) as InLatitude,
            CAST(OutLongitude as varchar) as OutLongitude,CAST(InLongitude as varchar) as InLongitude,
            CAST(TotalWorkingMinutes as varchar) as TotalWorkingMinutes,CAST(CustLatitude as varchar) as CustLatitude,CAST(CustLongitude as varchar) as CustLatitude,
            ROUND(case when InLatitude is not null and InLongitude is not null and d.CustLatitude is not null and d.CustLongitude is not null
            then Geography::Point(InLatitude, InLongitude, 4326).STDistance(Geography::Point(d.CustLatitude, d.CustLongitude, 4326)) else 0 end,2) Distancemeters,
            isnull(NoOfPlan,0) NoOfPlan,isnull(NoOfVisit,0) NoOfVisit,isnull(NoOfInvoice,0) NoOfInvoice,
            isnull(NetSales,0) NetSales,isnull(SalesQty,0) SalesQty
            From
            (
            SELECT TOP (DATEDIFF(DAY,  '" + fromDate + @"',  '" + toDate.Value.AddDays(-1) + @"') + 1)
            PunchDate = DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1,  '" + fromDate + @"')
            FROM sys.all_objects a CROSS JOIN sys.all_objects b
            ) a
            join v_EmployeeDetails b on 1=1--Attendance--
            left outer join
            (
            Select a.EmployeeID,
            a.PunchDate,a.InTime AS InTime, a.OutTime AS OutTime,
            CASE WHEN a.InTime = '' THEN '' WHEN a.OutTime = '' THEN '' ELSE
            CAST(DATEPART(HOUR, CAST(a.OutTime AS datetime) - CAST(a.InTime AS datetime)) AS nvarchar(100)) +
            ' Hours ' + CAST(DATEPART(MINUTE, CAST(a.OutTime
            AS datetime) - CAST(a.InTime AS datetime)) AS nvarchar(100)) +
            ' Minutes ' END AS WorkingTime, a.InAddress AS InAddress,
            a.OutAddress AS OutAddress,
            OutLatitude,InLatitude,OutLongitude,InLongitude,
            CASE WHEN a.InTime = '' THEN '' WHEN a.OutTime = '' THEN 0 ELSE
            DATEDIFF(mi,a.InTime,a.OutTime) END AS TotalWorkingMinutes
            From
            (
            Select EmployeeId,PunchDate,max(InTime)InTime,max(OutTime) OutTime,
            max(InAddress) InAddress,max(OutAddress) OutAddress,
            max(InLatitude) InLatitude,max(OutLatitude) OutLatitude,
            max(InLongitude) InLongitude,max(OutLongitude) OutLongitude From
            (
            SELECT EmployeeId,
            REPLACE(CONVERT(VARCHAR(11), CheckTime, 106), ' ', '-') AS PunchDate,
            CASE WHEN Type = 1 THEN CONVERT(VARCHAR(10), CAST(CheckTime AS time), 0) ELSE '' END AS InTime,
            CASE WHEN Type = 2 THEN CONVERT(VARCHAR(10), CAST(CheckTime AS time), 0) ELSE '' END AS OutTime,
            CASE WHEN Type = 1 THEN Address ELSE '' END AS InAddress,
            CASE WHEN Type = 2 THEN Address ELSE '' END AS OutAddress,
            CASE WHEN Type = 1 THEN cast(Longitude as varchar) ELSE '' END AS InLongitude,
            CASE WHEN Type = 2 THEN cast(Longitude as varchar) ELSE '' END AS OutLongitude,
            CASE WHEN Type = 1 THEN cast(Latitude as varchar) ELSE '' END AS InLatitude,
            CASE WHEN Type = 2 THEN cast(Latitude as varchar) ELSE '' END AS OutLatitude
            FROM dbo.t_DayTracker
            WHERE (Type IN (1, 2))
            ) x group by EmployeeId,PunchDate
            ) a
            ) c on cast(a.PunchDate as date)=cast(c.PunchDate as date) and b.EmployeeID=c.EmployeeId
            ---end Attendance---
            ---First Checkin Customer LatLong from Plan--
            left outer join
            (
            Select x.EmployeeId,min(x.Customerid) CustomerID,x.PlanDate,min(z.Latitude) as CustLatitude,min(z.Longitude) as CustLongitude
            from
            (
            Select b.ID,a.EmployeeId,Customerid,PlanDate,
            min(CONVERT(varchar,TimeFrom,108)) FirstCheckIntime
            From t_DayPlan a
            join t_DayPlanDetails b on a.PlanId=b.PlanId
            where CustomerId<>0
            group by b.ID,a.EmployeeId,Customerid,PlanDate
            ) x,
            (
            Select a.EmployeeId,PlanDate,
            min(CONVERT(varchar,TimeFrom,108)) FirstCheckIntime
            From t_DayPlan a
            join t_DayPlanDetails b on a.PlanId=b.PlanId
            where CustomerId<>0
            group by a.EmployeeId,PlanDate
            ) y,
            t_Customer z
            where x.CustomerId=z.CustomerID and x.EmployeeId=y.EmployeeId and x.PlanDate=y.PlanDate and x.FirstCheckIntime=y.FirstCheckIntime
            group by x.EmployeeId,x.PlanDate
            ) d on cast(a.PunchDate as date)=cast(d.PlanDate as date) and b.EmployeeID=d.EmployeeId
            ---END First Checkin Customer LatLong from Plan--
            left outer join
            (select EmployeeID,PlanDate, SUM(NoOfPlan) as NoOfPlan,
            SUM(NoOfVisit) as NoOfVisit,SUM(NoOfInvoice) as NoOfInvoice,SUM(round(NetSales,0)) as NetSales,
            SUM(SalesQty) as SalesQty from V_DistributionPlanDetails
            WHERE PlanDate between  '" + fromDate + @"' and  '" + toDate + @"' and PlanDate< '" + toDate + @"'
            group by EmployeeID,PlanDate
            ) e on b.EmployeeID=e.EmployeeID and cast(a.PunchDate as date)=cast(e.PlanDate as date)
            where b.SBUID=5 and b.EMPStatus in (1,2) and LocationID<>145
            ";

            if (!String.IsNullOrWhiteSpace(region) | !String.IsNullOrWhiteSpace(area) |
                    !String.IsNullOrWhiteSpace(zone) |
                    !String.IsNullOrWhiteSpace(cust))
            {

                if (!String.IsNullOrWhiteSpace(zone))
                {
                    sQuery = sQuery + @" and b.EmployeeID in (Select distinct EmployeeID From 
                                    (
                                    Select EmployeeID From t_MarketGroup where EmployeeID is not null and MarketGroupID = "+ zone + @"
                                    ) Territory)";
                }
                else if (!String.IsNullOrWhiteSpace(area))
                {
                    sQuery = sQuery + @" and b.EmployeeID in (Select distinct EmployeeID From 
                                    (
                                    Select EmployeeID From t_MarketGroup where MarketGroupID = "+ area + @"
                                    and EmployeeID is not null
                                    Union All
                                    Select b.EmployeeID From t_MarketGroup a, t_MarketGroup b
                                    where a.RegionName is not null and a.MarketGroupID = b.ParentID
                                    and a.MarketGroupID =  " + area + @" and b.EmployeeID is not null
                                    ) Area)";
                }

                else if (!String.IsNullOrWhiteSpace(region))
                {
                    sQuery = sQuery + @" and b.EmployeeID in (Select distinct EmployeeID From 
                                    (
                                    Select RSM as EmployeeID From t_MarketGroup where RegionName='" + region + @"' 
                                    and RSM is not null
                                    Union All
                                    Select a.EmployeeID From t_MarketGroup a,t_MarketGroup b 
                                    where a.RegionName is not null and a.MarketGroupID=b.ParentID
                                    and a.RegionName='" + region + @"' and a.EmployeeID is not null
                                    Union All
                                    Select b.EmployeeID From t_MarketGroup a,t_MarketGroup b 
                                    where a.RegionName is not null and a.MarketGroupID=b.ParentID
                                    and a.RegionName='" + region + @"' and b.EmployeeID is not null
                                    ) RSM) ";
                }
                
                
                
            }
            var details = _db.Database.SqlQuery<DistributionAttendance>(sQuery).ToList();
            return Json(new { data = details }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadDistributionMap(string region, string area,
            string zone, string cust)
        {

                DateTime from = DateTime.Now.Date;
                DateTime to = DateTime.Now.Date.AddDays(1);
                var sQuery = @"Select EmployeeCode,EmployeeName,DesignationName as Designation,DepartmentName as Department,
                CompanyCode,JobLocationName,b.SBUName as BU,
                CAST(Type as varchar) as Type,CAST(a.Latitude as varchar) as Latitude,
                CAST(a.Longitude as varchar) as Longitude,a.Address,
                CONVERT(VARCHAR,FORMAT(CheckTime,'dd-MMM-yyyy hh:mm tt')) as LastCheckTime
                From
                (
                Select a.* From t_DayTracker a,
                (
                Select max(TrackId) trackid,EmployeeId,max(CheckTime) LastCheckTime From t_DayTracker
                where CheckTime between '" + from + @"' and '" + to + @"' and CheckTime<'" + to + @"'
                group by EmployeeId
                ) b where a.TrackId=b.TrackId and a.EmployeeId=b.EmployeeId
                ) a,v_EmployeeDetails b
                where a.EmployeeId=b.EmployeeID
                and EMPStatus in (1,2) and b.SBUID=5";
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
                var details = _db.Database.SqlQuery<Map>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
        }
    }
}
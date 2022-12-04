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
    public class CommonController : Controller
    {
        private readonly ProjectDb _db = new ProjectDb();
        // GET: UI Items

        [ChildActionOnly]
        //[RemoveAuthenticationFilter]
        public ActionResult RenderMenu()
        {
            return PartialView("_MenuBar");
        }
        public ActionResult PageNotFound()
        {
            return View();
        }
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UnderConstruction()
        {
            return View();
        }

        public ActionResult loadZones(int? area)
        {
            if (area != null)
            {
                var sQuery = @"Select a.MarketGroupID as Id, MarketGroupDesc as Text, a.Pos 
                from t_MarketGroup a, t_Showroom b, t_Customer c where a.MarketGroupID = c.MarketGroupID
                and c.CustomerID = b.CustomerID and b.IsTDOutlet = 1 and IsPOSActive = 1 and a.ParentID = " + area + @"
                group by a.MarketGroupID, MarketGroupDesc, a.Pos order by a.Pos";
                var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult loadOutlet(int? zone)
        {
            if (zone != null)
            {
                var sQuery = @"Select ShowroomCode as Id, (ShowroomCode+' - '+ShowroomName) as Text from t_Showroom a, t_Customer b where a.IsPOSActive = 1 and a.IsActive = 1 and a.IsTDOutlet = 1
                and a.CustomerID = b.CustomerID and b.MarketGroupID = " + zone + " order by ShowroomCode";
                var details = _db.Database.SqlQuery<DropdownText>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult loadWH()
        {
            var sQuery = @"select WarehouseID, WarehouseCode, WarehouseName, ShortCode from t_Warehouse where IsActive = 1";
            var details = _db.Database.SqlQuery<DropdownText>(sQuery).ToList();
            //new EnvironmentVariableTarget
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        // GET: Visit
        public ActionResult loadVisitArea(string region)
        {
            if (region != null)
            {
                var sQuery = @"Select MarketGroupID as Id,MarketGroupDesc as Text From t_MarketGroup where ChannelID in
                (Select ChannelID from t_Channel where ChannelID=3)
                and MarketGroupType=1 and RegionName='" + region + @"'
                order By Pos";
                var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult loadVisitZones(string area)
        {
            if (!string.IsNullOrWhiteSpace(area))
            {
                var sQuery = "Select * From t_MarketGroup Where MarketGroupType=2 and ParentID=" + area;
                var details = _db.MarketGroup.SqlQuery(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadVisitCust(string zone)
        {
            if (!string.IsNullOrWhiteSpace(zone))
            {
                var sQuery = "Select CustomerID as Id, (CustomerName+' ('+CustomerCode+ ')') as Text From t_Customer Where MarketGroupID =" + zone;
                var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        // GET: Date
        public ActionResult loadYear()
        {
                var sQuery = @"select distinct CYear as Id,convert(varchar, CYear) as Text  from t_CalendarWeek where cyear>=year(getdate())-1";
                var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadMonth(string year)
        {
                var sQuery = "select distinct  CMonth as Id,convert(varchar, CMonth) as Text  from t_CalendarWeek where cyear=" + year;
                var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadweek(string month, string year)
        {
                var sQuery = "select WeekNo as Id,convert(varchar, WeekNo) as Text from t_CalendarWeek where cyear="+ year + " and CMonth=" + month;
                var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
        }

        // GET: Product Group
        public ActionResult loapgbycat(string cat)
        {
            var sQuery = @"select PdtGroupID as Id,PdtGroupName as Text 
            from t_ProductGroup where PdtGroupType=1 and PGCategory<>'Service Charges'
            and PGCategory='"+ cat +@"' order by POS";
            var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }

        public ActionResult loadpg()
        {
            var sQuery = @"select PdtGroupID as Id,PdtGroupName as Text  from t_ProductGroup where IsActive=1 and PdtGroupType=1";
            var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadmag(string pg)
        {
            var sQuery = "select PdtGroupID as Id,PdtGroupName as Text  from t_ProductGroup where IsActive=1 and PdtGroupType=2 and ParentID=" + pg;
            var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadasg(string mag)
        {
            var sQuery = "select PdtGroupID as Id,PdtGroupName as Text  from t_ProductGroup where IsActive=1 and PdtGroupType=3 and ParentID=" + mag;
            var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult loadag(string asg)
        {
            var sQuery = "select PdtGroupID as Id,PdtGroupName as Text  from t_ProductGroup where IsActive=1 and PdtGroupType=4 and ParentID=" + asg;
            var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        // GET: Product Brand
        public ActionResult loadbrand()
        {
            var sQuery = "select BrandID as Id, BrandDesc as Text from t_Brand where IsActive=1";
            var details = _db.Database.SqlQuery<Dropdown>(sQuery).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
    }
}
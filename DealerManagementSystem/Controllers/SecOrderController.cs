using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web.Mvc;
using DealerManagementSystem.Models;
using System.Data.Entity.SqlServer;
using System.Transactions;
using System.Web.UI.WebControls;
using DealerManagementSystem.Models.ViewModel;
using DealerManagementSystem.Models.Helper;

namespace DealerManagementSystem.Controllers
{
    public class SecOrderController : Controller
    {
        private readonly ProjectDb _db;

        public SecOrderController()
        {
            _db = new ProjectDb();
        }
        public ActionResult SecOrder()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                var nCustomerId = (int)Session["CustomerID"];

                if (Session["EmpId"] != null)
                {
                    var nEmpId = (int)Session["EmpId"];
                    var sCustomers = _db.Database.SqlQuery<Dropdown>(@"SELECT CustomerId as Id,CustomerName+' ('+CustomerCode+')' as Text
                    FROM t_MarketGroup a,t_Customer b Where
                    a.MarketGroupID = b.MarketGroupID
                    AND a.EmployeeId =" + nEmpId + @" and b.IsActive=1
                    Order BY CustomerName ASC").ToList();
                    ViewBag.CustomersList = sCustomers;
                }


                var Orders = _db.Database.SqlQuery<SecOrderViewModel>(@"
                Select TranID,a.BeatID,RouteName,RetailID,RetailName,a.NetAmount,Convert(varchar,cast(a.CreateDate as Date)) as TranDate
                from t_DMSOrder a, t_DMSRoute c, t_DMSClusterOutlet d
                where  a.BeatID=c.RouteID and a.outletID=d.RetailID 
                and TranID not in (Select TranID from t_DMSOrder where IsDelivered=1) and NetAmount>0 and CreateDate between CAST(dateadd(dd,-45, GETDATE()) AS Date)
                and CAST(dateadd(dd,+1, GETDATE()) AS Date) and CreateDate <CAST(dateadd(dd,+1, GETDATE()) AS Date) and a.DistributorID=" + nCustomerId + "").ToList();
                ViewBag.OrderList = Orders;
                return View(Orders);
            }
            return RedirectToAction("Login", "User");

        }

        public ActionResult EditOrder(int TranID)
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                if (Session["EmpId"] != null)
                {
                    var nEmpId = (int)Session["EmpId"];
                    var sCustomers = _db.Database.SqlQuery<Dropdown>(@"SELECT CustomerId as Id,CustomerName+' ('+CustomerCode+')' as Text
                                FROM t_MarketGroup a,t_Customer b Where
                                a.MarketGroupID = b.MarketGroupID
                                AND a.EmployeeId =" + nEmpId + @" and b.IsActive=1
                                Order BY CustomerName ASC").ToList();
                    ViewBag.CustomersList = sCustomers;
                }
                var nCustomerId = (int)Session["CustomerID"];

                string sGetMag = @"Select distinct MAGID as PdtGroupId, MAGName as PdtGroupName,
                                  MAGSort as GroupSort from dbo.t_DMSProductStock a, v_ProductDetails b
                                  Where a.ProductId = b.ProductId and DistributorID=" + nCustomerId + "Order by MAGSort";
                var allMag = _db.ProductGroups.SqlQuery(sGetMag).ToList();

                const string getBrand = @"Select *
                                    from t_Brand a
                                    Where IsActive =1  
                                    and BrandLevel =1 ";
                var allBrand = _db.Brands.SqlQuery(getBrand).ToList();
                //OrderCreateViewModel orderModel = new OrderCreateViewModel();
                ViewBag.MAGList = new SelectList(allMag, "PdtGroupId", "PdtGroupName");
                ViewBag.BradList = new SelectList(allBrand, "BrandId", "BrandDesc");
                SecOrder dmsOrder = (from order in _db.SecOrder
                                     where
                                     order.TranID == TranID
                                     select order).FirstOrDefault();

                if (dmsOrder != null)
                {
                    var orderitems = (from product in _db.OrderItem
                                      where product.TranID == TranID
                                      select product).ToList();
                    List<OrderItemViewModel> Products = new List<OrderItemViewModel>();
                    foreach (var product in orderitems)
                    {
                        var iitem = GetProduct(product.ProductID);
                        iitem.Qty = product.Qty;
                        Products.Add(iitem);
                    }
                    ViewBag.ProductsList = Products;
                }

                var Stock = _db.Database.SqlQuery<SerialandStock>(@"
                SELECT  ToCustomerID as CustomerID,ProductID,ProductSerial
                from t_DMSProductStockSerial a
                INNER JOIN  t_DMSProductStockTran b
                ON a.TranID = b.TranID
                Where a.Status = 1 and ToCustomerID=" + nCustomerId + "").ToList();
                ViewBag.StockList = Stock;

                if (TempData["error"] != null)
                {
                    ViewBag.error = TempData["error"].ToString();
                    TempData["error"] = null;
                }
                //return View(dmsOrder);
                return View("~/Views/SecOrder/AddEditOrder.cshtml", dmsOrder);

            }
            return RedirectToAction("Login", "User");
        }

        public OrderItemViewModel GetProduct(int productId)
        {
            string query = "SELECT * FROM t_Product WHERE ProductId=" + productId;
            var products = _db.Database.SqlQuery<OrderItemViewModel>(query).FirstOrDefault();
            return products;
        }

        public JsonResult GetOrders(string sStartDate, string sEndDate, int? custId)
        {
            var nCustomerId = (int)Session["CustomerID"];
            if (Session["UserType"].ToString() == "Manager")
            {
                if (custId != null)
                {
                    nCustomerId = (int)custId;
                }
            }
            var dtStartDate = DateTime.Parse(sStartDate).Date;
            var dtEndDate = DateTime.Parse(sEndDate).Date;

            var Orders = _db.Database.SqlQuery<SecOrderViewModel>(@"
                Select TranID,a.BeatID,RouteName,RetailID,RetailName,a.NetAmount,Convert(varchar,cast(a.CreateDate as Date)) as TranDate
                from t_DMSOrder a, t_DMSRoute c, t_DMSClusterOutlet d
                where  a.BeatID=c.RouteID and a.outletID=d.RetailID 
                and TranID not in (Select TranID from t_DMSOrder where IsDelivered=1) and NetAmount>0 and CreateDate >= '" + dtStartDate + @"'
                and CreateDate<= '" + dtEndDate + "'  and a.DistributorID=" + nCustomerId + "").ToList();
            return Json(Orders, JsonRequestBehavior.AllowGet);
        }
    }
}


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
    public class DmsOrderController : Controller
    {
        private readonly ProjectDb _db;

        public DmsOrderController()
        {
            _db = new ProjectDb();
        }
        public ActionResult DmsOrder()
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
                var dmsOrder = (from order in _db.DmsSecondarySalesOrder
                                where System.Data.Entity.DbFunctions.TruncateTime(order.CreateDate) == DateTime.Today
                                && order.CustomerId == nCustomerId && order.CustomerId != 0
                                select order)
                                .ToList();
                return View(dmsOrder);
            }
            return RedirectToAction("Login", "User");

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

            var orders = (from a in _db.DmsSecondarySalesOrder
                          where a.CreateDate >= dtStartDate && a.CreateDate <= dtEndDate && a.CustomerId == nCustomerId
                          orderby a.CreateDate descending
                          select new
                          {
                              a.OrderId,
                              a.OrderNo,
                              OrderCreateDay = SqlFunctions.DateName("dd", a.CreateDate),
                              OrderCreateMonth = SqlFunctions.DateName("mm", a.CreateDate),
                              OrderCreateCreateYear = SqlFunctions.DateName("yyyy", a.CreateDate),
                              InvoiceDay = SqlFunctions.DateName("dd", a.Edd),
                              InvoiceMonth = SqlFunctions.DateName("mm", a.Edd),
                              InvoiceYear = SqlFunctions.DateName("yyyy", a.Edd),
                              Status = a.Status,
                              a.CustomerId,
                              a.OrderAmount
                          }).ToList();
            return Json(orders, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult MakeOrder()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
            {
                //var nCustomerId = (int)Session["CustomerID"];
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
                string sGetMag = "Select PdtGroupId, PdtGroupName, pos as GroupSort From t_ProductGroup where IsActive=1 and PdtGroupType=2 Order by Pos";
                var allMag = _db.ProductGroups.SqlQuery(sGetMag).ToList();
                const string getBrand = @"Select *
                                    from t_Brand a
                                    Where IsActive =1  
                                    and BrandLevel =1  Order by BrandPos";
                var allBrand = _db.Brands.SqlQuery(getBrand).ToList();
                ViewBag.MAGList = new SelectList(allMag, "PdtGroupId", "PdtGroupName");
                ViewBag.BradList = new SelectList(allBrand, "BrandId", "BrandDesc");
                ViewBag.ProductsList = "";
                if (TempData["error"] != null)
                {
                    ViewBag.error = TempData["error"].ToString();
                    TempData["error"] = null;
                }
                //return View();
                return View("~/Views/DMSOrder/AddEditOrder.cshtml", new DmsSecondarySalesOrder());
            }
            return RedirectToAction("Login", "User");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeOrder(OrderCreateViewModel order)
        {
            if (Session["LoggedUserName"] == null && Session["LogedUserPassword"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var multiProducts = order.OrderProducts.Select(c => c.ProductId).GroupBy(p => p).Any(c => c.Count() > 1);
            if (multiProducts)
            {
                TempData["error"] = "Same product can not be added multiple time!";
                return RedirectToAction("MakeOrder", "DMSOrder");
            }
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    var customerId = (int)Session["CustomerId"];
                    if (Session["UserType"].ToString() == "Manager")
                    {
                        if (order.CustomerId != null)
                        {
                            customerId = (int)order.CustomerId;
                        }
                    }
                    var parentId = (int)Session["ParentCustomerId"];
                    var showroom = _db.Showroom.First(a => a.CustomerId == parentId);
                    var orderId = _db.DmsSecondarySalesOrder.DefaultIfEmpty().Max(r => r == null ? 1 : r.OrderId + 1);
                    var prodIds = order.OrderProducts.Select(a => a.ProductId).ToList();
                    var productDetails = _db.ProductDetails.Where(a => prodIds.Contains(a.ProductId)).ToList();
                    string q = @"Select isnull(MappedPOSWH,0) WHID From t_Customer a,t_CustomerType b,t_Channel c
                                where a.CusttypeID=b.CusttypeID and b.ChannelID=c.ChannelID
                                and CustomerID=" + customerId;
                    int wId = _db.Database.SqlQuery<int>(q).First();
                    var anOrder = new DmsSecondarySalesOrder
                    {
                        WarehouseId = order.IsOrderToTaggedShowroom ? showroom.WarehouseId : wId,
                        CreateUserId = 1,
                        CreateDate = DateTime.Now,
                        CustomerId = customerId,
                        Edd = DateTime.Now,
                        OrderAmount = productDetails.Sum(a => a.Rsp),
                        OrderId = orderId,
                        OrderNo = "Order-" + showroom.ShowroomCode + "-" + DateTime.Today.ToString("yy") + orderId,
                        ParentCustomerId = parentId,
                        Remarks = order.Remarks,
                        Status = 0,
                        SalesType = 5
                    };
                    _db.DmsSecondarySalesOrder.Add(anOrder);
                    DmsSecondarySalesOrderDetail aDmsOrderDetails;
                    order.OrderProducts.ForEach(a =>
                    {
                        aDmsOrderDetails = new DmsSecondarySalesOrderDetail
                        {
                            Qty = a.OrderQty,
                            OrderId = orderId,
                            UnitPrice = productDetails.First(b => b.ProductId == a.ProductId).Rsp,
                            ProductId = a.ProductId,
                            WarehouseId = showroom.WarehouseId
                        };
                        _db.DmsSecondarySalesOrderDetail.Add(aDmsOrderDetails);

                    });

                    _db.SaveChanges();
                    scope.Complete();

                    return RedirectToAction("DMSOrder");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        public ActionResult EditOrder(int orderId)
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
                DmsSecondarySalesOrder dmsOrder = (from order in _db.DmsSecondarySalesOrder
                                                   where order.CustomerId == nCustomerId && order.OrderId == orderId
                                                   select order).FirstOrDefault();

                if (dmsOrder != null)
                {
                    var dmsOrderProducts = (from product in _db.DmsSecondarySalesOrderDetail
                                            where product.OrderId == orderId
                                            select product).ToList();
                    List<OrderProductViewModel> products = new List<OrderProductViewModel>();
                    foreach (var product in dmsOrderProducts)
                    {
                        var item = GetProduct(product.ProductId);
                        item.OrderQty = product.Qty;
                        products.Add(item);
                    }
                    ViewBag.ProductsList = products;
                }
                if (TempData["error"] != null)
                {
                    ViewBag.error = TempData["error"].ToString();
                    TempData["error"] = null;
                }
                //return View(dmsOrder);
                return View("~/Views/DMSOrder/AddEditOrder.cshtml", dmsOrder);

            }
            return RedirectToAction("Login", "User");
        }
        public OrderProductViewModel GetProduct(int productId)
        {
            string query = "SELECT * FROM t_Product WHERE ProductId=" + productId;
            var products = _db.Database.SqlQuery<OrderProductViewModel>(query).FirstOrDefault();
            return products;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrder(OrderEditViewModel order)
        {
            if (Session["LoggedUserName"] == null && Session["LogedUserPassword"] == null)
            {
                return View(order);
            }
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
                {
                    var multiProducts = order.OrderProducts.Select(c => c.ProductId).GroupBy(p => p).Any(c => c.Count() > 1);
                    if (multiProducts)
                    {
                        TempData["error"] = "Same product can not be added multiple time!";
                        return RedirectToAction("EditOrder", "DMSOrder", new { orderId = order.OrderId });
                    }
                    var customerId = (int)Session["CustomerId"];
                    if (Session["UserType"].ToString() == "Manager")
                    {
                        if (order.CustomerId != null)
                        {
                            customerId = (int)order.CustomerId;
                        }
                    }
                    var parentId = (int)Session["ParentCustomerId"];
                    var showroom = _db.Showroom.First(a => a.CustomerId == parentId);
                    var orderId = order.OrderId;
                    var prodIds = order.OrderProducts.Select(a => a.ProductId).ToList();
                    var productDetails = _db.ProductDetails.Where(a => prodIds.Contains(a.ProductId)).ToList();
                    string q = @"Select isnull(MappedPOSWH,0) WHID From t_Customer a,t_CustomerType b,t_Channel c
                                where a.CusttypeID=b.CusttypeID and b.ChannelID=c.ChannelID
                                and CustomerID=" + customerId;
                    int wId = _db.Database.SqlQuery<int>(q).First();
                    _db.DmsSecondarySalesOrderDetail.RemoveRange(_db.DmsSecondarySalesOrderDetail.Where(x => x.OrderId == order.OrderId));
                    _db.SaveChanges();

                    DmsSecondarySalesOrder anOrder = _db.DmsSecondarySalesOrder.Where(c => c.OrderId == order.OrderId).FirstOrDefault();
                    anOrder.OrderAmount = productDetails.Sum(a => a.Rsp);
                    anOrder.Remarks = order.Remarks;
                    order.OrderProducts.ForEach(a =>
                    {
                        DmsSecondarySalesOrderDetail aDmsOrderDetails = new DmsSecondarySalesOrderDetail
                        {
                            Qty = a.OrderQty,
                            OrderId = orderId,
                            UnitPrice = productDetails.First(b => b.ProductId == a.ProductId).Rsp,
                            ProductId = a.ProductId,
                            WarehouseId = showroom.WarehouseId
                        };
                        _db.DmsSecondarySalesOrderDetail.Add(aDmsOrderDetails);
                    });

                    _db.SaveChanges();
                    scope.Complete();
                    ViewBag.success = "Order successfully updated!  Order Id = " + orderId;
                    return RedirectToAction("DMSOrder");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [NonAction]
        private DmsSecondarySalesOrder InsertOrderAndGetOrderId(DateTime edd, DmsSecondarySalesOrder aDmsSalesOrder, InvoiceViewModel aInvoiceViewModel)
        {
            int nCustomerId = (int)Session["CustomerId"];
            aDmsSalesOrder.OrderId =
            _db.DmsSecondarySalesOrder.DefaultIfEmpty().Max(r => r == null ? 1 : r.OrderId + 1);

            int numberOfOrder = _db.DmsSecondarySalesOrder.Count
                                (m => m.CustomerId == nCustomerId
                                && m.CreateDate.Month == DateTime.Now.Month
                                && m.CreateDate.Year == DateTime.Now.Year) + 1;


            aDmsSalesOrder.OrderNo = DateTime.Now.ToString("yy") + DateTime.Now.ToString("MM") +
                                     numberOfOrder.ToString("000");
            aDmsSalesOrder.CreateDate = DateTime.Today;
            aDmsSalesOrder.CustomerId = (int)Session["CustomerId"];
            aDmsSalesOrder.CreateUserId = (int)Session["UserId"];
            aDmsSalesOrder.CreateDate = DateTime.Now;
            aDmsSalesOrder.OrderAmount = aInvoiceViewModel.DmsSecondarySalesOrder.OrderAmount;
            aDmsSalesOrder.SalesType = 5;

            var dealercontacts = from c in _db.Customers
                                 join s in _db.Showroom on c.ParentCustomerId equals s.CustomerId
                                 where (c.CustomerId == (int)Session["CustomerId"])
                                 select s.WarehouseId;

            aDmsSalesOrder.Status = 5;

            _db.DmsSecondarySalesOrder.Add(aDmsSalesOrder);

            return aDmsSalesOrder;
        }
    }
}
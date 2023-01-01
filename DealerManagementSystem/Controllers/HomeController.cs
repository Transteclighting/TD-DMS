using System.Web.Mvc;
using DealerManagementSystem.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Globalization;

namespace DealerManagementSystem.Controllers
{

    public class HomeController : Controller
    {
        private ProjectDb db = new ProjectDb();
        // GET: /Home/
        public ActionResult Index()
        {
            if (Session["CustomerID"] != null)
            {
                int nCustomerId = (int)Session["CustomerID"];
                if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null)
                {
                    if (TempData["ChangePassMsg"] != null)
                    {
                        ViewBag.ChangePassStatus = (int)TempData["ChangePassStatus"];
                        ViewBag.ChangePassMsg = TempData["ChangePassMsg"].ToString();
                    }
                    GetOperationDate();
                    ViewBag.OperationDate = TempData["OperationDate"].ToString();
                    ViewBag.NextOperationDate = TempData["NextOperationDate"].ToString();

                    if (TempData["CheckOperationDate"] != null)
                    {
                        ViewBag.CheckOperationDate = TempData["CheckOperationDate"].ToString();
                    }
                    return View();
                }
            }
            return RedirectToAction("Login", "User");

        }
        public ActionResult Dashboard()
        {
            if (Session["LoggedUserName"] != null && Session["LogedUserPassword"] != null && Session["UserType"].ToString() == "Admin")
            {
                if (TempData["ChangePassMsg"] != null)
                {
                    ViewBag.ChangePassStatus = (int)TempData["ChangePassStatus"];
                    ViewBag.ChangePassMsg = TempData["ChangePassMsg"].ToString();
                }

                else
                {
                    //**********Display CHart ***********
                    Calculatetion();
                    Region_MTD();
                    Region_YTD();
                    Total_TGT_VS_SALES_MTD();
                    Total_TGT_VS_SALES_YTD();
                    StrikeRate_Region();
                    Order_Delivery_MTD();
                }
                return View();
            }

            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public void Calculatetion()
        {
            var sCustomers = db.Database.SqlQuery<CalculatedData>(@"
            Select 
            Convert(float,isnull(TDTarget,0)) as TDTarget,
            convert(float,isnull(TDOrder,0)) as TDOrder,
            convert(float,isnull(LDTarget,0)) as LDTarget,
            convert(float,isnull(LDOrder,0)) as LDOrder,
            convert(float,isnull(LD_1Order,0)) as LDOrder,
            convert(float,isnull(LDDelivery,0)) as LDDelivery,
            convert(float,isnull(Case When Bounce<0 then 0 else Bounce End,0)) as Bounce,
            convert(float,Case When BouncePer <0 then 0 else BouncePer END ) as BouncePer
            from
            (
            Select Sum(TDTarget) as TDTarget,
            sum(TDOrder) as TDOrder,
            sum(LDTarget) as LDTarget,
            sum(LDOrder) as LDOrder,
            sum(LD_1Order) as LD_1Order,
            sum(LDDelivery) as LDDelivery,
            (Sum(isnull(LD_1Order,0))-sum(isnull(LDDelivery,0))) as Bounce,
            isnull(round ( ( (Sum(LD_1Order)-sum(LDDelivery))/ nullif(cast ( ((Sum(LD_1Order))) as float)*100,0)),2),0) as BouncePer
            from
            (
            -----------------TD  Target Start-----
            select round(sum(TSOTargetTO),0)  as TDTarget, 0 as TDOrder,0 as LDTarget,0 as LDOrder,0 as LD_1Order,0 as LDDelivery
            from t_DMSDailyTargetTO
            where TGTDate between   CAST( GETDATE() AS Date )  and  CAST(dateadd(dd,+1, GETDATE()) AS Date) 
            and   TGTDate <    CAST(dateadd(dd,+1, GETDATE()) AS Date)
            -----------------TD  Target End-----
            Union ALL
            -----------------Today Order-----
            select 0 as TDTarget, round(sum(NetAmount),0) as TDOrder,0 as LDTarget,0 as LDOrder,0 as LD_1Order,0 as LDDelivery
            from t_DMSorder
            where TranDate between CAST( GETDATE() AS Date )  and   CAST(dateadd(dd,+1, GETDATE()) AS Date) 
            and   TranDate <   CAST(dateadd(dd,+1, GETDATE()) AS Date)
            -----------------Today Order End -----
            Union ALL
            -----------------LD  Target Start-----
            select 0 as TDTarget,0 as TDOrder, round(sum(TSOTargetTO),0)  as LDTarget, 0 as LDOrder,0 as LD_1Order,0 as LDDelivery
            from t_DMSDailyTargetTO
            where TGTDate between    CAST(dateadd(dd,-2, GETDATE()) AS Date)  and  CAST(dateadd(dd,-1, GETDATE()) AS Date)   
            and   TGTDate <    CAST(dateadd(dd,-1, GETDATE()) AS Date)   
            -----------------LD  Target End-----
            Union ALL
            -----------------LD Order-----
            select 0 as TDTarget,0 as TDOrder, 0 as LDTarget, round(sum(NetAmount),0) as LDOrder,0 as LD_1Order,0 as LDDelivery
            from t_DMSorder
            where TranDate between    CAST(dateadd(dd,-1, GETDATE()) AS Date) and  CAST(GETDATE() AS Date)
            and   TranDate <  CAST(GETDATE() AS Date)
            -----------------LD Order End -----
            union ALL
            -----------------LD-1  Order-----
            select  0 as TDTarget,0 as TDOrder, 0 as LDTarget,0 as LDOrder,sum(NetAmount) as LD_1Order,0 as LDDelivery
            from t_DMSorder
            where TranDate between   dateadd(day,-2, cast(getdate() as date))  and   dateadd(day,-1, cast(getdate() as date)) and   
            TranDate <   dateadd(day,-1, cast(getdate() as date))
            group by BeatID
            -----------------LD-1 Order End -----
            union all
            -----------------Yesterday Delivery start-----
            select   0 as TDTarget,0 as TDOrder, 0 as LDTarget,0 as LDOrder,0 as LD_1Order,sum(DeliveryAmount) as LDDelivery
            from t_DMSorder
            where DeliveryDate between  dateadd(day,-1, cast(getdate() as date))  
            and   CAST( GETDATE() AS Date ) and   DeliveryDate <    CAST( GETDATE() AS Date )
            and IsDelivered=1
            group by BeatID		
            -----------------Yesterday Delivery End-----
            ) as Main 
            ) as QQ").ToList();
            foreach (var items in sCustomers)
            {
                ViewBag.TDTarget = items.TDTarget;
                ViewBag.TDOrder = items.TDOrder;
                ViewBag.LDTarget = items.LDTarget;
                ViewBag.LDOrder = items.LDOrder;
                ViewBag.LD_minus1_Order = items.LD_minus1_Order;
                ViewBag.LDDelivery = items.LDDelivery;
                ViewBag.Bounce = items.Bounce;
                ViewBag.BouncePer = items.BouncePer;
            }
            // ViewBag.CustomersList = sCustomers;
        }
        [HttpPost]
        public void Region_MTD()
        {
            //************PrimarySales
            var Primary = db.Database.SqlQuery<ChartView.MTD_Pri>(@"
            Select Main.RegionName,
            isnull(round(sum(PriSales),0),0) as PriSales ,
            isnull(sum(Secsales),0) as SecSales from 
            (
            Select Distinct RegionName from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            ) as Main Left join 
            (
            Select RegionName,SUM(Pri_Sales) as PriSales,Sum(Sec_Sales) as SecSales
            from(
            Select RegionName,round(SUM(NetSales),0) as Pri_Sales,0 as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and InvoiceDate< DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)  and IsPrimarySales=1
            group by RegionName
            union ALL
            Select RegionName,0 as Pri_Sales,round(SUM(NetSales),0) as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and InvoiceDate< DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)  and IsPrimarySales!=1
            group by RegionName
            ) as Union_Data Group by RegionName
            ) as Sales on Main.RegionName=Sales.RegionName
            where PriSales>0
            Group by Main.RegionName").ToList();

            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var item in Primary)
            {
                dataPoints.Add(new DataPoint(item.RegionName, item.PriSales));
            }
            ViewBag.MTD_PriSales = JsonConvert.SerializeObject(dataPoints);


            //Sec sales
            var SecCondary = db.Database.SqlQuery<ChartView.MTD_Sec>(@"  
            Select Main.RegionName,isnull(round(sum(Secsales),0),0) as SecSales from 
            (
            Select Distinct RegionName from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            ) as Main Left join 
            (
            Select RegionName,SUM(Pri_Sales) as PriSales,Sum(Sec_Sales) as SecSales
            from(
            Select RegionName,round(SUM(NetSales),0) as Pri_Sales,0 as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and InvoiceDate< DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)  and IsPrimarySales=1
            group by RegionName
            union ALL
            Select RegionName,0 as Pri_Sales,round(SUM(NetSales),0) as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and InvoiceDate< DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)  and IsPrimarySales!=1
            group by RegionName
            ) as Union_Data Group by RegionName
            ) as Sales on Main.RegionName=Sales.RegionName
            where SecSales>0
            Group by Main.RegionName").ToList();

            List<DataPoint> dataPoints1 = new List<DataPoint>();
            foreach (var item in SecCondary)
            {
                dataPoints1.Add(new DataPoint(item.RegionName, item.SecSales));
            }
            ViewBag.MTD_SecSales = JsonConvert.SerializeObject(dataPoints1);



            // Primary Target
            var PriTGT = db.Database.SqlQuery<ChartView.MTD_PriTGT>(@"  
            Select RegionName,round(sum(nullif(TargetValue,0)),0) PriTGT
            From t_PlanDealerMAGTarget a
            join v_CustomerDetails b on a.CustomerID=b.CustomerID 
            where TYear=YEAR(Getdate()) and TMonth=Month(GetDate())
            and  RegionName !=''
            group by RegionName").ToList();

            List<DataPoint> PriTGT_DataPoints = new List<DataPoint>();
            foreach (var item in PriTGT)
            {
                PriTGT_DataPoints.Add(new DataPoint(item.RegionName, item.PriTGT));
            }
            ViewBag.MTD_PriTarget = JsonConvert.SerializeObject(PriTGT_DataPoints);


            //Sec Target
            var SecTGT = db.Database.SqlQuery<ChartView.MTD_SecTGT>(@"  
            Select RegionName,convert(float,0) as SecTGT  
            from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where RegionName not in ('N1','N2') 
            Group by RegionName").ToList();

            List<DataPoint> SecTGT_DataPoints = new List<DataPoint>();
            foreach (var item in SecTGT)
            {
                SecTGT_DataPoints.Add(new DataPoint(item.RegionName, item.SecTGT));
            }
            ViewBag.MTD_SecTarget = JsonConvert.SerializeObject(SecTGT_DataPoints);


        }
        public void Region_YTD()
        {
            //************PrimarySales
            var Primary = db.Database.SqlQuery<ChartView.MTD_Pri>(@"
            Select Main.RegionName,
            isnull(round(sum(PriSales),0),0) as PriSales ,
            isnull(sum(Secsales),0) as SecSales from 
            (
            Select Distinct RegionName from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            ) as Main Left join 
            (
            Select RegionName,SUM(Pri_Sales) as PriSales,Sum(Sec_Sales) as SecSales
            from(
            Select RegionName,round(SUM(NetSales),0) as Pri_Sales,0 as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between DATEADD(year, DATEDIFF(year, 0, GETDATE()), 0) and DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0)
            and InvoiceDate< DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0)  and IsPrimarySales=1
            group by RegionName
            union ALL
            Select RegionName,0 as Pri_Sales,round(SUM(NetSales),0) as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between DATEADD(year, DATEDIFF(year, 0, GETDATE()), 0) and DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0)
            and InvoiceDate< DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0) and IsPrimarySales!=1
            group by RegionName
            ) as Union_Data Group by RegionName
            ) as Sales on Main.RegionName=Sales.RegionName
			where PriSales>0
            Group by Main.RegionName").ToList();

            List<DataPoint> dataPoints = new List<DataPoint>();
            foreach (var item in Primary)
            {
                dataPoints.Add(new DataPoint(item.RegionName, item.PriSales));
            }
            ViewBag.YTD_PriSales = JsonConvert.SerializeObject(dataPoints);


            //Sec sales
            var SecCondary = db.Database.SqlQuery<ChartView.MTD_Sec>(@"  
            Select Main.RegionName,
            isnull(round(sum(Secsales),0),0) as SecSales from 
            (
            Select Distinct RegionName from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            ) as Main Left join 
            (
            Select RegionName,SUM(Pri_Sales) as PriSales,Sum(Sec_Sales) as SecSales
            from(
            Select RegionName,round(SUM(NetSales),0) as Pri_Sales,0 as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between DATEADD(year, DATEDIFF(year, 0, GETDATE()), 0) and DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0)
            and InvoiceDate< DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0) and IsPrimarySales=1
            group by RegionName
            union ALL
            Select RegionName,0 as Pri_Sales,round(SUM(NetSales),0) as Sec_Sales
            from  [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between DATEADD(year, DATEDIFF(year, 0, GETDATE()), 0) and DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0)
            and InvoiceDate< DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0) and IsPrimarySales!=1
            group by RegionName
            ) as Union_Data Group by RegionName
            ) as Sales on Main.RegionName=Sales.RegionName
			where PriSales>0
            Group by Main.RegionName").ToList();

            List<DataPoint> dataPoints1 = new List<DataPoint>();
            foreach (var item in SecCondary)
            {
                dataPoints1.Add(new DataPoint(item.RegionName, item.SecSales));
            }
            ViewBag.YTD_SecSales = JsonConvert.SerializeObject(dataPoints1);



            // Primary Target YTD
            var PriTGT = db.Database.SqlQuery<ChartView.MTD_PriTGT>(@"  
            Select RegionName,round(sum(nullif(TargetValue,0)),0) PriTGT
            From t_PlanDealerMAGTarget a
            join v_CustomerDetails b on a.CustomerID=b.CustomerID
            where TYear=YEAR(Getdate()) and RegionName !=''
            group by RegionName").ToList();

            List<DataPoint> PriTGT_DataPoints = new List<DataPoint>();
            foreach (var item in PriTGT)
            {
                PriTGT_DataPoints.Add(new DataPoint(item.RegionName, item.PriTGT));
            }
            ViewBag.YTD_PriTarget = JsonConvert.SerializeObject(PriTGT_DataPoints);


            //Sec Target
            var SecTGT = db.Database.SqlQuery<ChartView.MTD_SecTGT>(@"  
            Select RegionName,convert(float,0) as SecTGT  
            from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail 
            where RegionName not in ('N1','N2') 
            Group by RegionName").ToList();

            List<DataPoint> SecTGT_DataPoints = new List<DataPoint>();
            foreach (var item in SecTGT)
            {
                SecTGT_DataPoints.Add(new DataPoint(item.RegionName, item.SecTGT));
            }
            ViewBag.YTD_SecTarget = JsonConvert.SerializeObject(SecTGT_DataPoints);


        }
        public void Total_TGT_VS_SALES_MTD()
        {

            //***************National Target
            var NTGT = db.Database.SqlQuery<ChartView.National_TGT>(@"       
            Select 'Primary' as Nationals,
            isnull(round(sum(nullif(TargetValue,0)),0),0) Target
            From t_PlanDealerMAGTarget a
            join v_CustomerDetails b on a.CustomerID=b.CustomerID 
            where TYear=YEAR(Getdate()) and TMonth=Month(GetDate())
            and RegionName !=''
            union
            Select 'Secondary' as Nationals,isnull(round(sum(nullif(TSOTargetTO,0)),0),0) Target
            From t_DMSTargetTO a
            join v_CustomerDetails b on a.DistributorID=b.CustomerID
            where TGTDate=DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and RegionName !=''").ToList();

            List<DataPoint> N_Target = new List<DataPoint>();
            foreach (var item in NTGT)
            {
                N_Target.Add(new DataPoint(item.Nationals, item.Target));
            }
            ViewBag.N_Target = JsonConvert.SerializeObject(N_Target);


            ///************* National Sales

            var NS = db.Database.SqlQuery<ChartView.National_Sales>(@"
            Select * from 
            (
            Select 'Primary' as Nationals ,
            isnull(round(SUM(isnull(NetSales,0)),0),0) as Sales
            from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail 
            where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0)
            and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and InvoiceDate<DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and  isprimarysales=1
            union ALL
            Select 'Secondary' as Nationals ,isnull(round(SUM(isnull(NetSales,0)),0),0) as Sales
            from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail 
            where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0)
            and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and InvoiceDate<DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and  isprimarysales=0
            ) as Main").ToList();

            List<DataPoint> N_Sales = new List<DataPoint>();
            foreach (var item in NS)
            {
                N_Sales.Add(new DataPoint(item.Nationals, item.Sales));
            }
            ViewBag.N_Sales = JsonConvert.SerializeObject(N_Sales);
        }
        public void Total_TGT_VS_SALES_YTD()
        {

            //***************National Target
            var NTGT_YTD = db.Database.SqlQuery<ChartView.National_TGT_YTD>(@"           
            Select 'Primary' as Nationals,
            isnull(round(sum(nullif(TargetValue,0)),0),0) Target
            From t_PlanDealerMAGTarget a
            join v_CustomerDetails b on a.CustomerID=b.CustomerID 
            where TYear=YEAR(Getdate())
            and RegionName !=''
            union
            Select 'Secondary' as Nationals,isnull(round(sum(nullif(TSOTargetTO,0)),0),0) Target
            From t_DMSTargetTO a
            join v_CustomerDetails b on a.DistributorID=b.CustomerID
            where YEAR(GETDATE())=year(TGTDate) and RegionName !=''").ToList();
            List<DataPoint> N_Target_YTD = new List<DataPoint>();
            foreach (var item in NTGT_YTD)
            {
                N_Target_YTD.Add(new DataPoint(item.Nationals, item.Target));
            }
            ViewBag.N_Target_YTD = JsonConvert.SerializeObject(N_Target_YTD);


            ///************* National Sales
            var NS_YTD = db.Database.SqlQuery<ChartView.National_Sales_YTD>(@"
            Select * from 
            (
            Select 'Primary' as Nationals ,isnull(round(SUM(isnull(NetSales,0)),0),0) as Sales
            from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail 
            where InvoiceDate between  DATEADD(yy, DATEDIFF(yy, 0, GETDATE()), 0)
            and DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0) and InvoiceDate<DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0)
            and  isprimarysales=1
            union ALL
            Select 'Secondary' as Nationals ,isnull(round(SUM(isnull(NetSales,0)),0),0) as Sales
            from [10.168.14.36].DWDB.dbo.t_InvoiceWiseSalesDetail 
            where InvoiceDate between  DATEADD(yy, DATEDIFF(yy, 0, GETDATE()), 0)
            and DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0) and InvoiceDate<DATEADD(year, DATEDIFF(year, -1, GETDATE()), 0)
            and  isprimarysales=0
            ) as Main").ToList();
            List<DataPoint> N_Sales_YTD = new List<DataPoint>();
            foreach (var item in NS_YTD)
            {
                N_Sales_YTD.Add(new DataPoint(item.Nationals, item.Sales));
            }
            ViewBag.N_Sales_YTD = JsonConvert.SerializeObject(N_Sales_YTD);
        }
        public void StrikeRate_Region()
        {
            var SR = db.Database.SqlQuery<ChartView.StrikeRates>(@"
            Select RegionName,
            isnull(round( (cast(sum(MTDMemo) as float ) / cast(sum(TTOutlet) as float)*100),0),0)  as StrikeRate
            from
            (
            select RegionName,AreaName,Areaid,TerritoryID,TerritoryName,CustomerName,DSRCode,DSRName,RouteCode,MTDMemo,TTOutlet
            from
            (
            select RegionName,AreaName,Areaid,TerritoryID,TerritoryName,CustomerName,DSRCOde,DSRName,RouteCode
            from v_CustomerDetails a, t_DMSDSR b, t_DMSRoute c
            where a.CustomerID=b.DistributorID and b.DSRID=c.DSRID
            ) as Cust
            left outer join
            (
            Select BeatID, Sum(MTDMemo)as MTDMemo,sum(TTOutlet)as TTOutlet
            from
            (

            select BeatID,0 as TTOutlet,count(OutletID) as MTDMemo
            from
            (
            select BeatID, OutletID,sum(DeliveryAmount) as DelAmount
            from t_DMSOrder
            where DeliveryDate between DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and DeliveryDate< DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and IsDelivered=1 and DeliveryAmount>0
            group by BeatID,OutletID
            ) as mm group by BeatID
            union ALL
            select RouteID, COUNT(RetailID) as TTOutlet,0 as MTDMemo
            from t_DMSClusterOutlet where IsActive=1
            group by RouteID

            ) as main group by BeatID
            ) as Val on Cust.RouteCode=Val.BeatID
            ) as final where DSRCode>0 and  TTOutlet>0
            group by RegionName").ToList();
            List<DataPoint> SRList = new List<DataPoint>();
            foreach (var item in SR)
            {
                SRList.Add(new DataPoint(item.RegionName, item.StrikeRate));
            }
            //SRList.Add(new DataPoint("Central", 35));
            //SRList.Add(new DataPoint("East", 35));
            //SRList.Add(new DataPoint("North", 17));
            //SRList.Add(new DataPoint("South", 13));
            ViewBag.StrikeRate = JsonConvert.SerializeObject(SRList);


        }
        public void Order_Delivery_MTD()
        {


            var SR = db.Database.SqlQuery<ChartView.Bounce>(@"
            Select 'Order' as Name, 
            isnull(round(isnull(sum(nullif(NetAmount,0)),0),0),0) as Value 
            from t_DMSOrder
            where TranDate between DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and TranDate<DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            union all
            Select 'Delivery' as Name,
            isnull(round(sum(nullif(DeliveryAmount,0)),0),0) as Value 
            from t_DMSOrder
            where DeliveryDate between DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and DeliveryDate<DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and IsDelivered=1 and DeliveryAmount>0
            union ALL
            Select 
            'Bounce (%)' as Name,
            isnull(round((Bounce/cast(nullif(OrdAmount,0) as float)*100),2),0) as Value
            from(
            Select 
            isnull(SUm(OrdAmount),0) as OrdAmount, 
            isnull(sum(DelAmount),0)  as DelAmount,
            isnull(SUm(OrdAmount),0)-isnull(sum(DelAmount),0)  as Bounce
            from
            (Select sum(NetAmount) as OrdAmount ,0 as DelAmount
            from t_DMSOrder
            where TranDate between DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and TranDate<DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            union all
            Select 0 as OrdAmount, isnull(sum(DeliveryAmount),0) as DelAMount 
            from t_DMSOrder
            where DeliveryDate between DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
            and DeliveryDate<DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and IsDelivered=1 and DeliveryAmount>0
            ) as Main 
            ) as Final").ToList();
            List<DataPoint> LOV = new List<DataPoint>();
            foreach (var item in SR)
            {
                LOV.Add(new DataPoint(item.Name, item.Value));
            }
            //SRList.Add(new DataPoint("Order", 20000));
            //SRList.Add(new DataPoint("Delivery", 15000));
            //SRList.Add(new DataPoint("Bounce", 5000));
            //SRList.Add(new DataPoint("South", 13));
            ViewBag.BounceChart = JsonConvert.SerializeObject(LOV);


        }
        public void MonthWiseSales()
        {
            var Data = db.Database.SqlQuery<ChartView.MonthWiseSales>(@"              
            Select 
            Case
            When Months=1 then 'Jan' When Months=2 then 'Feb' When Months=3 then 'Mar' When Months=4 then 'April'
            When Months=5 then 'May' When Months=6 then 'June' When Months=7 then 'July' When Months=8 then 'Aug'
            When Months=9 then 'Sep' When Months=10 then 'Oct' When Months=11 then 'Nov' When Months=12 then 'Dec'
            End as Months,round(Sales/10000000,2) as Sales
            from
            (
            Select mm As Months,Sum(Sales) as Sales
            from 
            (
            Select month(InvoiceDate) as MM,Year(InvoiceDate) as YY,round(Sum(NetSales),0) as Sales
            from DWDB.dbo.t_InvoiceWiseSalesDetail
            where InvoiceDate between '01-Jan-2022' and '01-Jan-2023' and
            InvoiceDate<'01-Jan-2023'
            group by Month(InvoiceDate),YEAR(InvoiceDate) 
            ) as Main Group by MM 
            ) as Main ").ToList();
            //List<DataPoint> DataList = new List<DataPoint>();
            //foreach (var item in Data)
            //{
            //    DataList.Add(new DataPoint(item.Months, item.Sales));
            //}
            //ViewBag.MonthSales = JsonConvert.SerializeObject(DataList);
        }
        public ActionResult Logout()
        {
            Session["LoggedUserName"] = null;
            Session["LogedUserPassword"] = null;
            Session["LoggedUserFullName"] = null;
            Session["CustomerID"] = null;
            Session["selectedProducts"] = null;
            Session["InvoiceID"] = null;
            Session["OutletName"] = null;
            return RedirectToAction("Login", "User");
        }
        public void GetOperationDate()
        {
            int nCustomerId = (int)Session["CustomerID"];
            string date = db.DmsOutlet.FirstOrDefault(i => i.CustomerId == nCustomerId).OperationDate.ToShortDateString();


            TempData["OperationDate"] = Convert.ToDateTime(date).Day.ToString() + "-" + Convert.ToDateTime(date).Month.ToString() + "-" + Convert.ToDateTime(date).Year.ToString();
            DateTime dt = Convert.ToDateTime(date).AddDays(+1);
            TempData["NextOperationDate"] = dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString();


        }
        public ActionResult CheckOperationDate(int Status)
        {
            int nCustomerId = (int)Session["CustomerID"];
            string date = db.DmsOutlet.FirstOrDefault(i => i.CustomerId == nCustomerId).OperationDate.ToShortDateString();


            if (Convert.ToDateTime(date) == DateTime.Today)
            {
                if (Status == 1)
                {
                    return RedirectToAction("Index", "DmsSalesInvoice");
                }
                else
                {
                    return RedirectToAction("SecOrder", "SecOrder");
                }
            }

            else
            {
                TempData["CheckOperationDate"] = "Your Operation Date Doesn't Match with Current Date.";
                return RedirectToAction("Index", "Home");
            }

            //  return null;

        }
        public ActionResult DayEnd()
        {
            int nCustomerId = (int)Session["CustomerID"];
            var date = db.DmsOutlet.FirstOrDefault(i => i.CustomerId == nCustomerId).OperationDate.ToShortDateString();
            DateTime dt = new DateTime();
            dt = Convert.ToDateTime(date);
            //var DMS = new DmsOutlet();
            if (dt < DateTime.Today.AddDays(1))
            {
                DmsOutlet DMS = db.DmsOutlet.First(i => i.CustomerId == nCustomerId);
                DMS.OperationDate = dt.AddDays(1);
                db.SaveChanges();

            }
            //TempData["DayEnd"] = "1";
            return RedirectToRoute(new { Controller = "Home", Action = "Index" });
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    public class SalesPerformance_Value
    {
        public string RegionName { get; set; }
        public string Name { get; set; }
        public string AreaName { get; set; }
        public string TerritoryName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerShortName { get; set; }
        public string RouteName { get; set; }
        public int AreaID { get; set; }
        public int Areasort { get; set; }
        public int TerritoryID { get; set; }
        public int CustomerID { get; set; }
        public int CustomerTypeID { get; set; }
        public int RouteID { get; set; }


        public decimal TDTarget { get; set; }
        public decimal TDOrder { get; set; }
        public decimal TDDel { get; set; }
        public decimal LDOrder { get; set; }
        public decimal LDDelivery { get; set; }
        public decimal MTDOrder { get; set; }
        public decimal MTDDel { get; set; }
        public decimal CMTarget { get; set; }
        public decimal CMSales { get; set; }
        public decimal LMSales { get; set; }
        public decimal LMMTDSales { get; set; }
        public decimal LYSMSales { get; set; }
        public decimal YTDTGT { get; set; }
        public decimal YTDSales { get; set; }
        public decimal LYYTDSales { get; set; }

        public string Sql = @"
        isnull(convert(decimal,round(sum(TDTarget),0)),0) as TDTarget,
        isnull(convert(decimal,round(sum(TDOrder),0)),0) as TDOrder,
        isnull(convert(decimal,sum(TDDel)),0) as TDDel,
        isnull(convert(decimal,sum(LDOrder)),0) as LDOrder,
        isnull(convert(decimal,sum(LDDelivery)),0) as LDDelivery, 
        round(isnull(convert(decimal,sum(MTDOrder)),0),0) as MTDOrder,
        round(isnull(convert(decimal,sum(MTDDel)),0),0) as MTDDel, 
        ------------------Today---------------------------------
        round(isnull(convert(decimal,sum(CMTarget)),0),0) as CMTarget,
        round(isnull(convert(decimal,sum(CMSales)),0),0) as CMSales,
        round(isnull(convert(decimal,sum(LMSales)),0),0) as LMSales,
        round(isnull(convert(decimal,sum(LMMTDSales)),0),0) as LMMTDSales,
        round(isnull(convert(decimal,sum(LYSMSales)),0),0) as LYSMSales,
        round(isnull(convert(decimal,sum(YTDTGT)),0),0) as YTDTGT,
        round(isnull(convert(decimal,sum(YTDSales)),0),0) as YTDSales,
        round(isnull(convert(decimal,sum(LYYTDSales)),0),0) as LYYTDSales
        from (select
        QQ1.RegionID,RegionName,Areaid,AreaName,Areasort,
        TerritoryID,TerritoryName,CustomerTypeID,CustomerTypeName,
        QQ1.CustomerID,CustomerName,isnull(CustomerShortName,'N/A') as CustomerShortName,isnull(TownName,'N/A') as TownName,
        QQ1.DSRID,QQ1.DSRName,isnull(DesignationName,'N/A') as Designation,
        RouteID,RouteCode,RouteName,round(sum(TDTarget),0) as TDTarget,
        round(sum(TDOrder),0) as TDOrder,isnull(sum(TDDel),0) as TDDel,
        isnull(sum(LDOrder),0) as LDOrder,isnull(sum(LDDelivery),0) as LDDelivery, 
        round(isnull(sum(MTDDOrder),0),0) as MTDOrder,round(isnull(sum(MTDDel),0),0) as MTDDel, 
        ------------------Today---------------------------------
        round(isnull(sum(CMSecTGT),0),0) as CMTarget,round(isnull(sum(CMSecSales),0),0) as CMSales,
        round(isnull(sum(LMSecSales),0),0) as LMSales,round(isnull(sum(LMMTDSales),0),0) as LMMTDSales,
        round(isnull(sum(LYSMSales),0),0) as LYSMSales,round(isnull(sum(YTDTGT),0),0) as YTDTGT,
        round(isnull(sum(YTDSec),0),0) as YTDSales,round(isnull(sum(LYYTDSec),0),0) as LYYTDSales
        from
        (
        select TownName,RegionID,RegionName,Areaid,AreaName,Areasort,TerritoryID,TerritoryName,CustomerID,CustomerCode,CustomerName,CustomerTypeID,CustomerTypeName,CustomerShortName,DSRID,DSRCode,DSRName,RouteCode,RouteName, RouteID, TDTarget, TDDel,
        TDOrder,LDOrder,LDDelivery,MTDDOrder,MTDDel, CMSecSales, LMSecSales,LYSMSales , YTDTGT, YTDSec, LYYTDSec, CMSecTGT, LMSecTGT,LMMTDSAles,TotalWD,PassWD,((CMSecTGT/TotalWD)*PassWD)as MTDTGT
        from
        (
        select TownName,RegionID,RegionName,Areaid,AreaName,Areasort,TerritoryID,TerritoryName,CustomerID,CustomerCode,CustomerName,CustomerTypeID,CustomerTypeName,CustomerShortName,DSRID,DSRCode,DSRName,
        RouteCode,RouteName, Main.RouteID,TDTarget,TDOrder, LDOrder ,LDDelivery,MTDDOrder,MTDDel, CMSecSales, LMSecSales,LYSMSales, YTDSec, LYYTDSec, CMSecTGT, LMSecTGT,LMMTDSales,  YTDTGT,TDDel,
        (select count(ID)as WD from t_CalendarSales where  IssalesDay=1 and  TDate between DATEADD(mm,DATEDIFF(mm,0,GETDATE()),0)  and   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0) and TDate < DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0)) as TotalWD, 
        (select count(ID)as WD from t_CalendarSales where  IssalesDay=1 and  TDate between DATEADD(mm,DATEDIFF(mm,0,GETDATE()),0)  and   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0) and TDate <  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0) 
        AND TDate <  Convert(datetime,replace(convert(varchar, getdate(),6),'','-'),1)) as PassWD
        from
        (
        select BeatID as RouteID, sum(CMSecSales) as CMSecSales ,sum(LMSecSales) as LMSecSales, sum(YTDSec) as YTDSec, sum(LYYTDSec) as LYYTDSec, sum(CMSecTGT) as CMSecTGT,sum(LMSecTGT) as LMSecTGT ,sum(LMMTDSales) as LMMTDSales,sum(LYSMSales) as LYSMSales,
        sum(TDOrder) as TDOrder, sum(LDOrd) as LDOrder,sum(LDDelivery) as LDDelivery,sum(MTDDOrder) as MTDDOrder,sum(MTDDel) As MTDDel, sum(YTDTGT) as YTDTGT,sum(TDTarget) as TDTarget, sum(TDDel) as TDDel
        from
        (             
        ---------------------Current Month Sec Sales Start--------------
        Select RouteID as BeatID, round(sum(InvoiceAmount),0)  as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 
        0 as CMSecTGT,0 as LMSecTGT ,0 as LMMTDSales,0 as LYSMSales,0 as TDOrder, 0 as LDOrd,
        0 as LDDelivery,0 as MTDDOrder,0 As MTDDel,0 as YTDTGT,0 as TDTarget,0 as TDDel
        From t_DMSSalesInvoice a
        join t_DMSConsumer b on a.ConsumerID=b.ConsumerID
        join t_DMSClusterOutlet c on b.ReffID=c.RetailID
        where InvoiceDate between   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 0, 0)  and    DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0) and
        InvoiceDate < DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 1, 0)
        group by RouteID

        ---------------------Current Month Sec Sales End--------------
        union all
        ---------------------Last Month Sec Sales Start--------------
        Select RouteID as BeatID, 0 as CMSecSales, round(sum(InvoiceAmount),0) as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT ,0 as LMMTDSales,
        0 as LYSMSales,0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel,0 as YTDTGT,0 as TDTarget,0 as TDDel 
        From t_DMSSalesInvoice a
        join t_DMSConsumer b on a.ConsumerID=b.ConsumerID
        join t_DMSClusterOutlet c on b.ReffID=c.RetailID
        where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) - 1, 0)  and  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 0, 0) and
        InvoiceDate < DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 0, 0) 
        group by RouteID
        ---------------------Last Month Sec Sales End--------------
        union all
        ---------------------Last Month MTD Sales Start------------
        Select RouteID as BeatID, 0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT ,round(sum(InvoiceAmount),0) as LMMTDSales,0 as LYSMSales,0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel,0 as YTDTGT,0 as TDTarget,0 as TDDel
        From t_DMSSalesInvoice a
        join t_DMSConsumer b on a.ConsumerID=b.ConsumerID
        join t_DMSClusterOutlet c on b.ReffID=c.RetailID
        where InvoiceDate between  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) -1, 0) and DATEADD(MONTH, -1, DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0)) and
        InvoiceDate < DATEADD(MONTH, -1, DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), 0)) 
        group by RouteID
        ---------------------Last Month MTD Sales End------------
        union all
        ---------------------Last Year MTD Sales Start------------
        Select RouteID as BeatID, 0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT ,0 as LMMTDSales,
        round(sum(InvoiceAmount),0) as LYSMSales,0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel,0 as YTDTGT,0 as TDTarget,0 as TDDel
        From t_DMSSalesInvoice a
        join t_DMSConsumer b on a.ConsumerID=b.ConsumerID
        join t_DMSClusterOutlet c on b.ReffID=c.RetailID
        where InvoiceDate between DATEADD(mm , DATEDIFF(mm, 0, GETDATE()) -12, 0)  and  DATEADD(YEAR, -1, cast (GETDATE() as Date))  and 
        InvoiceDate  < DATEADD(YEAR, -1, cast (GETDATE() as Date))
        group by RouteID

        ---------------------Last  Year Month MTD Sales End------------
        union all
        ---------------------YTD  Sececondary Sales Start--------------
        Select RouteID as BeatID, 0 as CMSecSales, 0 as LMSecSales, round(sum(InvoiceAmount),0)  as YTDSec, 0 as LYYTDSec, 
        0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales,0 as LYSMSales,0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel,0 as YTDTGT,0 as TDTarget,0 as TDDel
        From t_DMSSalesInvoice a
        join t_DMSConsumer b on a.ConsumerID=b.ConsumerID
        join t_DMSClusterOutlet c on b.ReffID=c.RetailID
        where InvoiceDate between      DATEADD(yy, DATEDIFF(yy, 0, GETDATE()), 0)  and     DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and
        InvoiceDate  <  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0)
        group by RouteID
        ---------------------YTD  Sececondary Sales End--------------
        union all
        ---------------------Last Year YTD  Sececondary Sales Start--------------
        Select RouteID as BeatID, 0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, round(sum(InvoiceAmount),0) as LYYTDSec, 0 as CMSecTGT,
        0 as LMSecTGT ,0 as LMMTDSales,0 as LYSMSales,0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel,0 as YTDTGT,0 as TDTarget,0 As TDDel
        From t_DMSSalesInvoice a
        join t_DMSConsumer b on a.ConsumerID=b.ConsumerID
        join t_DMSClusterOutlet c on b.ReffID=c.RetailID
        where InvoiceDate between   DATEADD(yy, DATEDIFF(yy, 0, GETDATE()) - 1, 0)  and  DateAdd(dd, 1, DATEADD(yy, -1, GETDATE())) 
        and InvoiceDate < DateAdd(dd, 1, DATEADD(yy, -1, GETDATE()))
        group by RouteID

        ---------------------Last Year YTD  Sececondary Sales End--------------
        union all
        -----------------Current Month Target--------------
        select RouteID , 0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 
        round(sum(TSMTGTTO),0) as CMSecTGT,0 as LMSecTGT ,0 as LMMTDSales,0 as LYSMSales,0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel,0 as YTDTGT,0 as TDTarget,0 as TDDel
        from t_DMSTargetTO
        where TGTDate between   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) 
        and   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and TGTDate<  DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) 
        group by RouteID

        -----------------Current Month Target End --------------
        union all
        ---------------------YTD Target Start--------------------
        select RouteID ,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales,0 as LYSMSales, 0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel, round(Sum(TSMTGTTO),0) as YTDTGT,0 as TDTarget  ,0 as TDDel      
        from t_DMSTargetTO 
        where TGTDate between  DATEADD(yy, DATEDIFF(yy, 0, GETDATE()), 0)  
        and    DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and TGTDate<   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +1, 0) and   RouteID>0 
        group by  RouteID
        ---------------------YTD Target End--------------------
        union all
        -----------------Last Month Target--------------
        select RouteID as BeatID, 0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,round(sum(TSMTGTTO),0) as LMSecTGT ,0 as LMMTDSales,0 as LYSMSales,0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel, 0 as YTDTGT,0 as TDTarget,0 as TDDel
        from t_DMSTargetTO 
        where TGTDate between    DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) -1, 0)  and   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and   TGTDate<   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, 0) and RouteID>0

        group by RouteID
        -----------------Last Month Target End --------------
        union all
        -----------------Today Order-----(Yesterday Date)
        select BeatID,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales, 0 as LYSMSales, round(sum(NetAmount),0) as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel, 0 as YTDTGT,0 as TDTarget,0 as TDDel
        from t_DMSorder
        where TranDate between   DATEADD(dd,DATEDIFF(dd,0,GETDATE()),-1) and   DATEADD(dd,DATEDIFF(dd,0,GETDATE()),0) and TranDate < DATEADD(dd,DATEDIFF(dd,0,GETDATE()),0)
        group by BeatID
        -----------------Today Order End -----
        union all
        -----------------Today  Delivery-----( Todays Date)
        select BeatID,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales, 0 as LYSMSales, 0 as TDOrder, 
        0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel ,0 as YTDTGT,0 as TDTarget,round(sum(DeliveryAmount),0)  as TDDel
        from t_DMSorder
        where DeliveryDate between  DATEADD(dd,DATEDIFF(dd,0,GETDATE()),0) and   DATEADD(dd,DATEDIFF(dd,0,GETDATE()),+1) and DeliveryDate < DATEADD(dd,DATEDIFF(dd,0,GETDATE()),+1)
        and  IsDelivered=1 and DeliveryAmount>0
        group by BeatID            
        -----------------Today Delivery End-----(Todays Date Delivery)
        union all
                                  
        -----------------LD  Order-----(2 Days Back Order )
        select BeatID,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales,0 as LYSMSales, 0 as TDOrder, round(sum(NetAmount),0) as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel, 0 as YTDTGT,0 as TDTarget,0 as TDDel
        from t_DMSorder
        where Trandate between    DATEADD(dd,DATEDIFF(dd,0,GETDATE()),-2) and   DATEADD(dd,DATEDIFF(dd,0,GETDATE()),-1) and
        Trandate < DATEADD(dd,DATEDIFF(dd,0,GETDATE()),-1) 
        ---and BeatID=5
        group by BeatID
        -----------------LD Order End -----
        union all
        -----------------Last Day Delivery-----(Yesterday Delivery)
        select BeatID,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales, 0 as LYSMSales, 0 as TDOrder, 0 as LDOrd,round(sum(DeliveryAmount),0) as LDDelivery,0 as MTDDOrder,0 As MTDDel ,0 as YTDTGT,0 as TDTarget,0 as TDDel
        from t_DMSorder
        where DeliveryDate between  DATEADD(dd,DATEDIFF(dd,0,GETDATE()),-1) and   DATEADD(dd,DATEDIFF(dd,0,GETDATE()),0) and DeliveryDate < DATEADD(dd,DATEDIFF(dd,0,GETDATE()),0)
        and  IsDelivered=1 and DeliveryAmount>0
        group by BeatID            
        -----------------Last Day Delivery End-----(Yesterday Delivery)

        Union All
        -----------------MTD  Order-----
        select BeatID,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales, 0 as LYSMSales, 0 as TDOrder, 0 as LDOrd,0 as LDDelivery,Round(sum(NetAmount),0) as MTDDOrder,0 As MTDDel, 0 as YTDTGT,0 as TDTarget,0 as TDDel
        from t_DMSorder
        where Trandate  between    DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) +0, -1) and   dateadd(day,-1, cast(getdate() as date)) and
        Trandate < dateadd(day,-1, cast(getdate() as date)) 
        group by BeatID 
        -----------------MTD Order End -----
        union all
        -----------------MTD Delivery Start-----
        select BeatID,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales, 0 as LYSMSales, 0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,round(sum(DeliveryAmount),0) As MTDDel, 0 as YTDTGT,0 as TDTarget,0 as TDDel
        from t_DMSorder
        where DeliveryDate between   DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 0, 0) and  Convert(datetime, DateAdd(month, 0, Convert(date, GetDate()))) and
        DeliveryDate <Convert(datetime, DateAdd(month, 0, Convert(date, GetDate()))) and IsDelivered=1
        group by BeatID            
        -----------------MTD Delivery End-----        
        union all
        -----------------TD Order Target Start-----
        select RouteID,0 as CMSecSales, 0 as LMSecSales, 0 as YTDSec, 0 as LYYTDSec, 0 as CMSecTGT,0 as LMSecTGT,0 as LMMTDSales, 0 as LYSMSales, 0 as TDOrder, 0 as LDOrd,0 as LDDelivery,0 as MTDDOrder,0 As MTDDel, 0 as YTDTGT,round(sum(TSOTargetTO),0)  as TDTarget,0 as TDDel
        from t_DMSDailyTargetTO
        where TGTDate between   CAST( GETDATE() AS Date )  and  CAST(dateadd(dd,+1, GETDATE()) AS Date) and   TGTDate <    CAST(dateadd(dd,+1, GETDATE()) AS Date)
        group by RouteID           
        -----------------TD Order Target End-----
        )As SlTGT group by BeatID

        )as SecSLTGT 

        left outer join
        (
        Select -1 RegionID,RegionName,Areaid,AreaName,Areasort,TerritoryID,TerritoryName,a.CustomerID,CustomerCode,CustomerName,CustomerTypeID,CustomerTypeName,CustomerShortName,
        b.DSRID,DSRCode,DSRName,c.RouteID,RouteCode,RouteName,TownName
        from v_CustomerDetails a, t_DMSDSR b, t_DMSRoute c
        where a.CustomerID=b.DistributorID and b.DSRID=c.DSRID 
        )AS Main on SecSLTGT.RouteID=Main.RouteID 

        ) AS Gfinal   
        ) as QQ1 
        left outer join v_EmployeeDetails DS on QQ1.DSRID=DS.EmployeeID
        group by TownName,QQ1.RegionID,RegionName,Areaid,AreaName,Areasort,TerritoryID,TerritoryName,
        QQ1.CustomerID,CustomerCode,CustomerName,CustomerShortName,
        CustomerTypeID,CustomerTypeName,QQ1.DSRID,QQ1.DSRCode,QQ1.DSRName,DesignationName,RouteID,RouteCode,RouteName
        ) as Final";

    }
}
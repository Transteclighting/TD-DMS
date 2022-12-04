using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    public class MainChartModel
    {
        public string AreaName { get; set; }
        public string TerritoryName { get; set; }
        public string ShowroomCode { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string WeekName { get; set; }
        public int WeekNo { get; set; }
        public int CMonth { get; set; }
        public string PGCategory { get; set; }
        public string PGName { get; set; }
        public string MAGName { get; set; }
        public string Brand { get; set; }
        public int TYearSalesQty { get; set; }
        public double TYearInvoiceAmount { get; set; }
        public double TYearNetSale { get; set; }
        public int TYearTargetQty { get; set; }
        public double TYearTargetAmount { get; set; }
        public int LYearSalesQty { get; set; }
        public double LYearInvoiceAmount { get; set; }
        public double LYearNetSale { get; set; }
    }
    public class StockAgingChartModel
    {
        public string PGCategory { get; set; }
        public string PGName { get; set; }
        public string MAGName { get; set; }
        public Int64 Saleable { get; set; }
        public int DisplayStock { get; set; }
        public Int64 Defective { get; set; }
        public Int64 StockQty { get; set; }
        public double StockValue { get; set; }
    }
    public class LeadChartModel
    {
        public int TYear { get; set; }
        public int TMonth { get; set; }
        public int WeekNo { get; set; }
        public DateTime TDate { get; set; }
        public string Channel { get; set; }
        public string ShowroomCode { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string PGCategory { get; set; }
        public string PGName { get; set; }
        public string MAGName { get; set; }
        public string BrandDesc { get; set; }
        public int TargetQty { get; set; }
        public double TargetAmount { get; set; }
        public int LeadQty { get; set; }
        public double LeadAmount { get; set; }
        public int ConverttoSalesQty { get; set; }
        public double ConverttoSalesAmount { get; set; }
        public int TotalSalesQty { get; set; }
        public double TotalNetSale { get; set; }
    }
    public class ChartViewModel
    {
        public List<MAGwiseChartModel> MAGwiseChartModel { get; set; }
        public List<PGwiseChartModel> PGwiseChartModel { get; set; }
        public List<AreawiseChartModel> AreawiseChartModel { get; set; }
        public List<CatwiseChartModel> CatwiseChartModel { get; set; }
        public NationalwiseChartModel NationalwiseChartModel { get; set; }
        public List<StockAgingChartModel> Castwisestock { get; set; }
        public List<StockAgingChartModel> MAGwisestock { get; set; }
        public List<StockAgingChartModel> PGwisestock { get; set; }
        public List<MonthwiseChartModel> MonthwiseChartModel { get; set; }

        //public StockAgingChartModel StockAgingChartModel { get; set; }
        public List<DailySalesChartModel> AreawiseDalysales { get; set; }
        public List<DailySalesChartModel> N1wiseDalysales { get; set; }
        public List<DailySalesChartModel> N2wiseDalysales { get; set; }
        public List<DailySalesChartModel> EstorewiseDalysales { get; set; }

        //public List<DailySalesChartModel> ShowroomwiseDalysales { get; set; }
    }
    public class ChartViewModelNew
    {
        public List<SalesChartModel> MAGwiseChartModel { get; set; }
        public List<SalesChartModel> PGwiseChartModel { get; set; }
        public List<SalesChartModel> AreawiseChartModel { get; set; }
        public List<SalesChartModel> CatwiseChartModel { get; set; }
        public NationalwiseChartModel NationalwiseChartModel { get; set; }
        public List<SalesChartModel> MonthwiseChartModel { get; set; }

        public List<StockChartModel> Castwisestock { get; set; }
        public List<StockChartModel> MAGwisestock { get; set; }
        public List<StockChartModel> PGwisestock { get; set; }
    }
    public class SalesChartModel
    {
        public string Name { get; set; }
        public double NetSale { get; set; }
        public double TargetAmount { get; set; }
    }
    public class DailySalesChartModel
    {
        public string Area { get; set; }
        public string Territory { get; set; }
        public string ShowroomCode { get; set; }
        public int WeekNo { get; set; }
        public int CMonth { get; set; }
        public int CYear { get; set; }
        public string TDate { get; set; }
        public string TDayName { get; set; }
        public double WeekDayTarget { get; set; }
        public double NetSales { get; set; }
    }

    public class CATChartModel
    {
        public int TMonth { get; set; }
        public string AreaName { get; set; }
        public string TerritoryName { get; set; }
        public string ShowroomCode { get; set; }
        public string PGCategory { get; set; }
        public string PGName { get; set; }
        public int TyearTargetQty { get; set; }
        public double TyearTargetValue { get; set; }
        public int TyearSalesQty { get; set; }
        public double TyearNetsale { get; set; }
        public int LyearSalesQty { get; set; }
        public double LyearNetsale { get; set; }
        public int TyearNOI { get; set; }
        public int LyearNOI { get; set; }
    }
    public class CATChartViewModel
    {
        public List<CATChartModel> PGwiseChartModel { get; set; }
        public List<CATChartModel> AreawiseChartModel { get; set; }
        public CATChartModel NationalwiseChartModel { get; set; }
        public List<CATChartModel> Data { get; set; }
        public List<CATChartModel> Cat1ChartModel { get; set; }
        public List<CATChartModel> Cat2ChartModel { get; set; }


    }


    public class StockChartModel
    {
        public string Name { get; set; }
        public Int64 Saleable { get; set; }
        public int DisplayStock { get; set; }
        public Int64 Defective { get; set; }
        public Int64 StockQty { get; set; }
        public double StockValue { get; set; }
    }
    public class MAGwiseChartModel
    {
        public string MAGName { get; set; }
        public double TYearNetSale { get; set; }
        public double TYearTargetAmount { get; set; }
    }
    public class PGwiseChartModel
    {
        public string PGName { get; set; }
        public double TYearNetSale { get; set; }
        public double TYearTargetAmount { get; set; }
    }
    public class AreawiseChartModel
    {
        public string AreaName { get; set; }
        public double TYearNetSale { get; set; }
        public double TYearTargetAmount { get; set; }
    }
    public class CatwiseChartModel
    {
        public string PGCategory { get; set; }
        public double TYearNetSale { get; set; }
        public double TYearTargetAmount { get; set; }
    }
    public class MonthwiseChartModel
    {
        public string WeekName { get; set; }
        public double TYearNetSale { get; set; }
        public double TYearTargetAmount { get; set; }
    }
    public class NationalwiseChartModel
    {
        public string NationalName { get; set; }
        public double TYearNetSale { get; set; }
        public double TYearTargetAmount { get; set; }
    }
    
}
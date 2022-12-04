namespace Cj.web.Enums
{
    public enum ProductStockTranType
    {
        GoodsReceive = 1,
        Transfer = 3,
        Invoice = 5,
        AddStock = 7,
        DeductStock = 8,
        IssueSalesPromotion = 9,
        IssueFixedAsset = 10,
        IssueCeServiceReplacement = 11,
        ReturnDefectiveProduct = 12,
        IssueCompanyConsumption = 13,
        IssueLeServiceReplacenent = 14,
        IssueProductReturnToSupplier = 15,
        IssueForProduction = 16,
        DeliveryBreakageReplacement = 17,
        IssueShortDeliveryProduct = 18,
        IssueScrapSale = 19,
        IssueDefectiveOrScrap = 20,
        ReceiveServiceDefectiveProduct = 21,
        ReturnSalesPromotion = 22,
        ReturnFixedAsset = 23,
        IssueForCannibalize = 24,
        All = -1
    }
}
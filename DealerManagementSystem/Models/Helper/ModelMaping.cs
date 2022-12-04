using DealerManagementSystem.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models.Helper
{
    public static class MappingExtensions
    {

        public static DmsSecondarySalesOrderDetail ToModel(this ProductForOrder model)
        {
            DmsSecondarySalesOrderDetail order = new DmsSecondarySalesOrderDetail
            {
                ProductId = model.ProductId,
                Qty = model.OrderQty
            };
            return order;
        }
        public static ProductForOrder ToModel(this DmsSecondarySalesOrderDetail model)
        {
            ProductForOrder order = new ProductForOrder
            {
                ProductId = model.ProductId,
                OrderQty = model.Qty
            };
            return order;
        }
        public static DmsSecondarySalesOrder ToModel(this OrderEditViewModel model)
        {
            DmsSecondarySalesOrder order = new DmsSecondarySalesOrder
            {
                        OrderId = model.OrderId,
                        UpdateDate= DateTime.Now,
                        Remarks= model.Remarks,
            };
            return order;
        }
        public static OrderEditViewModel ToModel(this DmsSecondarySalesOrder model)
        {
            OrderEditViewModel order = new OrderEditViewModel
            {
                OrderId = model.OrderId,
                Remarks = model.Remarks,
                IsOrderToTaggedShowroom = false,
                OrderProducts = null,
            };
            return order;
        }
    }
}
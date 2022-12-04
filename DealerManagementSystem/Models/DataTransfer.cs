using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.Models
{
    [Table("t_DataTransfer")]
    public class DataTransfer
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; }
        public Int64 DataId { get; set; }
        public int WarehouseId { get; set; }
        public int TransferType { get; set; }
        public int IsDownload { get; set; }
        public int BatchNo { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
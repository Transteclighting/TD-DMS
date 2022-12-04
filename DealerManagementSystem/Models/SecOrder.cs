using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_DMSOrder")]
    public class SecOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TranID { get; set; }
        public string TranNo { get; set; }
        public DateTime TranDate { get; set; }
        public int TranTypeID { get; set; }
        public int DistributorID { get; set; }
        public int? BeatID { get; set; }

        public int? OutletID { get; set; }
        public string Remarks { get; set; }
        public string MemoNo { get; set; }
        public decimal? GrossAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NetAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public int? TsoID { get; set; }
        public int? TsmID { get; set; }
        public int? RsmID { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? SAID { get; set; }
        public string Reason { get; set; }
        public int? HaveOrder { get; set; }
        public decimal? DeliveryAmount { get; set; }
        public bool? IsDelivered { get; set; }
        public System.DateTime? DeliveryDate { get; set; }
        public bool? IsConfirmByDB { get; set; }
        public int? IsProcess { get; set; }
        public List<Routes> Routes { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealerManagementSystem.Models
{
    [Table("t_Brand")]
    public class Brand
    {
        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        [Display(Name = "Brand")]
        public string BrandDesc { get; set; }
        public int IsActive { get; set; }
        public int? BrandLevel { get; set; }
        public int? ParentId { get; set; }
        public int? BrandPos { get; set; }
        public byte? UploadStatus { get; set; }
        public DateTime? UploadDate { get; set; }
        public DateTime? DownloadDate { get; set; }
        public byte? RowStatus { get; set; }
        public byte? Terminal { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DealerManagementSystem.Models
{
    [Table("t_ShowroomAsset")]
    public class POSMachine
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int AssetID { get; set; }
        public string AssetName { get; set; }
    }
}
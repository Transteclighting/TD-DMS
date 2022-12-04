using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cj.web.protal.Models
{
    [Table("t_MarketGroup")]
    public class MarketGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MarketGroupID { get; set; }
        public int? Pos { get; set; }
        public string MarketGroupCode { get; set; }
        public string MarketGroupDesc { get; set; }
        public int MarketGroupType { get; set; }
        public int? ParentID { get; set; }
        public int? EmployeeID { get; set; }
        public int ChannelID { get; set; }
        public string ShortName { get; set; }
    }
}
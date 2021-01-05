using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    [Table("scmIPRequest")]
    public class scmIPRequest
    {
        [Key]
        public int id { get; set; }
        public int requestId { get; set; }
        public int supplyId { get; set; }
        public int? children { get; set; }
        public int? currentBalance { get; set; }
        public int? adjustment { get; set; }
        public string program { get; set; }
        public string adjustmentReason { get; set; }
        public int? stockForChildren { get; set; }
        public int tenantId { get; set; }
        public string userName { get; set; }
        public DateTime? updateDate { get; set; }
        public int? emergency { get; set; }
        public string emergencyReason { get; set; }
        public string pndUserId { get; set; }
        public string unicefUserId { get; set; }
        public bool? approveByPnd { get; set; }
        public bool? approveByUnicef { get; set; }
        public string commentByPnd { get; set; }
        public string commentByUnicef { get; set; }
        public string pndInchargeId { get; set; }
        public string unicefInchargeId { get; set; }
        public string commentByIp { get; set; }
        public double? winterization { get; set; }
        public double? ipbalance { get; set; }
        public double? buffer { get; set; } = 0.2;
    }
}

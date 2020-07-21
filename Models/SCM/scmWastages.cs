using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmWastages
    {
        [Key]
        public int Id { get; set; }
        public int IPDistributionId{ get; set; }
        public int WasteId { get; set; }
        public DateTime DateWasted{ get; set; }
        public double? Quantity { get; set; }
        public string Reason { get; set; }
        public string ActionTaken { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}

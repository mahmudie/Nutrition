using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmTransfers
    {
        [Key]
        public int id { get; set; }
        public int ipdistributionId { get; set; }
        public int whId { get; set; }
        public int? quantitiy { get; set; }
        public string userName { get; set; }
        public string remarks { get; set; }
        public int tenantId { get; set; }
        public DateTime? updateDate { get; set; }
        public DateTime? transferDate { get; set; }
    }
}

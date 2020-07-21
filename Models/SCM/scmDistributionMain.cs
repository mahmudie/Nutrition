using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmDistributionMain
    {
        [Key]
        public int DistributionId { get; set; }
        public int RoundId { get; set; }
        public int ImpId { get; set; }
        public string ProvinceId { get; set; }
        public DateTime DistributionDate { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string ReceiverUser { get; set; }
    }
}

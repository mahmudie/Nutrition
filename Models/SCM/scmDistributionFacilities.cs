using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmDistributionFacilities
    {
        [Key]
        public int id { get; set; }
        public int ipdistributionId { get; set; }
        public int supplyId { get; set; }
        public int facilityId { get; set; }
        public int facilityTypeId { get; set; }
        public int estimation { get; set; }
        public double distribution { get; set; }
        public string program { get; set; }
        public string userName { get; set; }
        public int tenantId { get; set; }
        public DateTime updateDate { get; set; }
        public string approve { get; set; }
        public DateTime? distributionDate { get; set; }
        public double? distributionb { get; set; }
        public DateTime? distributionbDate { get; set; }
    }
}

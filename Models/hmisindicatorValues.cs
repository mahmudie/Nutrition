using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    [Table("HMISIndicatorValues")]
    public class HMISIndicatorValues
    {
        [Key]
        public Int64 id { get; set; }
        public int facilityId { get; set; }
        public int facilityTypeId { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int indicatorId { get; set; }
        public string grantId { get; set; }
        public string program { get; set; }
        public string implementer { get; set; }
        public int num { get; set; }
        public int denom { get; set; }
        public string userName { get; set; }
        public int tenantId { get; set; }
        public DateTime uploadDate { get; set; }
    }

    [Table("DistPopulation")]
    public class DistPopulation
    {
        [Key]
        public int Id { get; set; }
        public int PopYear { get; set; }
        public string DistCode { get; set; }
        public int Pop { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
        public DateTime UploadDate { get; set; }
    }
}

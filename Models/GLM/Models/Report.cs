using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class Report
    {
        [Key]
        public string Id { get; set; }
        public string ProvinceId { get; set; }
        public string DistrictId { get; set; }
        public int FacilityId { get; set; }
        public int FacilityTypeId { get; set; }
        public int ImpId { get; set; }
        public string DataCollectorOffice { get; set; }
        public string ReportedBy { get; set; }
        public DateTime? ReportPreparedDate { get; set; }
        public DateTime? ReportReceivedDate { get; set; }
        public int? TenantId { get; set; }
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Double? ReportLat { get; set; }
        public Double? ReportLon { get; set; }

        [ForeignKey("DataForm")]
        public int DataFormId { get; set; }
        public DataForm Dataform { get; set; }
        public ICollection<DateValue> DateValues { get; set; }

        [NotMapped]
        public bool ExpiryWarning { get; set; } = false;
    }
}

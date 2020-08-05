using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    [Table("vReports")]
    public class ReportsView
    {
        [Key]
        public string Id { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public int FacilityID { get; set; }
        public string Facility { get; set; }
        [Column("ImpAcronym")]
        public string Implementer { get; set; }
        public string DataCollectorOffice { get; set; }
        public string ReportedBy { get; set; }
        public DateTime? ReportPreparedDate { get; set; }
        public DateTime? ReportReceivedDate { get; set; }
        public int TenantId { get; set; }

        public string UserName { get; set; }
        public DateTime UpdateDate { get; set; }

        [ForeignKey("DataForm")]
        public int DataFormId { get; set; }
        public DataForm Dataforms { get; set; }
        public ICollection<DateValue> DateValues { get; set; }

        [NotMapped]
        public bool ExpiryWarning { get; set; } = false;
    }
}

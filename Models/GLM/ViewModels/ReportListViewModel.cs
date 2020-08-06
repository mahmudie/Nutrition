using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM.ViewModels
{
    public class ReportListViewModel
    {
        public string ReportId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Implementer { get; set; }
        public int FacilityId { get; set; }
        public string Facility { get; set; }
        public string ReportedBy { get; set; }
        public string DataCollectorOffice { get; set; }
        public DateTime? PreparedDate { get; set; }
        public string DataForm { get; set; }
        public bool HasExpiryWarning { get; set; } = false;
        public bool IsCompleted { get; set; } = false;
    }
}

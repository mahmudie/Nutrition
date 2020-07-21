using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class ReportViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Province")]
        public string ProvinceId { get; set; }

        [Required]
        [Display(Name = "District")]
        public string DistrictId { get; set; }

        [Required]
        [Display(Name = "Facility")]
        public int FacilityId { get; set; }

        [Required]
        [Display(Name = "Facility Type")]
        public int FacilityTypeId { get; set; }

        [Required]
        [Display(Name = "Implementer")]
        public int ImpId { get; set; }

        [Required]
        [Display(Name = "Data Collector Office")]
        public string DataCollectorOffice { get; set; }

        [Required]
        [Display(Name = "Data Collector Name")]
        public string ReportedBy { get; set; }

        [Required]
        [Display(Name = "Data Collection Date")]
        public DateTime? ReportPreparedDate { get; set; }

        [Required]
        [Display(Name = "Report Recieved Date")]
        public DateTime? ReportReceivedDate { get; set; }
        public Double? ReportLat { get; set; }
        public Double? ReportLon { get; set; }

        [Required]
        [Display(Name = "Data Form")]
        public int DataFormId { get; set; }
        public int? TenantId { get; set; }
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }

        public IEnumerable<DataForm> DataForms { get; set; }

        public Dictionary<string, string> FacilityTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "TypeOne", "Type One" },
                    { "TypeTwo", "Type Two" },
                    { "TypeThree", "Type Three" },
                };
            }
        }

        public Dictionary<string, string> DataCollectorOffices
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "PND", "PND" },
                    { "PNO", "PNO" },
                    { "SP Nutrition Officer", "SP Nutrition Officer" },
                    { "Nutrition Extender", "Nutrition Extender" },
                    { "UINICEF", "UNICEF" },
                    { "Other", "Other" },
                };
            }
        }
    }
}

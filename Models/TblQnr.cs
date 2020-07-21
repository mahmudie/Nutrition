using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class TblQnr
    {
        public int Qnrid { get; set; }
        [Display(Name = "Year")]
        public int? ReportYear { get; set; }
        [Display(Name = "Quarter")]
        public int? ReportMonth { get; set; }
        [Display(Name = "Reported At")]
        public DateTime? ReportingDate { get; set; }
        public int Implementer { get; set; }
        public string Province { get; set; }
        public string Highlights { get; set; }
        public string IpdsamAdmissionsTrend { get; set; }
        public string IpdsamPerformanceTrend { get; set; }
        public string OpdsamAdmissionsTrend { get; set; }
        public string OpdsamPerformanceTrend { get; set; }
        public string OpdmamAdmissionsTrend { get; set; }
        public string OpdmamPerformanceTrend { get; set; }
        public string Iycf { get; set; }
        public string Micronutrients { get; set; }
        public string UserName { get; set; }
        public string message{get;set;}
        public DateTime? UpdateDate { get; set; }
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        [ForeignKey("Implementer")]
        public virtual Implementers ImpNavigation { get; set; }
        [ForeignKey("Province")]
        public virtual Provinces ProvNavigation { get; set; }
        public int Tenant { get; set; }

        public virtual TblkpStatus Status { get; set; }
    }
}

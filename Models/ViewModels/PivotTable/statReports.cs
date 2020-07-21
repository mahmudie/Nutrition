
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models.ViewModels.PivotTable
{
    [Table("statReports")]
    public class statReports
    {
        [Key]
        public string NMRID { get; set; }
        public int FacilityID { get; set; }
        public string FacilityName { get; set; }
        public int FacTypeCode { get; set; }
        public string TypeAbbrv { get; set; }
        public string ProvCode { get; set; }
        public string DistCode { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int mYear { get; set; }
        public int mMonth { get; set; }
        public int Quarter { get; set; }
        public int mQuarter { get; set; }
        public int samAdmittedTotal { get; set; }
        public int samAdmittedMale { get; set; }
        public int samAdmittedFemale { get; set; }
        public int samAdmitIpd { get; set; }
        public int samAdmitIpdMale { get; set; }
        public int samAdmitIpdFemale { get; set; }
        public int samAdmitOpd { get; set; }
        public int samAdmitOpdMale { get; set; }
        public int samAdmitOpdFemale { get; set; }
        public int mamU5 { get; set; }
        public int mamPlw { get; set; }
        public int withSamServices { get; set; }
        public int withSamIpd { get; set; }
        public int withSamOpd { get; set; }
        public int mamU5Services { get; set; }
        public int mamPlwServices { get; set; }
        public int samCured { get; set; }   
        public int samDeaths { get; set; }
        public int samDefaults { get; set; }
        public int ipdCured { get; set; }
        public int ipdDeaths { get; set; }
        public int ipdDefaults { get; set; }
        public int ipdExists { get; set; }
        public int samExists { get; set; }
    }
}
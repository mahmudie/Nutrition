

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models.ViewModels.PivotTable
{
    public class CreateReq
    {
        [RequiredAttribute]
        [Display(Name = "Province")]
        public string ProvCode { get; set; }

        [RequiredAttribute]
        public int Year { get; set; }
        public string Title { get; set; }
        [Range(1, 2)]
        public int option { get; set; }
        public string CollumnName { get; set; }
        public string FileName { get; set; }
        public string DistCode {get;set;}
        public int Facility{get;set;}
        public string Implementer {get;set;}
    }

    public class FormatFilterReq
    {
        [RequiredAttribute]
        [Display(Name = "Province")]
        public string ProvCode { get; set; }
        [Display(Name = "District")]
        public string DistCode { get; set; }
        public int FacilityId { get; set; }
        [RequiredAttribute]
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
        public string Implementer { get; set; }
    }

    public class FormatFilterReqImp
    {
        [RequiredAttribute]
        [Display(Name = "Implementer")]
        public string Implementer { get; set; }
        [RequiredAttribute]
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public int MonthFrom { get; set; }
        public int MonthTo { get; set; }
       
    }

}
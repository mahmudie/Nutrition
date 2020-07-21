using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public partial class qnrViewModel
    {
        public int Qnrid{get;set;}
        public string Highlights { get; set; }
        public string IpdsamAdmissionsTrend { get; set; }
        public string IpdsamPerformanceTrend { get; set; }
        public string OpdsamAdmissionsTrend { get; set; }
        public string OpdsamPerformanceTrend { get; set; }
        public string OpdmamAdmissionsTrend { get; set; }
        public string OpdmamPerformanceTrend { get; set; }
        public string Iycf { get; set; }
        public string Micronutrients { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name="Last Update")]
        public DateTime? UpdateDate { get; set; }
         [Required]
        public int Implementer { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name="Created at")]
        public DateTime? OpeningDate { get; set; }
        [Required]
        [Range(1390,1500, ErrorMessage = "Enter a valid Year")]
        public int Year { get; set; }
        [Required]
        [Display(Name="Quarter")]
        [Range(1,4,ErrorMessage ="Enter a valid month")]
        public int Month { get; set; }
        public int? StatusId { get; set; }
        public string UserName { get; set; }
        public string Province {get;set;}





    }
}

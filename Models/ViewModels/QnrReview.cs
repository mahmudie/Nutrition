using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public class QnrReview
    {
        [Range(3, 4, ErrorMessage = "Invalid number")]
        [Required]
        [Display(Name="Status")]
        public int? StatusId { get; set; }
        [Required]
        public int Qnrid { get; set; }
        public string message { get; set; }
        public string Highlights { get; set; }
        public string IpdsamAdmissionsTrend { get; set; }
        public string IpdsamPerformanceTrend { get; set; }
        public string OpdsamAdmissionsTrend { get; set; }
        public string OpdsamPerformanceTrend { get; set; }
        public string OpdmamAdmissionsTrend { get; set; }
        public string OpdmamPerformanceTrend { get; set; }
        public string Iycf { get; set; }
        public string Micronutrients { get; set; }
        public string Province { get; set; }
        
        public string Implementer   { get; set; }
        public int? Month   { get; set; }
        public int? Year   { get; set; }




    }
}

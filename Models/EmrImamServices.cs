using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class EmrImamServices
    {
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int IndicatorId { get; set; }
        [Required]
        public int ErnmrId { get; set; }
        [Display(Name="Male")]
        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int? Male { get; set; }
         [Display(Name="Female")]
        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int? Female { get; set; }
         [Display(Name="Cured")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of cured children")]
        public int? Cures { get; set; }
        [Display(Name="Death")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of death children")]
        public int? Deaths { get; set; }
        [Display(Name = "Defaulters")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of children defaulted")]
        public int? Defaulters { get; set; }
        [Display(Name = "Refer outs")]
        [Range(0, int.MaxValue, ErrorMessage = "Number of children reffered out")]
        public int? Referouts { get; set; }
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual tlkpEmrIndicators tlkpEmrIndicators { get; set; }
        public virtual Ernmr Ernmr { get; set; }
    }
}

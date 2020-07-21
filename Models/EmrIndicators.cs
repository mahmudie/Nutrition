using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class EmrIndicators
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
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual tlkpEmrIndicators lkpEmrIndicators { get; set; }
        public virtual Ernmr Ernmr { get; set; }
    }
}

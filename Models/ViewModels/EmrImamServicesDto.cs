using System;
using System.ComponentModel.DataAnnotations;


namespace DataSystem.Models
{
    public class EmrImamServicesDto
    {
        public int IndicatorId { get; set; }
        public int ErnmrId { get; set; }
        [Display(Name = "Male")]
        public int? Male { get; set; }
        [Display(Name = "Female")]
        public int? Female { get; set; }
        [Display(Name = "Cures")]
        public int? Cures { get; set; }
        [Display(Name = "Deaths")]
        public int? Deaths { get; set; }
        [Display(Name = "Defaulters")]
        public int? Defaulters { get; set; }
        [Display(Name = "Refer Outs")]
        public int? Referouts { get; set; }
        [Display(Name = "Updated at")]
        public DateTime? UpdateDate { get; set; }
        public string UserName { get; set; }
        public string IndicatorName { get; set; }
        public int Type { get; set; }
    }
}

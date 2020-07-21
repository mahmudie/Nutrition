using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class SurveyResults
    {
        [Key]
        [Display(Name = "IndResultId")]
        public int IndResultId { get; set; }
        [Display(Name = "Survey Id")]
        public int SurveyId { get; set; }
        [Display(Name = "DisaggregId")]
        public int DisaggregId { get; set; }
        [Display(Name = "CategoryID")]
        public int CategoryId { get; set; }
        [Display(Name = "ThemeId")]
        public int ThemeId { get; set; }
        [Display(Name = "Indicator")]
        public int IndicatorId { get; set; }
        [Display(Name = "IndicatorValue")]
        public double? IndicatorValue { get; set; }
        [Display(Name = "CI (National)")]
        public string CINational { get; set; }
        [Display(Name = "Year")]
        public int Year { get; set; }
        [Display(Name = "Month")]
        public int? Month { get; set; }
        public string UserName { get; set; }
        public string Remarks { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int TenantId { get; set; }

        [ForeignKey("DisaggregId")]
        public virtual LkpDisaggregation LkpDisaggregations { get; set; }
        [ForeignKey("ThemeId")]
        public virtual lkpThematicArea LkpThematicAreas { get; set; }
        [ForeignKey("SurveyId")]
        public virtual SurveyInfo SurveyInfoNav { get; set; }
    }
}

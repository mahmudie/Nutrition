using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class hmisindicators
    {
        [Display(Name = "IndicatorId")]
        public Int32 IndicatorId { get; set; }
        [Display(Name = "Indicator details")]
        [Required(ErrorMessage = "Indicator is required.")]
        public string IndicatorDescription { get; set; }
        [Display(Name = "Indicator Data Source")]
        [Required(ErrorMessage = "Data source is required.")]
        public string IndDataSource { get; set; }
        [Display(Name = "Caluculation")]
        public string IndCaluculation { get; set; }
        [Display(Name = "Indicator Type")]
        [Required(ErrorMessage = "Indicator Type is required.")]
        public string IndType { get; set; }
    }
}

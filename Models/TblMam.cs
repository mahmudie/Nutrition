using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    [MamAttribute]
    public partial class TblMam
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid number")]
        public int Mamid { get; set; }
        [Required]
        public string Nmrid { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name = "Total  at the beginning of the month")]
        public int? Totalbegin { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name = "W/H <-2 to -3 Z score")]
        public int? Zscore23 { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name = "MUAC ≥115 <125mm ")]
        public int? Muac12 { get; set; }
        

        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name = "MUAC < 230mm")]
        public int? Muac23 { get; set; }
       
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name = "Referred In")]
        public int? ReferIn { get; set; }
     
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Absents { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Cured { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Deaths { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Defaulters { get; set; }

        [Display(Name = "Refer Out")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Transfers { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? NonCured { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name = "Male")]
        public int? TMale { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name = "Female")]
        public int? TFemale { get; set; }
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }
        public virtual TlkpSfp Mam { get; set; }
        public virtual Nmr Nmr { get; set; }
    }
}

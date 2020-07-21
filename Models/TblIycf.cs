using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TblIycf
    {
        [Range(1, int.MaxValue, ErrorMessage = "Invalid number")]
        public int Iycfid { get; set; }
        public string Nmrid { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name="Mother With Child < 5")]
        public int? MChildU5months { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name="Mother With Child 5-24 month")]
        public int? MChild524months { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name="Pregnanat Women")]
        public int? Pregnanatwomen { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [Display(Name="First Visit")]
        public int? Firstvisit { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Revisit { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? ReferIn { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? ReferOut { get; set; }
        public string UserName { get; set; }
        [Display(Name="Last Update")]        
        public DateTime? UpdateDate { get; set; }

        public virtual TlkpIycf Iycf { get; set; }
        public virtual Nmr Nmr { get; set; }
    }
}

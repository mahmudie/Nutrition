using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class Samreq
    {
        public Samreq()
        {
            SamreqDetails = new HashSet<SamreqDetails>();
        }

        public long Rid { get; set; }
        [Required]
        public string ProvCode { get; set; }
        [Required]
              [Display(Name = "Implementer")]
  
        public string ImpCode { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public short Month { get; set; }

        public short? YearTo { get; set; }
        public short? MonthTo { get; set; }
        public short? YearFrom { get; set; }
        public short? MonthFrom { get; set; }

        [Display(Name = "PH")]

        public short Ph { get; set; }
        [Display(Name = "DH")]

        public short Dh { get; set; }
        [Display(Name = "CHC")]

        public short Chc { get; set; }
        [Display(Name = "SHC")]

        public short Shc { get; set; }
        [Display(Name = "MHT")]

        public short Mht { get; set; }
        [Display(Name = "BHC")]

        public short Bhc { get; set; }
        public string ReqBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UserName { get; set; }

        public int Tenant { get; set; }

        public virtual ICollection<SamreqDetails> SamreqDetails { get; set; }
    }
}

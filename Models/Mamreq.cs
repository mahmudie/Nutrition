using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class Mamreq
    {
        public Mamreq()
        {
            MamreqDetails = new HashSet<MamreqDetails>();
        }

        public long Rid { get; set; }
        public string ProvCode { get; set; }
        [Display(Name = "Implementer")]
        public string ImpCode { get; set; }
        public int Year { get; set; }
        public short Month { get; set; }
        public int? ReqYear { get; set; }
        public short? ReqMonth { get; set; }
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

        public virtual ICollection<MamreqDetails> MamreqDetails { get; set; }
    }
}

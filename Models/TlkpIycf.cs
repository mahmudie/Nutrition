using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TlkpIycf
    {
        public TlkpIycf()
        {
            TblIycf = new HashSet<TblIycf>();
        }
        [Display(Name = "ID")]
        public int Iycfid { get; set; }
        [Display(Name = "Cause Consultation")]
        public string CauseConsultation { get; set; }
        [Display(Name = "Cause Short Name")]
        public string CauseShortName { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<TblIycf> TblIycf { get; set; }
    }
}

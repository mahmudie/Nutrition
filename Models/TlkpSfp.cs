using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TlkpSfp
    {
        public TlkpSfp()
        {
            TblMam = new HashSet<TblMam>();
        }

        public int Sfpid { get; set; }
        [StringLength(50)]
        [Display(Name="Age Group")]
        public string AgeGroup { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<TblMam> TblMam { get; set; }
    }
}

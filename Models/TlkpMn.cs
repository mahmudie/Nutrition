using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TlkpMn
    {
        public TlkpMn()
        {
            TblMn = new HashSet<TblMn>();
        }
        [Display(Name ="Id")]
        public int Mnid { get; set; }
        [Display(Name = "Item")]
        public string Mnitems { get; set; }
        [Display(Name = "Active")]
        public bool? Active { get; set; }

        public virtual ICollection<TblMn> TblMn { get; set; }
    }
}

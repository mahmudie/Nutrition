using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TlkpOtptfu
    {
        public TlkpOtptfu()
        {
            TblOtp = new HashSet<TblOtp>();
            TblOtptfu = new HashSet<TblOtptfu>();
        }

        public int Otptfuid { get; set; }
        [StringLength(50)]
        public string AgeGroup { get; set; }
        public bool? Active { get; set; }

        public virtual ICollection<TblOtp> TblOtp { get; set; }
        public virtual ICollection<TblOtptfu> TblOtptfu { get; set; }
    }
}

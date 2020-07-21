using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TblkpStatus
    {
        public TblkpStatus()
        {
            Nmr = new HashSet<Nmr>();
        }
        [Display(Name = "ID")]
        [Range(1,1500,ErrorMessage ="Enter a valid key.")]
        public int StatusId { get; set; }
        [Display(Name = "Status ")]
        [StringLength(150)]
        public string StatusDescription { get; set; }

        public virtual ICollection<Nmr> Nmr { get; set; }
    }
}

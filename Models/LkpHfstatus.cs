using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class LkpHfstatus
    {
        public LkpHfstatus()
        {
            Nmr = new HashSet<Nmr>();
        }
        [Range(1, 3000, ErrorMessage = "ID is not valid.")]
        [Display(Name ="Id")]
        public int HfactiveStatusId { get; set; }
        [Display(Name = "Description")]
        [StringLength(150)]
        public string HfstatusDescription { get; set; }

        public virtual ICollection<Nmr> Nmr { get; set; }
    }
}

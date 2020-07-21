using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRegions
    {
        public scmRegions()
        {
            warehouses = new HashSet<scmWarehouses>();
        }
        [Key]
        public int RegionId { get; set; }
        [Required()]
        [Display(Name ="Region Long Name")]
        public string RegionLong { get; set; }
        [Required]
        [Display(Name = "Region Short Name")]
        public string RegionShort { get; set; }

        public virtual ICollection<scmWarehouses> warehouses { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class tlkpEmrIndicators
    {
        public tlkpEmrIndicators()
        {
            EmrImamServices = new HashSet<EmrImamServices>();
            EmrIndicators = new HashSet<EmrIndicators>();

        }
        [Display(Name = "ID")]
        public int IndicatorId { get; set; }
        [Required(ErrorMessage ="Must enter name of indicator")]
        [Display(Name = "Indicator Name")]
        public string IndicatorName { get; set; }
        [Required]
        [Range(1, 2, ErrorMessage = "Enter 1 for Indicators 2 for IMAM Service")]
        public int Type { get;set;}

        public virtual ICollection<EmrImamServices> EmrImamServices { get; set; }
        public virtual ICollection<EmrIndicators> EmrIndicators { get; set; }

    }
}

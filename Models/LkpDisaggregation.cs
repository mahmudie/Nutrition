using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class LkpDisaggregation
    {
        public LkpDisaggregation()
        {
            SurveyResults = new HashSet<SurveyResults>();
        }
        [Display(Name = "Id")]
        public int DisaggregId { get; set; }
        [Display(Name ="CategoryId")]
        public int CategoryId { get; set; }
        public int Ordno { get; set; }
        [Display(Name = "Disaggregation")]
        public string Disaggregation { get; set; }

        public virtual ICollection<SurveyResults> SurveyResults { get; set; }
        [ForeignKey("CategoryId")]
        public virtual LkpCategory LkpCategory { get; set; }
    }
}

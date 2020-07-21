using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class lkpThematicArea
    {
        public lkpThematicArea()
        {
            SurveyResults = new HashSet<SurveyResults>();
        }
        [Display(Name = "ThemeId")]
        public int ThemeId { get; set; }
        [Display(Name = "Thematic Area")]
        public string ThematicArea { get; set; }

        public virtual ICollection<SurveyResults> SurveyResults { get; set; }
    }
}

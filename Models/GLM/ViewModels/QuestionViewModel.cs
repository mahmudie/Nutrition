using DataSystem.Models.GLM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.GLM
{
    public class QuestionViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Section")]
        public long SectionId { get; set; }

        public IEnumerable<Section> Sections { get; set; }

        [Display(Name = "Sort Order")]
        public int? SortOrder { get; set; } = 0;
    }
}

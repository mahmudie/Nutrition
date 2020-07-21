using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class ColumnViewModel
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

        [Required]
        [Display(Name = "Column Type")]
        [StringLength(100)]
        public string ColumnType { get; set; } = "standard";

        [Display(Name = "Dividend Column")]
        public long? DividendColumn { get; set; } = null;

        [Display(Name = "Divisor Column")]
        public long? DivisorColumn { get; set; } = null;

        public List<Column> DividendColumns { get; set; }
        public List<Column> DivisorColumns { get; set; }

        public Dictionary<string, string> ColumnTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "standard", "Standard" },
                    { "percentage", "Percentage" }
                };
            }
        }
    }
}

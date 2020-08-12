using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class Question
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }

        [ForeignKey("Section")]
        public long SectionId { get; set; }

        public Section Section { get; set; }
        public ICollection<Field> Fields { get; set; }
        public int? SortOrder { get; set; } = 0;
    }
}

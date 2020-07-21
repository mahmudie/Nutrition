using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class Section
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public int SortOrder { get; set; }

        [ForeignKey("DataForm")]
        public int DataFormId { get; set; }
        public DataForm DataForm { get; set; }

        public ICollection<Column> Columns { get; set; }
        public ICollection<Question> Questions { get; set; }
    }
}

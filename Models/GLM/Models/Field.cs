using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class Field
    {
        [Key]
        public long Id { get; set; }
        public string DataType { get; set; }
        public string InputType { get; set; }
        public string YesNoDefaultCaption { get; set; } = "N/A";
        public bool IsRequired { get; set; } = true;

        public bool IsExpiryDate { get; set; } = false;
        public int? ExpiryWarningPeriod { get; set; } = null;

        [ForeignKey("Question")]
        public long QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("Column")]
        public long ColumnId { get; set; }
        public Column Column { get; set; }

        public ICollection<FieldOption> FieldOptions { get; set; }
    }
}

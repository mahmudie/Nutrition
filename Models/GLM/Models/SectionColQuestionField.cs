using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM.Models
{
    [Table("VSectionColQuestionField")]
    public class SectionColQuestionField
    {
        public long SectionId { get; set; }
        public long FieldId { get; set; }
        public string DataType { get; set; }
        public string InputType { get; set; }
        public string SectionTitle { get; set; }
        public int SortOrder { get; set; }
        public int DataFormId { get; set; }
        public long QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public int QSortOrder { get; set; }
        public long ColumnId { get; set; }
        public string ColumnTitle { get; set; }
        public int CSortOrder { get; set; }
        public string ColumnType { get; set; }
        
    }
}

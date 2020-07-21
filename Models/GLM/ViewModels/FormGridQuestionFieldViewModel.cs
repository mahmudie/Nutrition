using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace DataSystem.Models.GLM
{
    public class FormGridQuestionFieldViewModel
    {
        public long? Id { get; set; } = null;
        public long QuestionId { get; set; }
        public long ColumnId { get; set; }

        public string DataType { get; set; }
        public string InputType { get; set; }

        public string Data { get; set; } = null;

        public bool IsPercentageValue { get; set; } = false;

        public string RequiredAttr { get; set; } = "";

        public ICollection<FieldOption> FieldOptions { get; set; }

        public Dictionary<byte, string> YesNoOptions { get; set; }
    }
}

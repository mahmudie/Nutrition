using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class FormGridQuestionField
    {
        public long? Id { get; set; } = null;
        public long QuestionId { get; set; }
        public long ColumnId { get; set; }

        public string DataType { get; set; }
        public string InputType { get; set; }

        public string Data { get; set; } = null;

        public ICollection<FieldOption> FieldOptions { get; set; }
        public Dictionary<string, string> YesNoOptions {
            get
            {
                return new Dictionary<string, string>
                {
                    { "No", "No" },
                    { "Yes", "Yes" }
                };
            }
        }
    }
}

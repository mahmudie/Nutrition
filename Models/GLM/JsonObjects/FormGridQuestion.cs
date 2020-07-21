using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class FormGridQuestion
    {
        public FormGridQuestion()
        {
            this.Fields = new List<FormGridQuestionField>();
        }

        public string QuestionTitle { get; set; }
        public List<FormGridQuestionField> Fields { get; set; }
    }
}

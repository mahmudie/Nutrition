using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class FormGridQuestionViewModel
    {
        public FormGridQuestionViewModel()
        {
            this.Fields = new List<FormGridQuestionFieldViewModel>();
        }

        public string QuestionTitle { get; set; }
        public List<FormGridQuestionFieldViewModel> Fields { get; set; }
    }
}

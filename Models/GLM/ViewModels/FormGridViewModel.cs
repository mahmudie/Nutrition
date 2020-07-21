using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DataSystem.Models.GLM
{
    public class FormGridViewModel
    {
        public FormGridViewModel()
        {
            this.Columns = new List<Column>();
            this.Questions = new List<FormGridQuestionViewModel>();
        }

        public List<Column> Columns { get; set; }
        public List<FormGridQuestionViewModel> Questions { get; set; }
        public bool MultiColumn { get; set; } = false;
    }
}

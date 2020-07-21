using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class FormGrid
    {
        public FormGrid()
        {
            this.Columns = new List<Column>();
            this.Questions = new List<FormGridQuestion>();
        }

        public List<Column> Columns { get; set; }
        public List<FormGridQuestion> Questions { get; set; }
        public bool MultiColumn { get; set; } = false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class SectionGridViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public FormGridViewModel Grid { get; set; }
    }
}

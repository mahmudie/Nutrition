using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class ReportDetailsForm
    {
        public ReportDetailsForm()
        {
            this.SectionGrids = new List<SectionGrid>();
        }

        public List<SectionGrid> SectionGrids { get; set; }

        [Required]
        public string ReportId { get; set; }

        public Report Report { get; set; }
    }
}

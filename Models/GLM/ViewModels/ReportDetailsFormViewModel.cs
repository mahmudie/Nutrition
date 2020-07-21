using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace DataSystem.Models.GLM
{
    public class ReportDetailsFormViewModel
    {
        public ReportDetailsFormViewModel()
        {
            this.SectionGrids = new List<SectionGridViewModel>();
        }

        public List<SectionGridViewModel> SectionGrids { get; set; }

        [Required]
        public string ReportId { get; set; }

        public Report Report { get; set; }
    }
}

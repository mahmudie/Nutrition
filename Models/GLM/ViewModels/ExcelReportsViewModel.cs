using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM.ViewModels
{
    public class ExcelReportsViewModel
    {
        [Required]
        public string ReportId { get; set; }

        [Required]
        [Display(Name = "Section")]
        public List<long> SectionIds { get; set; }

        public List<Section> Sections { get; set; }
    }
}

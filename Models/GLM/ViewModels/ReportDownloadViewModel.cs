using DataSystem.Models.GLM.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM.ViewModels
{
    public class ReportDownloadViewModel
    {
        [Required]
        [Display(Name = "Province")]
        public string ProvinceId { get; set; }

        [Required]
        public Int32 Year { get; set; }

        public List<ProvinceDropdownDto> Provinces { get; set; }
        public List<int?> Years { get; set; }
    }
}

using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class SectionViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        [Required]
        [Display(Name = "Data Form")]
        public int DataFormId { get; set; }
        public List<DataForm> DataForms { get; set; }
    }
}

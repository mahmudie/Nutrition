using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.ViewModels
{
    public partial class mreqvm
    {
        public string ProvCode { get; set; }
        [Display(Name = "Implementer")]
        public string ImpCode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [Display(Name = "no of month")]
        public int numMonth { get; set; }

        public string ReqBy { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.ViewModels
{
    [Table("FormatYear")]
    public class FormatYear
    {
        [Key]
        [Display(Name = "Year From")]
        public int YearFrom { get; set; }
        [Display(Name = "Year To")]
        public string YearTo { get; set; }
    }
}

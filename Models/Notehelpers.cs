using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    [Table("Notehelper")]
    public class Notehelpers
    {
        [Key]
        public int Id { get; set; }
        public string Tip { get; set; }
        public string FormName { get; set; }
        public string SectionName { get; set; }
        public string SectionCode { get; set; }
        public string Url { get; set; }
    }
}

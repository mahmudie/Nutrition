using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.USI
{
    public class Usilookup
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string indicatorName { get; set; }
        public Boolean isActive { get; set; }
    }
}

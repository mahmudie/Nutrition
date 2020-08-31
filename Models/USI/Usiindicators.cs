using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.USI
{
    public class Usiindicators
    {
        [Key]
        public int id { get; set; }
        public int usiId { get; set; }
        [Required]
        public int indicatorId { get; set; }
        [Required]
        public int value { get; set; }
    }
}

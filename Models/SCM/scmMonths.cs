using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmMonths
    {
        [Key]
        public int MonthId { get; set; }
        public int MonthName { get; set; }
    }
}

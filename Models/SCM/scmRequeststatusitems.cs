using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRequeststatusitems
    {
        [Key]
        public int id { get; set; }
        public string statusName { get; set; }
        public Boolean isActive { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmAveragelevel
    {
        [Key]
        public int id { get; set; }
        public int averagelevelId { get; set; }
        public bool isActive { get; set; }
        public string UserName { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}

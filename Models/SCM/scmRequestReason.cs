using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRequestReason
    {
        [Key]
        public int reasonId { get; set; }
        public string reasonName { get; set; }
    }
}

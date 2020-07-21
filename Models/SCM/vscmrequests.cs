using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class vscmrequests
    {
        [Key]
        public int requestId { get; set; }
        public string implementer { get; set; }
        public string province { get; set; }
        public string requestType { get; set; }
        public int startYear { get; set; }
        public int startMonth { get; set; }
        public int endYear { get; set; }
        public int endMonth { get; set; }
    }
}

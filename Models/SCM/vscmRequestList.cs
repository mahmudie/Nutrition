using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class vscmRequestList
    {
        [Key]
        public int RequestId { get; set; }
        public string Implementer { get; set; }
        public string Province { get; set; }
        public string Requesttype { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string Yearmonthfrom { get; set; }
        public string Yearmonthto { get; set; }
        public DateTime? DateOfRequest { get; set; }
        public string RequestBy { get; set; }
        public string PeriodName { get; set; }
        public string UserName { get; set; }
    }
}

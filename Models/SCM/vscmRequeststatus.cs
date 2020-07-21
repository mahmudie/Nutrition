using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class vscmRequeststatus
    {
        [Key]
        public int requestId { get; set; }
        public string Implementer { get; set; }
        public string Province { get; set; }
        public string RoundCode { get; set; }
        public Boolean? revievedByPnd { get; set; }
        public Boolean? reviewedByUnicef { get; set; }
        public Boolean? requestViewed { get; set; }
        public Boolean? distributionCompleted { get; set; }
        public Boolean? requestOntime { get; set; }
        public DateTime? dateCompleted { get; set; }
        public string remarks { get; set; }
    }
}

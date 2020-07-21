using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRequeststatus
    {
        [Key]
        public int statusId { get; set; }
        public int requestId { get; set; }
        public Boolean? revievedByPnd { get; set; }
        public Boolean? reviewedByUnicef { get; set; }
        public Boolean? requestViewed { get; set; }
        public Boolean? distributionCompleted { get; set; }
        public Boolean? requestOntime { get; set; }
        public DateTime? dateCompleted { get; set; }
        public string remarks { get; set; }
        public string userName { get; set; }
        public int tenantId { get; set; }
        public DateTime updateDate { get; set; }
        public bool? finalizeandemail { get; set; }
    }
}

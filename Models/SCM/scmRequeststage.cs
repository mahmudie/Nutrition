using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRequeststage
    {
        [Key]
        public int id { get; set; }
        public int? requestId { get; set; }
        public int statusId { get; set; }
        public Boolean? confirmed { get; set; }
        public DateTime? statusUpdateDate { get; set; }
        public string userName { get; set; }
        public DateTime? updateDate { get; set; }
        public string remarks { get; set; }
        public bool? finalizeandemail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmHFRequest
    {
        //public scmHFRequest()
        //{
        //    scmHFReqDetails = new HashSet<scmHFReqDetails>();
        //}

        [Key]
        public int HFReportId {get;set;}
        public int RequestId { get; set; }
        public int FacilityId { get; set; }
        public int FacilityTypeId { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public DateTime UpdateDate { get; set; }

        //public virtual ICollection<scmHFReqDetails> scmHFReqDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmHFsAcknowledgement
    {
        [Key]
        public int id { get; set; }
        public int distributionId { get; set; }
        public int facilityId { get; set; }
        public string acknowledgeBy { get; set; }
        public DateTime? dateOfAcknoledge { get; set; }
        public Boolean? acknowledge { get; set; }
        public DateTime? updateDate { get; set; }
        public string userName { get; set; }
        public string message { get; set; }
        public string waybillNumber { get; set; }
    }
}

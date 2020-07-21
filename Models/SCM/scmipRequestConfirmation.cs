using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmipRequestConfirmation
    {
        [Key]
        public int id { get; set; }
        public int requestId { get; set; }
        public bool isSubmitted { get; set; }
        public DateTime? submissionDate { get; set; }
        public string emailMessage { get; set; }
        public int reasonId { get; set; }
        public string userName { get; set; }
        public int tenantId { get; set; }
        public DateTime? updateDate { get; set; }
        public bool? sendEmail { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmEmail
    {
        [Key]
        public int id { get; set; }
        public int distributionId { get; set; }
        public string emailToUser { get; set; }
        public string emailFrom { get; set; }
        public string message { get; set; }
        public DateTime? dateSent { get; set; }
        public string emailToEmail { get; set; }
    }
}

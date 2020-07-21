using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class vscmiprequest
    {
        [Key]
        public int id { get; set; }
        public int requestId { get; set; }
        public string item { get; set; }
        public string program { get; set; }
        public int? children { get; set; }
        public int? totalEstimation { get; set; }
        public Single? totalBuffer { get; set; }
        public int currentBalance { get; set; }
        public int? adjustment { get; set; }
        public string adjustmentReason { get; set; }
        public int? emergency { get; set; }
        public string emergencyReason { get; set; }
        public Single? totalRequest { get; set; }
        public Single? ipdsamFactor { get; set; }
        public Single? opdsamFactor { get; set; }
    }
}

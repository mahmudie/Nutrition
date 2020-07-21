using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    [Table("scmdashrequest")]
    public class scmdashrequest
    {
        [Key]
        public string ID { get; set; }
        public int RequestId { get; set; }
        public string Type { get; set; }
        public int YearOfRequest { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public string Program { get; set; }
        public string Item { get; set; }
        public int Children { get; set; }
        public int Ballance { get; set; }
        public int NewStock { get; set; }
        public double BufferStock { get; set; }
        public int Adjustment { get; set; }
        public double? Total { get; set; }
        public string Tracker { get; set; }
    }
}

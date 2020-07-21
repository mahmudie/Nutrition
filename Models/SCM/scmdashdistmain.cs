using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    [Table("scmdashdistmain")]
    public class scmdashdistmain
    {
        [Key]
        public int DistributionId { get; set; }
        public string ID { get; set; }
        public string IP { get; set; }
        public string Location { get; set; }
        public string Item { get; set; }
        public string BatchNumber { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime IssueDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime DateFrom { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime DateTo { get; set; }
        public int Quantity { get; set; }
    }  
}

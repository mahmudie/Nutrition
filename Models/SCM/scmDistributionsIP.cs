using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmDistributionsIP
    {

        [Key]
        public int? id { get; set; }
        public int? distributionId { get; set; }
        public int stockId { get; set; }
        [Required()]
        public int whId { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int quantity { get; set; }
        [Display(Name = "Requested")]
        public int requested { get; set; }
        [Required]
        [Display(Name = "Batch Number")]
        public string batchNumber { get; set; }
        [Required()]
        public string issueTo { get; set; }
        [Required]
        public DateTime issueDate { get; set; }
        [Required]
        public string issueBy { get; set; }
        public DateTime updateDate { get; set; }
        public string userName { get; set; }
        public int tenantId { get; set; }
    }
}

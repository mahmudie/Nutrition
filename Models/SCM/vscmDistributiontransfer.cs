using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class vscmDistributiontransfer
    {
        [Key]
        public int id { get; set; }
        public string roundCode { get; set; }
        public string roundDescription { get; set; }
        public DateTime? periodFrom { get; set; }
        public DateTime? periodTo { get; set; }
        public string warehouseName { get; set; }
        public string item { get; set; }
        public string batchNumber { get; set; }
        public int quantity { get; set; }
        public DateTime? issueDate { get; set; }
        public string issueBy { get; set; }
        public string issueTo { get; set; }
        public int tenantId { get; set; }
        public string attachment { get; set; }
    }
}

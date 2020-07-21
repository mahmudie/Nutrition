using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class rptIPStockmovement
    {
        [Key]
        public int id { get; set; }
        public string implementer { get; set; }
        public string consignee { get; set; }
        public int tenantId { get; set; }
    }
}

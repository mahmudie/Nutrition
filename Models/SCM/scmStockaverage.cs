using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmStockaverage
    {
        [Key]
        public int id { get; set; }
        public string type { get; set; }
        public int year { get; set; }
        public int supplyId { get; set; }
        public string program { get; set; }
        public int totalNeeds { get; set; }
        public int facilityTypeId { get; set; }
    }
}

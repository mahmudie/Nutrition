using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class vscmstockwastages
    {
        [Key]
        public int Id { get; set; }
        public int StockId { get; set; }
        public string BatchNumber { get; set; }
        public string WateType { get; set; }
        public double Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Quarter { get; set; }
        public string StockItem { get; set; }
        public string Implementer { get; set; }
        public string Province { get; set; }
    }
}

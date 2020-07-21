using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmStocks
    {

        [Key]
        public int StockId { get; set; }
        [Required()]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Required]
        [Display(Name = "Batch Number")]
        public string BatchNumber { get; set; }
        [Required()]
        public int WhId { get; set; }
        [Required]
        [Display(Name = "Date Received")]
        public DateTime DateReceived { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public DateTime UpdateDate { get; set; }
        public virtual scmWarehouses scmWarehousesNav { get; set; }
        public virtual TlkpSstock TlkpSstockNav { get; set; }
    }
}

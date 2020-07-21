using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmStockBalance
    {

        [Key]
        public int StockId { get; set; }
        [Required()]
        public int ItemId { get; set; }
        [Required()]
        public string Item { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Required]
        [Display(Name = "Batch Number")]
        public string BatchNumber { get; set; }
        [Required()]
        public string RegionLong { get; set; }
        [Required()]
        public string ProvName { get; set; }
        public string ImpAcronym { get; set; }
        public int? Distributions { get; set; }
        public string StockItem { get; set; }
        public int Balance { get; set; }
    }
}

using DataSystem.Models.SCM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class TlkpSstock
    {
        public TlkpSstock()
        {
            TblStockIpt = new HashSet<TblStockIpt>();
            TblStockOtp = new HashSet<TblStockOtp>();
            SamreqDetails = new HashSet<SamreqDetails>();

        }

        public int SstockId { get; set; }
        [StringLength(50)]
        public string Item { get; set; }
        [Range(0, 400, ErrorMessage = "Invalid number")]
        public int? Persachet { get; set; }
        public bool? Active { get; set; }
        
        public float Buffer{get;set;}
        public float IPDSAMZarib{get;set;}
        public float OPDSAMZarib{get;set;}

        public string Comments { get; set; }

        public virtual ICollection<TblStockIpt> TblStockIpt { get; set; }
        public virtual ICollection<TblStockOtp> TblStockOtp { get; set; }
        public virtual ICollection<SamreqDetails> SamreqDetails { get; set; }
        [ForeignKey("ItemId")]
        public virtual ICollection<scmStocks> scmStocks { get; set; }

    }
}

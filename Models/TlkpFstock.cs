using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TlkpFstock
    {
        public TlkpFstock()
        {
            TblFstock = new HashSet<TblFstock>();
            MamreqDetails = new HashSet<MamreqDetails>();

        }
        [Display(Name = "ID")]
        public int StockId { get; set; }
        [Display(Name = "Item")]
        public string Item { get; set; }
        [Display(Name = "Amount Kg")]
        public decimal? DistAmountKg { get; set; }
        public bool? Active { get; set; }
        public float Buffer{get;set;}
        public float Zarib{get;set;}

        public virtual ICollection<TblFstock> TblFstock { get; set; }
        public virtual ICollection<MamreqDetails> MamreqDetails { get; set; }

    }
}

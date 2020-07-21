using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TblFstock
    {
        public int StockId { get; set; }
        public string Nmrid { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? OpeningBalance { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? QuantityReceived { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? QuantityDistributed { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? QuantityTransferred { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? QuantityReferin { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        
        public int? Losses { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? QuantityReturned { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? ExpectedRecepients { get; set; }
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual TlkpFstock Stock { get; set; }
        public virtual Nmr Nmr { get; set; }

    }
}

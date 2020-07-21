using System;

namespace DataSystem.Models.ViewModels
{
    public class fstockViewModel
    {
        public int StockId { get; set; }
        public string Nmrid { get; set; }
        public int? OpeningBalance { get; set; }
        public int? QuantityReceived { get; set; }
        public int? QuantityDistributed { get; set; }
        public int? QuantityTransferred { get; set; }
        public int? QuantityReferin { get; set; }
        public int? Losses { get; set; }
        public int? QuantityReturned { get; set; }
        public int? ExpectedRecepients { get; set; }
        public decimal? weight { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string Item { get; set; }
    }
}

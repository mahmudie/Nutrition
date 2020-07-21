using System;

namespace DataSystem.Models.ViewModels
{
    public class stockinDto
    {
        public int SstockId { get; set; }
        public string Nmrid { get; set; }
        public Decimal? Openingbalance { get; set; }
        public Decimal? Received { get; set; }
        public Decimal? Used { get; set; }
        public Decimal? Expired { get; set; }
        public Decimal? Damaged { get; set; }
        public Decimal? Loss { get; set; }
        public string Item { get; set; }
        public string UserName { get; set; }

    }
}

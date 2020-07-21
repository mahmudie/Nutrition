using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models.ViewModels
{
    [Table("Format_mamstock_report")]
    public class Formatmamstockreport
    {
        [Key]
        public string NMRID {get;set;}
        public int FacilityId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string ProvCode { get; set; }
        public string Province { get; set; }
        public string DistCode { get; set; }
        public double? ExpectedRecepients { get; set; }
        public double? Losses { get; set; }
        public double? OpeningBalance { get; set; }
        public double? QuantityDistributed { get; set; }
        public double? QuantityReceived { get; set; }
        public double? QuantityReferin { get; set; }
        public double? QuantityReturned { get; set; }
        public double? QuantityTransferred { get; set; }
        public int stockID { get; set; }
        public int Time { get; set; }
        public string Implementer { get; set; }
    }
}
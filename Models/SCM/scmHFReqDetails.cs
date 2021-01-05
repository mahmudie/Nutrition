using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmHFReqDetails
    {
        [Key]
        public int id { get; set; } 
        public int requestId { get; set; }
        public int facilityId { get; set; }
        public int facilityTypeId { get; set; }
        public int supplyId { get; set; }
        public int children { get; set; }
        [Range(0,0.9999)]
        public double? buffer { get; set; }
        public int? currentBalance { get; set; }
        public int? adjustment { get; set; }
        public string adjComment { get; set; }
        public int? stockForChildren { get; set; }
        public string program { get; set; }
        public int tenantId { get; set; }
        public string userName { get; set; }
        public DateTime updateDate { get; set; }
        public string approved { get; set; }
        public string Esttype { get; set; }

        [ForeignKey("supplyId")]
        public virtual TlkpSstock Stocks { get; set; }
    }

    public class scmrptRequestpivot
    {
        [Key]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int FacilityId { get; set; }
        public int FacilityTypeId { get; set; }
        public int SupplyId { get; set; }
        public int? Children { get; set; }
        public double Buffer { get; set; }
        public int? CurrentBalance { get; set; }
        public int? Adjustment { get; set; }
        public string AdjComment { get; set; }
        public int? StockForChildren { get; set; }
        public string Program { get; set; }
        public string FacilityName { get; set; }
        public string Item { get; set; }
        public string TypeAbbrv { get; set; }
        public string District { get; set; }
        public int? TotalNeeded { get; set; }
        public string Esttype { get; set; }
        public int RequesttypeId { get; set; }



        [ForeignKey("SupplyId")]
        public virtual TlkpSstock Stocks { get; set; }
    }
}

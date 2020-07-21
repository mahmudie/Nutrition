using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    [Table("TempHMISIndicatorValues")]
    public class TemphmisindicatorValues
    {
        [Key]
        public Int64 Id { get; set; }
        public int FacilityId { get; set; }
        public int FacilityTypeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int IndicatorId { get; set; }
        public string GrantId { get; set; }
        public string Program { get; set; }
        public string Implementer { get; set; }
        public int? Num { get; set; }
        public int? Denom { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
        public DateTime UploadDate { get; set; }
    }

    [Table("TempDistPopulation")]
    public class TempDistPopulation
    {
        [Key]
        public int Id { get; set; }
        public int PopYear { get; set; }
        public string DistCode { get; set; }
        public int Pop { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
        public DateTime UploadDate { get; set; }
    }
}

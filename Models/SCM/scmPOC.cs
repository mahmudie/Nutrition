using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    [Table("scmPOC")]
    public class scmPOC
    {
        [Key]
        public int PocId { get; set; }
        public int FacilityId { get; set; }
        public int FacilityTypeId { get; set; }
        public string DistrictId { get; set; }
        public string ProvinceId { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    public class FacilitViewModel
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string Implementer { get; set; }
    }
}

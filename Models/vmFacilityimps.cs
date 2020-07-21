using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class vmFacilityimps
    {
        [Key]
        public int FacilityId { get; set; }
        public int ImpId { get; set; }
        public string Implementer { get; set; }
        public string DistrictId { get; set; }
        public string ProvinceId { get; set; }
        public int FacilityTypeId { get; set; }
        public string FacilityName { get; set; }
        public string FacilityNameDari { get; set; }
        public string FacilityNamePashto { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }

    }
}

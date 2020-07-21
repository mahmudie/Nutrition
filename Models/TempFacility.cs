using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    [Table("TempFacilities")]
    public class TempFacilities
    {
        [Key]
        public int Id { get; set; }
        public Int32 FacilityId {get;set;}
        public String DistrictCode { get; set; }
        public String FacilityName { get; set; }
        public String FacilityNameDari { get; set; }
        public String FacilityNamePashto { get; set; }
        public String Location { get; set; }
        public String LocationDari { get; set; }
        public String LocationPashto { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? DateEstablished { get; set; }
        public String Implementer { get; set; }
        public Int32? FacilityTypeId { get; set; }
        public String IsActive { get; set; }
    }

}
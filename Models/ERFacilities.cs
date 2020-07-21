using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class ERFacilities
    {
        public ERFacilities()
        {
            Ernmr = new HashSet<Ernmr>();
        }
        [Display(Name ="FacilityID")]
        [Range(1,35000,ErrorMessage ="Enter ER Facility ID between 1 and 35000")]
        public Int32 FacilityId { get; set; }
        [Display(Name = "District")]
        [Required(ErrorMessage="District is required.")]
        public string DistCode { get; set; }
        [Display(Name = "Province")]
        [Required(ErrorMessage = "Province is required.")]
        public string ProvCode { get; set; }
        [Display(Name = "Facility Name (Eng)")]
        [Required(ErrorMessage = "Facility Name is required.")]
        public string FacilityName { get; set; }
        [Display(Name = "Facility Name (Dari)")]
        [Required(ErrorMessage = "Facility Name in Dari is required.")]
        public string FacilityNameDari { get; set; }
        [Required(ErrorMessage = "Facility Name in Pashto is required.")]
        [Display(Name = "Facilit Name (Pashto)")]
        public string FacilityNamePashto { get; set; }
        [Display(Name = "Location (Eng)")]
        public string Location { get; set; }
        [Display(Name = "Location (Dari)")]
        public string LocationDari { get; set; }
        [Display(Name = "Location (Pashto)")]
        public string LocationPashto { get; set; }
        [Display(Name = "Facility Type")]
        [Required(ErrorMessage = "Facility Type is required.")]
        [Range(1,int.MaxValue,ErrorMessage ="The provided type is not valid")]
        public int? FacilityType { get; set; }
        [Display(Name ="LAT")]
        public double? Lat { get; set; }
        [Display(Name = "LON")]
        public double? Lon { get; set; }
        public String Status { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name ="Establishment Date")]
        public DateTime? DateEstablished { get; set; }
        [Required]        
        public string Implementer { get; set; }
        public DateTime? DateClosed { get; set; }
        public string FacilityFull { 
        get{
            return FacilityId + "-" + FacilityName +"-"+Implementer;
        }
        }
        [ForeignKey("FacilityType")]
        public virtual FacilityTypes FacilityTypeNavigation { get; set; }
        [ForeignKey("DistCode")]
        public virtual Districts DistNavigation { get; set; }
        public virtual ICollection<Ernmr> Ernmr { get; set; }
    }
}

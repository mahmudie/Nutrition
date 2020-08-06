using DataSystem.Models.GLM;
using DataSystem.Models.HP;
using DataSystem.Models.SCM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class FacilityInfo
    {
        public FacilityInfo()
        {
            Nmr = new HashSet<Nmr>();
            scmHFRequest = new HashSet<scmHFRequest>();
        }
        [Display(Name ="ID")]
        [Range(1,int.MaxValue,ErrorMessage ="Enter A valid id.")]
        public int FacilityId { get; set; }
        [Display(Name = "District")]
        [Required(ErrorMessage="District is required.")]
        public string DistCode { get; set; }
        public string ViliCode { get; set; }
        [Display(Name = "Facility Name")]
        [Required(ErrorMessage = "Facility Name is required.")]
        public string FacilityName { get; set; }
        [Display(Name = "Name Dari")]
        public string FacilityNameDari { get; set; }
        [Display(Name = "Name Pashtoo")]
        public string FacilityNamePashto { get; set; }
        public string Location { get; set; }
        [Display(Name = "Location Dari")]
        public string LocationDari { get; set; }
        [Display(Name = "Location Pashto")]
        public string LocationPashto { get; set; }
        [Display(Name = "Facility Type")]
        [Required(ErrorMessage = "Facility Type is required.")]
        public int? FacilityType { get; set; }

        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public string ActiveStatus { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateEstablished { get; set; }
        public double? Gpslattitude { get; set; }
        public double? Gpslongtitude { get; set; }
        [Required]        
        public string Implementer { get; set; }
        
        public string FacilityFull { 
        get{
            return FacilityId + "-" + FacilityName +"-"+Implementer;
        }
        }
        public string SubImplementer { get; set; }
        [ForeignKey("FacilityType")]
        public virtual FacilityTypes FacilityTypeNavigation { get; set; }
        [ForeignKey("DistCode")]
        public virtual Districts DistNavigation { get; set; }
        public virtual ICollection<Nmr> Nmr { get; set; }
        public virtual ICollection<scmHFRequest> scmHFRequest { get; set; }
    }
}

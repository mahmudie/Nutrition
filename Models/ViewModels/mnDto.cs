using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public partial class mnDto
	{  
        public int? FacilityId { get; set; }
        public string FacilityType{get;set;}
        public string ProvName{get;set;}
        [Range(1390,1500, ErrorMessage = "Enter a valid Year")]
        public int Year { get; set; }
        public int Month { get; set; }
        public int? Children5 { get; set; }
        public int? Children24 { get; set; }
        public int? Women { get; set; }
        public int? Lactating { get; set; }
        public string DistCode { get; set; }
        public string ProvCode{get;set;}
        public string Province { get; set; }
        public string District { get; set; }
        public string Facility { get; set; }

            
        } 
}     
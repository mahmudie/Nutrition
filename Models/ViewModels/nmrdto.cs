using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public partial class nmrdto
	{   public string Nmrid { get; set; }
        [Display(Name="Facility")]
        [Range(1, int.MaxValue, ErrorMessage = "facility ID is not valid.")]
        public int FacilityId { get; set; }
        public string FacilityName{get;set;}
        public string FacilityType{get;set;}
        public string ProvName{get;set;}
        [Range(1390,1500, ErrorMessage = "Enter a valid Year")]
        public int Year { get; set; }
        [Range(1,12,ErrorMessage ="Enter a valid month")]
        public int Month { get; set; }
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        public int? Odema { get; set; }
        public int? Z3score { get; set; }
        public int? Muac115 { get; set; }
        public int? cured { get; set; }
        public int? death { get; set; }
        public int? defaulter { get; set; }
        public int? male { get; set; }
        public int? female { get; set; }
        public string DistCode { get; set; }
        public string ProvCode{get;set;}
        public string Province { get; set; }
        public string District { get; set; }
        public string Facility { get; set; }
        public string Group { get; set; }

            
        } 
}     
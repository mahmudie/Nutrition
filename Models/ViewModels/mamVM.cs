using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public partial class mamVM
	{
		 [Range(1, int.MaxValue, ErrorMessage = "Invalid number")]
        public int Mamid { get; set; }
        [Required]
        public string Nmrid { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Totalbegin { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Zscore23 { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Muac12 { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Muac23 { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? ReferIn { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Absents { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Cured { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Deaths { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Defaulters { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Transfers { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? NonCured { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? TMale { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? TFemale { get; set; }
        public string UserName { get; set; }
        public string AgeGroup{get;set;}
	}
}
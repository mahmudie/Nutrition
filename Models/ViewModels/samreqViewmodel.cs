
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DataSystem.Models.ViewModels
{
    public class SamreqVm
    {
        public string FacType { get; set; }
		[Range(1, int.MaxValue, ErrorMessage = "Invalid number")]
		public int? Totalbegin { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
		public int? Odema { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
		public int? Z3score { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
		public int? Muac115 { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
		public int? Fromscotp { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
		public int? Fromsfp { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
		public int? Defaultreturn { get; set; }
		[Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        
        public string AgeGroup{get;set;}


    }
}

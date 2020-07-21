
using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public class NmrVm
    {
        public string Nmrid { get; set; }
        public int FacilityId {get;set;}
        public string FacilityName { get; set; }
        public string Province { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int mYear { get; set; }
        public int mMonth { get; set; }
        public int FacilityTypes { get; set; }
        public string Implementer { get; set; }
        public DateTime? OpeningDate { get; set; }
        public string PreparedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string StatusDescription { get; set; }
        public string HfStatus { get; set; }
        public int? stat { get; set; }
        public int? hfstat { get; set; }
        public string message{get;set;}
        public string TypeAbbrv {get;set;}
        public string username { get; set; }

    }
}

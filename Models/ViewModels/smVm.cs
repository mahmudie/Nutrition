
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DataSystem.Models.ViewModels
{
    public class smVm
    {
        public long Rid{get;set;}
        public string ProvCode { get; set; }
        [Required]
        public string ImpCode { get; set; }
        [Required]
        public int Year{ get; set; }
        [Required]
        public short Month{ get; set; }
        public short? YearFrom { get; set; }
        public short? MonthFrom { get; set; }
        public short? YearTo { get; set; }
        public short? MonthTo { get; set; }        

        public short Ph { get; set; }
        public short Dh { get; set; }
        public short Chc { get; set; }
        public short Shc { get; set; }
        public short Mht { get; set; }
        public short Bhc { get; set; }
        public string ReqBy { get; set; }
        public string item{get;set;}
        public DateTime UpdateDate { get; set; }


    }
}

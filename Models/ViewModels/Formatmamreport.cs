using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models.ViewModels
{
    [Table("Format_mam_report")]
    public class Formatmamreport
    {
        [Key]
        public string NMRID {get;set;}
        public int FacilityId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string FacilityName { get; set; }
        public string ProvCode { get; set; }
        public string Province { get; set; }
        public string DistCode { get; set; }
        public string District { get; set; }
        public string TypeAbbrv { get; set; }
        public int Absents { get; set; }
        public int Cured { get; set; }
        public int Deaths { get; set; }
        public int Defaulters { get; set; }
        public int MUAC12 { get; set; }
        public int  MUAC23 { get; set; }
        public int NonCured { get; set; }
        public int ReferIn  { get; set; }
        public int tFemale { get; set; }
        public int tMale { get; set; }
        public int totalbegin { get; set; }
        public int Transfers { get; set; }
        public int Zscore23 { get; set; }
        public double SFP_ALS { get; set; }
        public double SFP_AWG { get; set; }
        public string Implementer { get; set; }
        public int SFPID { get; set; }
        public int Time { get; set; }

    }
}
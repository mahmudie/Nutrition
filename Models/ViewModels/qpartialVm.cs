using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.ViewModels
{
    public partial class qpartialVm
    {
        public double? SfpAls { get; set; }
        public double? SfpAwg { get; set; }
        public double? IalsKwashiorkor { get; set; }
        public double? IalsMarasmus { get; set; }
        public double? IawgKwashiorkor { get; set; }
        public double? IawgMarasmus { get; set; }
        public double? OalsKwashiorkor { get; set; }
        public double? OalsMarasmus { get; set; }
        public double? OawgKwashiorkor { get; set; }
        public double? OawgMarasmus { get; set; }
        public int Bnaqid { get; set; }
        public string Nmrid { get; set; }
        public int? IpdRutfstockOutWeeks { get; set; }
        public int? IpdAdmissionsByChws { get; set; }
        public int? OpdRutfstockOutWeeks { get; set; }
        public int? OpdAdmissionsByChws { get; set; }
        public int? MamRusfstockoutWeeks { get; set; }
        public int? MamAddminsionByChws { get; set; }
        public int? NoHealthWorkers { get; set; }
        public int? ChwstrainedScreening { get; set; }
        public int? GirlsScreened { get; set; }
        public int? BoysScreened { get; set; }
        public int? Plwreported { get; set; }
        public string UserName{get;set;}
    }
}

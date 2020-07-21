namespace DataSystem.Models.ViewModels
{
    public class opdPerf
    {
        public string Province { get; set; }
        public int? Admissions { get; set; }
        public int? Cured { get; set; }
        public int? Death { get; set; }
        public int? Defaulter { get; set; }
        public int? Discharge { get; set; }
        public decimal? percentCured { get; set; }
        public decimal? percentDeath { get; set; }
        public decimal? percentDefault { get; set; }
    }
}
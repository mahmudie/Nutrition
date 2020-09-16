
namespace DataSystem.Models.ViewModels.PivotTable
{
    public class IndicatorsFilter
    {
        public string Viewtype {get;set;}
        public int Province { get; set; }
        public int DistCode { get; set; }
        public int Period { get; set; }
        public int Calendar { get; set; }
        public int Facility { get; set; }
        public string Implementer { get; set; }
    }
}
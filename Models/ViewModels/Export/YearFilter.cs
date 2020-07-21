using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models.ViewModels.Export
{
    [Table("yFilter")]
    public class YearFilter
    {
        public int Facility {get;set;}
        public int YearFrom {get;set;}
        public int YearTo {get;set;}
        public string Year2 {get;set;}
    }

    [Table("vProvince")]
    public class ProvinceFilter
    {
        public string Implementer {get;set;}
        public string ProvCode {get;set;}
        public string ProvName {get;set;}
    }

   [Table("vImplementer")]
    public class ImpFilter    {
        public string ImpCode {get;set;}
        public string Implementer {get;set;}
    }

    [Table("Yearlist")]
    public class Yearlist
    {
        [Key]
        public int YearId { get; set; }
        public int YearName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.ViewModels
{
    [Table("vmamavail")]
    public class vmamavail
    {
        [Key]
        public string NMRID { get; set; }
        public string FacilityType { get; set; }
        public string FacilityName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Implementer { get; set; }
        public string ProvCode { get; set; }

    }
}

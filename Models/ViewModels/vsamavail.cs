using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.ViewModels
{
    [Table("vsamavail")]
    public class vsamavail
    {
        [Key]
        public string NMRID { get; set; }
        public string FacilityType { get; set; }
        public int FacilityId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Implementer { get; set; }
        public string ProvCode { get; set; }

    }
}

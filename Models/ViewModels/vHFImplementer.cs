using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.ViewModels
{
    [Table("vHFImplementer")]
    public class vHFImplementer
    {
        [Key]
        public int FacilityId { get; set; }
        public string ImpCode { get; set; }
        public string Implementer { get; set; }
    }

    [Table("vImplementer")]
    public class vImplementer
    {
        public string ImpCode { get; set; }
        public string Implementer { get; set; }
    }

    [Table("vDistImplementers")]
    public class vDistImplementers
    {
        [Key]
        public string DistCode { get; set; }
        public string ImpCode { get; set; }
        public string Implementer { get; set; }
    }
    [Table("vProvImplementers")]
    public class vProvImplementers
    {
        [Key]
        public string ProvCode { get; set; }
        public string ImpCode { get; set; }
        public string Implementer { get; set; }
    }
}

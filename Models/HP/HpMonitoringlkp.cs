using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataSystem.Models.HP
{
    public class HpMonitoringlkp
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Section { get; set; }
        [Required]
        public string PartCode { get; set; }
        [Required]
        public string Questionname { get; set; }
        public string VerificationSource { get; set; }
        public string PossibleReponse { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }

    }

}

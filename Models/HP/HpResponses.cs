using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataSystem.Models.HP
{
    public class HpResponses
    {
        [Key]
        public int ResponseId { get; set; }
        [Required]
        public string ResponseName { get; set; }
        public bool IsActive { get; set; }
    }

}

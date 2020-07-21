using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmWasteTypes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Waste Type Name")]
        public string name { get; set; }

    }
}

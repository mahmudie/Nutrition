using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class FieldOptionViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public string Caption { get; set; }

        [Required]
        public long FieldId { get; set; }
    }
}

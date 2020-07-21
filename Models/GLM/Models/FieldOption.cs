using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class FieldOption
    {
        [Key]
        public long Id { get; set; }

        public string Caption { get; set; }

        public string Value { get; set; }

        [ForeignKey("Field")]
        public long FieldId { get; set; }
        public Field Field { get; set; }

    }
}

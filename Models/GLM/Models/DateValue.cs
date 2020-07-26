using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class DateValue
    {
        public long FieldId { get; set; }
        public string ReportId { get; set; }
        public DateTime? Data { get; set; } = null;
        public Field Field { get; set; }
    }
}

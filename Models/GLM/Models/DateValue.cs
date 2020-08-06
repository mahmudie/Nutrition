using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class DateValue
    {
        [ForeignKey("Field")]
        public long FieldId { get; set; }
        public Field Field { get; set; }

        [ForeignKey("Report")]
        public string ReportId { get; set; }
        public Report Report { get; set; }

        public DateTime? Data { get; set; } = null;
        
    }
}

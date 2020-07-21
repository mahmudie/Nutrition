using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.GLM.Models
{
    public class FieldValue
    {
        public long FieldId { get; set; }
        public String ReportId { get; set; }

        public string Data { get; set; } = null;
    }
}

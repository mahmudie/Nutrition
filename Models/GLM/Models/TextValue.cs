using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class TextValue
    {
        public long FieldId { get; set; }
        public string ReportId { get; set; }
        public string Data { get; set; } = null;
    }
}

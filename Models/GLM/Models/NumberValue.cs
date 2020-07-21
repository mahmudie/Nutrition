using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class NumberValue
    {
        public long FieldId { get; set; }
        public string ReportId { get; set; }
        public long? Data { get; set; } = null;
    }
}

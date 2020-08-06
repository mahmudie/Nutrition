using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    [Table("vDateValues")]
    public class VDateValue
    {
        public long FieldId { get; set; }

        
        public string ReportsViewId { get; set; }
        //public ReportsView Report { get; set; }

        public DateTime? Data { get; set; }
        public bool IsExpiryDate { get; set; }
        public int? ExpiryWarningPeriod { get; set; }
    }
}

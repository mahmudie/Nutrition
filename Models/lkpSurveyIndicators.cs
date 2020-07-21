using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class lkpSurveyIndicators
    {
        [Key]
        public int indicatorId { get; set; }
        public string indicatorName { get; set; }
        public string originalIndicatorName { get; set; }
    }
}

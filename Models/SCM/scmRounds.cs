using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRounds
    {

        [Key]
        public int RoundId { get; set; }
        [Required()]
        public string RoundCode { get; set; }
        public string RoundDescription { get; set; }
        [Required]
        public DateTime PeriodFrom { get; set; }
        [Required()]
        public DateTime PeriodTo { get; set; }
        public bool? IsActive { get; set; }
        public int YearFrom { get; set; }
        public int MonthFrom { get; set; }
        public int YearTo { get; set; }
        public int MonthTo { get; set; }
        public int RequesttypeId { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
        public DateTime? UpdateDate { get; set; }
    }

    public class vscmRounds
    {

        [Key]
        public int roundId { get; set; }
        public string roundCode { get; set; }
        public string roundDescription { get; set; }
        public DateTime periodFrom { get; set; }
        public DateTime periodTo { get; set; }
    }
}

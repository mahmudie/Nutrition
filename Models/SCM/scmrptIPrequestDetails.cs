using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmrptIPrequestDetails
    {
        [Key]
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string RoundCode { get; set; }
        public DateTime? RequestDate { get; set; }
        public string IP { get; set; }
        public string UserName { get; set; }
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int EndYear { get; set; }
        public int EndMonth { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string RequestBy { get; set; }
        public string Province { get; set; }
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }
        public string Item { get; set; }
        public string Program { get; set; }
        public int? Children { get; set; }
        public int? BaseEst { get; set; }
        public int? Adjustment { get; set; }
        public string Adjustmentreason { get; set; }
        public double? ipbalance { get; set; }
        public double? Net { get; set; }
        public double? Buffer { get; set; }
        public double? BufferEst { get; set; }
        public double? TotalEstimation { get; set; }
        public bool? ApproveByPnd { get; set; }
        public bool? ApproveByUnicef { get; set; }
        public string CommentByPnd { get; set; }
        public string CommentByUnicef { get; set; }
        public string CommentByIp { get; set; }

    }
}

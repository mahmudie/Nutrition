using System;
using System.Collections.Generic;

namespace DataSystem.Models
{
    public partial class SamreqDetails
    {
        public long Id { get; set; }
        public long Rid { get; set; }
        public int SupplyId { get; set; }
        public string FormName { get; set; }
        public int? U6 { get; set; }
        public int? O6 { get; set; }
        public int? CurrentBalance { get; set; }
        public int? Adjustment { get; set; }
        public string AdjustmentComment { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UserName { get; set; }

        public virtual Samreq R { get; set; }
        public virtual TlkpSstock SId  { get; set; }

    }
}

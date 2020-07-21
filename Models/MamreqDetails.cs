using System;
using System.Collections.Generic;

namespace DataSystem.Models
{
    public partial class MamreqDetails
    {
        public long Id { get; set; }
        public long Rid { get; set; }
        public int SupplyId { get; set; }
        public string FormName { get; set; }
        public int? NoOfBenificiaries { get; set; }
        public int? CurrentBalance { get; set; }
        public int? Adjustment { get; set; }
        public string AdjustmentComment { get; set; }

        public virtual Mamreq R { get; set; }
        public virtual TlkpFstock SId  { get; set; }

    }
}

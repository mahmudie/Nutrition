using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models.Checklist_subs;

namespace DataSystem.Models
{
    public class lkpChecklist
    {
        public lkpChecklist()
        {
            ChkCGMs = new HashSet<ChkCGM>();
            ChkGMs = new HashSet<ChkGM>();
            ChkIYCFHFs = new HashSet<ChkIYCFHF>();
            ChkMNs = new HashSet<ChkMN>();
            ChkOPTs = new HashSet<ChkOPT>();
            ChkSAMHFs = new HashSet<ChkSAMHF>();
            ChkSFPs = new HashSet<ChkSFP>();
        }
        public int IntId { get; set; }
        public int OrderId { get; set; }
        public string Stringorder { get; set; }
        public string Description { get; set; }
        public string DescriptionDari { get; set; }
        public Boolean Active { get; set; }
        public string Type { get; set; }

        public virtual ICollection<ChkCGM> ChkCGMs { get; set; }
        public virtual ICollection<ChkGM> ChkGMs { get; set; }
        public virtual ICollection<ChkIYCFHF> ChkIYCFHFs { get; set; }
        public virtual ICollection<ChkMN> ChkMNs { get; set; }
        public virtual ICollection<ChkOPT> ChkOPTs { get; set; }
        public virtual ICollection<ChkSAMHF> ChkSAMHFs { get; set; }
        public virtual ICollection<ChkSFP> ChkSFPs { get; set; }
    }
}

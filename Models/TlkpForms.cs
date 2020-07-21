using System.Collections.Generic;

namespace DataSystem.Models
{
    public partial class TlkpForms
    {
        public TlkpForms()
        {
            TblFeedback = new HashSet<TblFeedback>();
        }

        public int FormId { get; set; }
        public string FormName { get; set; }

        public virtual ICollection<TblFeedback> TblFeedback { get; set; }
    }
}

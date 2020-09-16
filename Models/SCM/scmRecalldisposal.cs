using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRecalldisposal
    {
        [Key]
        public int id { get; set; }
        public int ipdistributionId { get; set; }
        public int whId { get; set; }
        public int wasteId { get; set; }
        public DateTime? dateOfRecall { get; set; }
        public DateTime? dateOfDisposal { get; set; }
        public Double? quantity { get; set; }
        public String placeDisposal { get; set; }
        public String remarks { get; set; }
        public int tenantId { get; set; }
        public String userName { get; set; }
        public DateTime? updateDate { get; set; }
        public string Attachment { get; set; }
    }
}

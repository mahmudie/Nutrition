using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DataSystem.Models.HP
{
    public class HpRecommendations
    {
        [Key]
        public int id { get; set; }
        public int hpmId { get; set; }
        [Required]
        public int monitoringId { get; set; }
        public string keyFindings { get; set; }
        public string responsiblePersonUnit { get; set; }
        public string contributingPersonUnit { get; set; }
        public DateTime? deadline { get; set; }
        public string recommendationStatus { get; set; }
        public DateTime? dateOfCompletion { get; set; }
        public string remarks { get; set; }
        public string userName { get; set; }
        public int tenantId { get; set; }
        public DateTime updateDate { get; set; }
    }

}

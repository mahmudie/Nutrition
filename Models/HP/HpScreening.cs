using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataSystem.Models.HP
{
    public class HpScreening
    {
        [Key]
        public int id { get; set; }
        public int hpmId { get; set; }
        public int monitoringId { get; set; }
        public int responseId { get; set; }
        public string remarks { get; set; }
        [Required]
        public string userName { get; set; }
        [Required]
        public int tenantId { get; set; }
        [Required]
        public DateTime updateDate { get; set; }
    }

}

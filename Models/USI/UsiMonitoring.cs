using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.USI
{
    public class UsiMonitoring
    {
        [Key]
        public int usiId { get; set; }
        [Required]
        public int year { get; set; }
        [Required]
        public int month { get; set; }
        [Required]
        public string provinceId { get; set; }
        public string userName { get; set; }
        public int tenantId { get; set; }
        public DateTime updateDate { get; set; }
    }

    public class vUsiMonitoring
    {
        [Key]
        public int UsiId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string ProvinceId { get; set; }
        public string UserName { get; set; }
        public string Province { get; set; }
    }
}

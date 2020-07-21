using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmRequest
    {
        [Key]
        public int requestId { get; set; }
        [Required]
        [Display(Name ="Request Date")]
        public DateTime requestDate { get; set; }
        [Required]
        [Display(Name = "Implementer")]
        public int impId { get; set; }
        [Required]
        [Display(Name = "Province")]
        public string provinceId { get; set; }
        [Required]
        [Display(Name = "Request Period")]
        public int requestPeriod { get; set; }

        [Display(Name = "Type of Request")]
        public string typeOfRequest{ get; set; }
        [Display(Name = "Year of Request")]
        public int yearOfRequest { get; set; } 

        [Display(Name = "Start Year")]
        public int startYear { get; set; }

        [Display(Name = "Start Month")]
        public int startMonth { get; set; }

        [Display(Name = "End Year")]
        public int endYear { get; set; }

        [Display(Name = "End Month")]
        public int endMonth { get; set; } 
        public int tenantId { get; set; } 
        public string userName { get; set; }
        public DateTime updateDate { get; set; }
        [Required]
        [Display(Name = "Requested By")]
        public string requestBy { get; set; }
        public int TimeStart { get; set; }
        public int TimeEnd { get; set; }
    }
}

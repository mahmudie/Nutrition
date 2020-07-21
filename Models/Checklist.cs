using DataSystem.Models.Checklist_subs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class Checklist
    {
        public Checklist()
        {
            ChkCGMs = new HashSet<ChkCGM>();
            ChkGMs = new HashSet<ChkGM>();
            ChkIYCFHFs = new HashSet<ChkIYCFHF>();
            ChkMNs = new HashSet<ChkMN>();
            ChkOPTs = new HashSet<ChkOPT>();
            ChkSAMHFs = new HashSet<ChkSAMHF>();
            ChkSFPs = new HashSet<ChkSFP>();
        }
        [Key]
        public int ChkId { get; set; }
        public string CKU { get; set; }
        [Display(Name = "Facility")]
        [Required]
        [Range(1, 35000, ErrorMessage = "Enter valid Facility")]
        public int FacilityId { get; set; }
        public int? FacilitytypeId { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Monitoring Date")]
        [Required(ErrorMessage ="Must enter a valid date")]
        public DateTime MonitoringDate { get; set; }
        public string SamConsistancy { get; set; }
        public string Implementer { get; set; }
        public bool Isprogramvisit { get; set; }
        public string DataCollector { get; set; }
        public int? A2typeofhwinterviewed { get; set; }

        public string UserName { get; set; }
        public int Tenant { get; set; }
        public int Designation { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        [ForeignKey("FacilityId")]
        public virtual FacilityInfo ChkFacilityNav { get; set; }
        [ForeignKey("FacilityType")]
        public virtual FacilityTypes ChkFacilityTypeNav { get; set; }

        public virtual ICollection<ChkCGM> ChkCGMs { get; set; }
        public virtual ICollection<ChkGM> ChkGMs { get; set; }
        public virtual ICollection<ChkIYCFHF> ChkIYCFHFs { get; set; }
        public virtual ICollection<ChkMN> ChkMNs { get; set; }
        public virtual ICollection<ChkOPT> ChkOPTs { get; set; }
        public virtual ICollection<ChkSAMHF> ChkSAMHFs { get; set; }
        public virtual ICollection<ChkSFP> ChkSFPs { get; set; }
    }
}

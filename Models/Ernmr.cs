using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class Ernmr
    {
        public Ernmr()
        {
            EmrImamServices = new HashSet<EmrImamServices>();
            EmrIndicators = new HashSet<EmrIndicators>();
        }
        [Key]
        public int ErnmrId { get; set; }
        [Display(Name = "Facility Id")]
        [Required]
        [Range(30000, 35000, ErrorMessage = "Enter ER Facility ID between 30000 and 35000")]
        public int FacilityId { get; set; }
        [Required]
        [Range(1398, 1500, ErrorMessage = "Enter a valid Year")]
        public int Year { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "Enter a valid month")]
        public int Month { get; set; }
        [Range(1, 3, ErrorMessage = "Enter a valid option")]
        public int? Biweekly { get; set; }
        [Required]
        public int mYear { get; set; }
        public int mMonth { get; set; }
        public int? FacilityType { get; set; }
        public string Implementer { get; set; }
        public string UserName { get; set; }
        public int Tenant { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }
        [Required]
        [Display(Name ="U5 Screened")]
        [Range(1, int.MaxValue, ErrorMessage = "You must enter the total sceening here")]
        public int U5Screened { get; set; }
        public virtual ICollection<EmrImamServices> EmrImamServices { get; set; }
        public virtual ICollection<EmrIndicators> EmrIndicators { get; set; }
        [ForeignKey("FacilityId")]
        public virtual ERFacilities ErFacilityNavigation { get; set; }
        [ForeignKey("FacilityType")]
        public virtual FacilityTypes ErFacilityTypeNavigation { get; set; }
    }
}

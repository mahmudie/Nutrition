using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class Nmr
    {
        public Nmr()
        {
            TblFeedback = new HashSet<TblFeedback>();
            TblIycf = new HashSet<TblIycf>();
            TblMam = new HashSet<TblMam>();
            TblMn = new HashSet<TblMn>();
            TblOtp = new HashSet<TblOtp>();
            TblOtptfu = new HashSet<TblOtptfu>();
            Feedback = new HashSet<Feedback>();

        }
        public string Nmrid { get; set; }
        [Display(Name="Facility")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "facility is not valid.")]
        public int FacilityId { get; set; }
        [Required]
        [Range(1390,1500, ErrorMessage = "Enter a valid Year")]
        public int Year { get; set; }
        [Required]
        [Range(1,12,ErrorMessage ="Enter a valid month")]
        public int Month { get; set; }
        [Required]
        public int mYear { get; set; }
        public int mMonth { get; set; }
        public int? FacilityType { get; set; }
        public string Implementer { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name="Created at")]
        public DateTime? OpeningDate { get; set; }
        [Display(Name="Prepared By")]
        public string PreparedBy { get; set; }
        [Range(0, 3000, ErrorMessage = "Invalid number")]
        public int? Flanumber { get; set; }
        public double? SfpAls { get; set; }
        public double? SfpAwg { get; set; }
        public double? IalsKwashiorkor { get; set; }
        public double? IalsMarasmus { get; set; }
        public double? IawgKwashiorkor { get; set; }
        public double? IawgMarasmus { get; set; }
        public double? OalsKwashiorkor { get; set; }
        public double? OalsMarasmus { get; set; }
        public double? OawgKwashiorkor { get; set; }
        public double? OawgMarasmus { get; set; }
        [Display(Name="Comment")]
        public string Commen { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name="Last Update")]
        public DateTime? UpdateDate { get; set; }
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ID is not valid.")]
        [Display(Name = "Health Facility Status")]
        public int? HfactiveStatusId { get; set; }
        public string message { get; set; }

        [Display(Name="Humanitarian")]
        public bool isHumanitarian{get;set;}
        public int Tenant { get; set; }

        public int? IpdRutfstockOutWeeks { get; set; }
        public int? IpdAdmissionsByChws { get; set; }
        public int? OpdRutfstockOutWeeks { get; set; }
        public int? OpdAdmissionsByChws { get; set; }
        public int? MamRusfstockoutWeeks { get; set; }
        public int? MamAddminsionByChws { get; set; }
        public int? GirlsScreened { get; set; }
        public int? BoysScreened { get; set; }
        public int? Plwreported { get; set; }
        public virtual ICollection<TblFeedback> TblFeedback { get; set; }
        public virtual ICollection<Feedback> Feedback { get; set; }
        public virtual ICollection<TblIycf> TblIycf { get; set; }
        public virtual ICollection<TblMam> TblMam { get; set; }
        public virtual ICollection<TblMn> TblMn { get; set; }
        public virtual ICollection<TblOtp> TblOtp { get; set; }
        public virtual ICollection<TblOtptfu> TblOtptfu { get; set; }
        public virtual FacilityInfo Facility { get; set; }
        public virtual LkpHfstatus HfactiveStatus { get; set; }
        public virtual TblkpStatus Status { get; set; }

    }
}

using DataSystem.Models.GLM;
using DataSystem.Models.HP;
using DataSystem.Models.SCM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class Implementers
    {
        public Implementers()
        {
            warehouses = new HashSet<scmWarehouses>();
        }
        [Display(Name = "Code")]
        public int ImpCode { get; set; }
        [Required]
        [Display(Name = " Acronym")]
        public string ImpAcronym { get; set; }
        [Display(Name = " Full Name")]
        public string ImpName { get; set; }
        [Display(Name = " Full Name Dari")]
        public string ImpNameDari { get; set; }
        [Display(Name = " Full Name Pashto")]
        public string ImpNamePashto { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }
        [Display(Name = "Afghanistan Address")]
        public string AfghanistanAddress { get; set; }
        [Display(Name = "Other Address")]
        public string OtherAddress { get; set; }
        [Display(Name = "Status")]
        public bool? ImpStatus { get; set; }
        public bool? IsScm { get; set; }


        [ForeignKey("ImpId")]
        public virtual ICollection<scmWarehouses> warehouses { get; set; }
        [ForeignKey("ImpId")]
        public virtual ICollection<Report> Reports { get; set; }

    }
}

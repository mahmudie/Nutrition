using DataSystem.Models.GLM;
using DataSystem.Models.HP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    [Table("Districts")]
    public partial class Districts
    {
        [Required]
        [Key]
        [Display(Name = "District code")]
        public string DistCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string DistName { get; set; }
        [Display(Name = "Name Dari")]
        public string DistNameDari { get; set; }
        [Display(Name = "Name Pashto")]
        public string DistNamePashto { get; set; }
        [Required]
        public string ProvCode { get; set; }

        public virtual Provinces ProvCodeNavigation { get; set; }
    }
}

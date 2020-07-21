using DataSystem.Models.GLM;
using DataSystem.Models.HP;
using DataSystem.Models.SCM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    public partial class Provinces
    {
        public Provinces()
        {
            Districts = new HashSet<Districts>();
            warehouses = new HashSet<scmWarehouses>();
        }
        [Required]
        public string ProvCode { get; set; }
        public int? AGHCHOCode { get; set; }
        public int? AIMSCode { get; set; }

        public DateTime? CreatedDate { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string ProvName { get; set; }
        [Required]
        [Display(Name = "Name Dari")]
        public string ProveNameDari { get; set; }
        [Required]
        [Display(Name = "Name Pashto")]
        public string ProveNamePashto { get; set; }

        public virtual ICollection<Districts> Districts { get; set; }
       

        [ForeignKey("ProvinceId")]
        public virtual ICollection<scmWarehouses> warehouses { get; set; }
    }
}

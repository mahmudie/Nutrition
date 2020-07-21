using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmWarehouses
    {
        public scmWarehouses()
        {
            scmStocks = new HashSet<scmStocks>();
        }
        [Key]
        public int WhId { get; set; }
        [Required()]
        public int RegionId { get; set; }
        public virtual scmRegions RegionsNav { get; set; }
        [Required]
        [Display(Name = "Province")]
        public string ProvinceId { get; set; }
        public virtual Provinces ProvincesNav { get; set; }
        [Required]
        [Display(Name = "Implementer")]
        public int ImpId { get; set; }
        public virtual Implementers ImplementerNav { get; set; }
        [Required]
        [Display(Name = "Warehouse Location")]
        public string Location { get; set; }
        public int LevelId { get; set; }
        public Boolean Active { get; set; }
        public String WarehouseName { get; set; }
        [ForeignKey("WhId")]
        public virtual ICollection<scmStocks> scmStocks { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class LkpCategory
    {
        public LkpCategory()
        {
            LkpDisaggregations = new HashSet<LkpDisaggregation>();
        }
        [Display(Name ="CategoryId")]
        public int CategoryId { get; set; }
        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }

        public virtual ICollection<LkpDisaggregation> LkpDisaggregations { get; set; }
    }

    public partial class LkpCategoryvm
    {
        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }
        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }
        public IFormFile formFile { get; set; }
    }
}

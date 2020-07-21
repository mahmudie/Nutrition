using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class tlkpbiweekly
    {
        [Key]
        [Display(Name ="Id")]
        public int Id { get; set; }
        [Display(Name = "Item")]
        public string Name { get; set; }
    }
}

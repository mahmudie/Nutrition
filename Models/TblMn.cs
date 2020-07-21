using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class TblMn
    {
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int Mnid { get; set; }
        [Required]
        public string Nmrid { get; set; }
        [Display(Name="Children U2 Male")]
        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int? chu2m { get; set; }
         [Display(Name="Children U2 Female")]
        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int? chu2f { get; set; }
         [Display(Name="Referred by CHWs")]
        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int? refbyCHW { get; set; }
        public string Remarks { get; set; }
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual TlkpMn Mn { get; set; }
        public virtual Nmr Nmr { get; set; }
    }
}

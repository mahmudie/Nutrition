using System;
using System.ComponentModel.DataAnnotations;


namespace DataSystem.Models
{
    public class TblmnDto
    {
        public int Mnid { get; set; }
        public string Nmrid { get; set; }
        [Display(Name = "Children U2 Male")]
        public int? chu2m { get; set; }
        [Display(Name = "Children U2 Female")]
        public int? chu2f { get; set; }
        [Display(Name = "Remarks ")]
        public string Remarks { get; set; }
        [Display(Name = "Updated at")]
        public DateTime? UpdateDate { get; set; }
        public int? UserName { get; set; }
        [Display(Name = "Referred by CHWs ")]
        public int? refbyCHW { get; set; }
        public string Mnitems { get; set; }
        public bool Active { get; set; }

    }
}

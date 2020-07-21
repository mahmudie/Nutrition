using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models.ViewModels
{
    [Table("mam_common")]
    public class mamcommon
    {
        [Key]
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string ProvCode { get; set; }
    }
}
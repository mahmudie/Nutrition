using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models.SCM
{
    public class scmRequesttype
    {
        [Key]
        public int RequesttypeId { get; set; }
        public string Requesttypename { get; set; }
    }
}

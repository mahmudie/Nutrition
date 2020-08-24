using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{

    [Table("Videoguides")]
    public class Videoguides
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string module { get; set; }
        public string link { get; set; }
        public bool? userreads { get; set; }
    }
}

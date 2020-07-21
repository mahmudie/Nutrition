using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmmailgroup
    {
        [Key]
        public int id { get; set; }
        public string ccemails { get; set; }
        public string toemails { get; set; }
        public string bccemails { get; set; }
        public bool isactive { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class Apistore
    {
        [Key]
        public int id { get; set; }
        public string apiurl { get; set; }
        public string filtervalue { get; set; }
        public Boolean? isActive { get; set; }
        public DateTime? lastDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.Analysis
{
    public class Procedureupdatestat
    {
        [Key]
        public int Id { get; set; }
        public DateTime Updatedate { get; set; }
    }
}

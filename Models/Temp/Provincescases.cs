using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.Temp
{
    public class Provincescases
    {
        [Key]
        public int Id { get; set; }
        public string Province { get; set; }
        public int Year { get; set; }
        public int Visits { get; set; }
    }
}

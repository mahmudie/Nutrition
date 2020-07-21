using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class Lkpwhlevels
    {
        [Key]
        public int LevelId { get; set; }
        public string LevelDescriptoin { get; set; }
    }
}

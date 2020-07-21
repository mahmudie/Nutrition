using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class DataForm
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public ICollection<Section> Sections { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace DataSystem.Models
{
    public partial class provincemonthly
    {
        [Key]
        public string ProvId { get; set; }  
        public string Province { get; set; }
        public int Year { get; set; }
        public Int32 M1 { get; set; }
        public Int32 M2 { get; set; }
        public Int32 M3 { get; set; }
        public Int32 M4 { get; set; }
        public Int32 M5 { get; set; }
        public Int32 M6 { get; set; }
        public Int32 M7 { get; set; }
        public Int32 M8 { get; set; }
        public Int32 M9 { get; set; }
        public Int32 M10 { get; set; }
        public Int32 M11 { get; set; }
        public Int32 M12 { get; set; }

    }
}
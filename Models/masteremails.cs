using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class masteremails
    {
        [Key]
        public int Id { get; set; }
        public string emailaccount { get; set; }
        public string smtp { get; set; }
        public bool ssl { get; set; }
        public bool issender { get; set; }
        public int port { get; set; }
        public bool isactive { get; set; }
    }
}

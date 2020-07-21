using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmContacts
    {
        [Key]
        public int id { get; set; }
        public int requestId { get; set; }
        public string position { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string email { get; set; }
        public string userName { get; set; }
        public DateTime updateDate { get; set; }
    }
}

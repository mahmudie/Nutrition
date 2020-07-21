using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    [Table("scmdashrequest_ip")]
    public class scmdashrequestip
    {
        [Key]
        public string id { get; set; }
        public int requestId { get; set; }
        public string type { get; set; }
        public int yearOfRequest { get; set; }
        public int startYear { get; set; }
        public int startMonth { get; set; }
        public int endYear { get; set; }
        public int endMonth { get; set; }
        public string ip { get; set; }
        public string program { get; set; }
        public string item { get; set; }
        public int children { get; set; }
        public int ballance { get; set; }
        public int newStock { get; set; }
        public int adjustment { get; set; }
        public int total { get; set; }
    }
}

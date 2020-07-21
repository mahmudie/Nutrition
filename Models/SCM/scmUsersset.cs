using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmUsersset
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
        public string ImpAcronym { get; set; }
        public string UserName { get; set; }
        public int IsUnicefPnd { get; set; }
        public string RoleName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    [Table("scmdashsubmission")]
    public class scmdashsubmission
    {
        [Key]
        public int Id { get; set; }
        public string Quarter { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime DeadlineDate { get; set; }
        public string Emailmessage { get; set; }
        public bool Completed { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime? Datecompleted { get; set; }
        public string ImpAcronym { get; set; }
        public string ProvName { get; set; }
        public int email { get; set; }
    }
}

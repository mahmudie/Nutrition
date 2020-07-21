using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmEstsubmission
    {
        [Key]
        public int id { get; set; }
        public int roundId { get; set; }
        public DateTime startDate { get; set; }
        public DateTime deadlineDate { get; set; }
        public bool? completed { get; set; }
        public DateTime? datecompleted { get; set; }
        public string userName { get; set; }
        public DateTime updateDate { get; set; }
        public int tenantId { get; set; }
        public string emailmessage { get; set; }
    }

    public class scmEstNotification
    {
        [Key]
        public int id { get; set; }
        public int submissionId { get; set; }
        //public int notifyId { get; set; }
        public int impId { get; set; }
        public string provinceId { get; set; }
        public int email { get; set; }
        public int sms { get; set; }
        public int inappnotification { get; set; }
        public DateTime? datesubmitted { get; set; }
        public bool? isontime { get; set; }
        public bool? completed { get; set; }

    }
    
    public class vmEstNotification
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Province { get; set; }
        public string Implementer { get; set; }
        public string RoundDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DeadlineDate { get; set; }
        public string Emailmessage { get; set; }

    }

}

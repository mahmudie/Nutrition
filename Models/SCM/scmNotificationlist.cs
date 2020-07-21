using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmNotificationlist
    {
        public scmNotificationlist()
        {
            scmEstNotification = new HashSet<scmEstNotification>();
        }
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public int TenantId { get; set; }
        public string ProvinceId { get; set; }
        public int ImpId { get; set; }
        public bool? IsActive { get; set; }

        [ForeignKey("notifyId")]
        public virtual ICollection<scmEstNotification> scmEstNotification { get; set; }
    }
}

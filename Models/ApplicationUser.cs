using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataSystem.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public bool Active { get; set; }
        public int TenantId{get;set;}
        public int? Pnd { get; set; }
        public int? Unicef { get; set; }
        public int? ImpId { get; set; }
        [NotMapped]
        public IList<string> RoleNames { get; set; }
        [NotMapped]
        public IList<string> Tenants {get;set;}
    }
}

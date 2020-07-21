
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace DataSystem.Data
{
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>() .Ignore(c => c.TwoFactorEnabled)
                                           .Ignore(c=>c.PhoneNumberConfirmed);
        }
    }
}

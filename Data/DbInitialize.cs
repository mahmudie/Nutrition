using DataSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq;

namespace DataSystem.Data
{
    public static class DbInitializer
    {
        public async static void Initialize(WebNutContext db, RoleManager<IdentityRole> _roleManager, UserManager<ApplicationUser> _userManager)
        {
            db.Database.EnsureCreated();

            //Look for any students.
            if (db.TblkpStatus.Any() || db.LkpHfstatus.Any() || _roleManager.Roles.Any() || _userManager.Users.Any())
                {
                    return;   // DB has been seeded
                }

            var stats = new TblkpStatus[]
            {
            new TblkpStatus{StatusId=1,StatusDescription="Insert"},
            new TblkpStatus{StatusId=2,StatusDescription="Update"},
            new TblkpStatus{StatusId=3,StatusDescription="Accept"},
            new TblkpStatus{StatusId=4,StatusDescription="Reject"},
            };
            foreach (TblkpStatus s in stats)
            {
                db.TblkpStatus.Add(s);
            }
            db.SaveChanges();

            var hfstats = new LkpHfstatus[]
            {
            new LkpHfstatus{HfactiveStatusId=1,HfstatusDescription="Open"},
            new LkpHfstatus{HfactiveStatusId=2,HfstatusDescription="Close"},
            new LkpHfstatus{HfactiveStatusId=3,HfstatusDescription="Closed-no supply"},
            new LkpHfstatus{HfactiveStatusId=4,HfstatusDescription="Closed-Security"},
            };
            foreach (LkpHfstatus s in hfstats)
            {
                db.LkpHfstatus.Add(s);
            }
            db.SaveChanges();

            var AgeGroups = new TlkpOtptfu[]
            {
            new TlkpOtptfu{Active=true,AgeGroup="<6 month"},
            new TlkpOtptfu{Active=true,AgeGroup="6-23 Months"},
            new TlkpOtptfu{Active=true,AgeGroup="24-59 Months"},
            new TlkpOtptfu{Active=true,AgeGroup="total"},
            };
            foreach (TlkpOtptfu s in AgeGroups)
            {
                db.TlkpOtptfu.Add(s);
            }
            db.SaveChanges();

            var MamAgeGroups = new TlkpSfp[]
            {
            new TlkpSfp{Active=true,AgeGroup="Children 6-23 months"},
            new TlkpSfp{Active=true,AgeGroup="Children 24-59 months"},
            new TlkpSfp{Active=true,AgeGroup="Lactating women"},
            new TlkpSfp{Active=true,AgeGroup="Pregnant women"},
            new TlkpSfp{Active=true,AgeGroup="total children"},
            new TlkpSfp{Active=true,AgeGroup="total women"},
            };
            foreach (TlkpSfp s in MamAgeGroups)
            {
                db.TlkpSfp.Add(s);
            }
            db.SaveChanges();

            var mnItems = new TlkpMn[]
            {
            new TlkpMn{Mnid=1,Active=true,Mnitems="Vit A Capsule  (200000,100000 or 50000IU)"},
            new TlkpMn{Mnid=2,Active=true,Mnitems="IFA (60mg Iron, 400Micogram Folic) Tab"},
            new TlkpMn{Mnid=3,Active=true,Mnitems="Ferrous Sulfate drop/syrop "},
            new TlkpMn{Mnid=4,Active=true,Mnitems="Ferrous Sulfate 200mg(base 60mg)"},
            new TlkpMn{Mnid=5,Active=true,Mnitems="Folic Acid 1 mg Tab"},
            new TlkpMn{Mnid=6,Active=true,Mnitems="Multiple Micronutrients Tablet"},
            new TlkpMn{Mnid=7,Active=true,Mnitems="Multiple Micronutrients Powder"},
            new TlkpMn{Mnid=8,Active=true,Mnitems="Zinc Tab or Syrup/Drop + ORS"},
            new TlkpMn{Mnid=9,Active=true,Mnitems="Mebendasol Tab"},
            };
            foreach (TlkpMn s in mnItems)
            
              db.TlkpMn.Add(s);
            
            db.SaveChanges();


            IdentityRole Role = new IdentityRole();
            Role.NormalizedName = "ADMINISTRATOR";
            Role.Name ="administrator";
             await _roleManager.CreateAsync(Role);
            IdentityRole Role2 = new IdentityRole();
            Role2.NormalizedName = "DATAENTRY";
            Role2.Name = "dataentry";
            await _roleManager.CreateAsync(Role2);
            IdentityRole Role3 = new IdentityRole();
            Role3.NormalizedName = "GUEST";
            Role3.Name = "guest";
            IdentityRole Role4 = new IdentityRole();
            Role4.NormalizedName = "UNICEF";
            Role4.Name = "unicef";
            IdentityRole Role5 = new IdentityRole();
            Role5.NormalizedName = "PND";
            Role5.Name = "pnd";

            await _roleManager.CreateAsync(Role5);
            var user = new ApplicationUser
            {
                UserName = "admins",
                PhoneNumber ="99999999",
                FirstName = "admin",
                LastName = "admin",
                Email = "admin@admin.com",
                Position = "admin",
                Active = true,
                TenantId=1
            };
            var result = await _userManager.CreateAsync(user, "159*951-Aa");
            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("super_admin", "1"));                
                await _userManager.AddToRoleAsync(user,"administrator");
            }
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace DataSystem.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole>roleManager)
        {

            _roleManager = roleManager; 
        }
        public ActionResult Index()
        {
            var Roles = _roleManager.Roles;
            return View(Roles);
        }

        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IdentityRole Role)
        {
            Role.NormalizedName= Role.Name.ToUpper();
            Role.Name=Role.Name.ToLower();
            await _roleManager.CreateAsync(Role);
            return RedirectToAction("Index");
        }

       public ActionResult Edit()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<ActionResult> Edit(IdentityRole Role)
        {
            Role.NormalizedName= Role.Name.ToUpper();
             await _roleManager.UpdateAsync(Role);
            return RedirectToAction("Index");
        }
       public ActionResult Delete()
        {
            var Role = new IdentityRole();
            return View(Role);
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        public async Task<ActionResult> Delete(IdentityRole Role)
        {
             await _roleManager.DeleteAsync(Role);
            return RedirectToAction("Index");
        }

    }
}
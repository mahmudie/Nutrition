
using DataSystem.Controllers;
using DataSystem.Models;
using DataSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Components
{
    public class ScmRequeststatusWidget : ViewComponent
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
    public ScmRequeststatusWidget(WebNutContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var catdata = await _context.vscmRequeststatus.ToListAsync();
            return View(catdata);
        }
    }
}

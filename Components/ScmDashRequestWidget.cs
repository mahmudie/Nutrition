
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
    public class ScmDashRequestWidget : ViewComponent
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
    public ScmDashRequestWidget(WebNutContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int maxReq = _context.scmdashrequest.Max(m => m.RequestId);

            var catdata = _context.scmdashrequest.Where(m => m.RequestId.Equals(maxReq)).ToList();

            return View(catdata);
        }
    }
}

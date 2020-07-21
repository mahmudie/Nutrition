
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
    public class SurveyResultsWidget : ViewComponent
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public SurveyResultsWidget(WebNutContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var catdata = _context.LkpCategories.ToList();
            ViewBag.CatSurvSource = catdata;

            getSurvInfos.SurvId = id;
            if(getSurvInfos.SurvId == 0)
            {
                return View(0);
            }
            else
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.ViewModels.Export;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataSystem.Controllers
{
    public class ExportController:Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ExportController(WebNutContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

        public IActionResult Index()
        {
            List<ImpFilter> Implementers = new List<ImpFilter>();
            Implementers = (from imp in _context.ImpFilter orderby imp.ImpCode select imp) .ToList();
            Implementers.Insert(0, new ImpFilter { ImpCode = "0", Implementer = "select" });
            ViewData["ImpList"] = new SelectList(Implementers, "ImpCode", "Implementer");
            return View();
        }

        
        public JsonResult districts(string ProvCode)
        {
            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts where dist.ProvCode == ProvCode select dist).ToList();
            //Districts.Insert(0, new Districts { DistCode = "0", DistName = "select" });
            return Json(new SelectList(Districts, "DistCode", "DistName"));
        }
        public JsonResult facilities(string DistCode)
        {
            List<FacilityInfo> facilities = new List<FacilityInfo>();
            facilities = (from fac in _context.FacilityInfo where fac.DistCode == DistCode select fac).ToList();
            //facilities.Insert(0, new FacilityInfo { FacilityId = 0, FacilityName = "select" });
            return Json(new SelectList(facilities, "FacilityId", "FacilityFull"));
        }

        public JsonResult years(int Facility)
        {
            List<YearFilter> years = new List<YearFilter>();
            years = (from yr in _context.YearFilter where yr.Facility==Facility select yr).ToList();
            years.Insert(0, new YearFilter { YearFrom = 0, Year2 = "select" });
            return Json(new SelectList(years, "YearFrom", "Year2"));
        }

        public JsonResult provinces (string ImpCode)
        {
            List<ProvinceFilter> Provinces = new List<ProvinceFilter>();
            Provinces = (from prov in _context.ProvinceFilter where prov.Implementer==ImpCode select prov).ToList();
            Provinces.Insert(0, new ProvinceFilter { ProvCode = "0", ProvName = "select" });
            return Json(new SelectList(Provinces, "ProvCode", "ProvName"));
        }
    }
}
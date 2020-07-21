using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using DataTables.AspNet.Core;
using System.Collections.Generic;
using System;
using DataTables.AspNet.AspNetCore;
using Syncfusion.XlsIO;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using DataSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry,administrator,pnd,unicef")]
    public class ErnmrController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ErnmrController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FacilityInfo
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PageData(IDataTablesRequest request)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _context.Database.SetCommandTimeout(5000);
            var data = _context.Ernmr.AsNoTracking().Select(n=>new ErnmrVm
            {
                Ernmrid = n.ErnmrId,
                FacilityId=n.FacilityId,
                FacilityName =n.ErFacilityNavigation.FacilityName,
                FacilityType =n.ErFacilityTypeNavigation.TypeAbbrv,
                Implementer =n.Implementer,
                Year =n.Year,
                Month=n.Month,
                Biweekly=n.Biweekly,
                Screens =n.U5Screened,
                Province=n.ErFacilityNavigation.DistNavigation.ProvCodeNavigation.ProvName,
                District =n.ErFacilityNavigation.DistNavigation.DistName,
                UserName=n.UserName,
                TenantId=n.Tenant
                
            }).OrderByDescending(m=>m.FacilityId).ToList();

            if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.UserName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                data = data.Where(m => m.TenantId == user.TenantId).ToList();
            }

            List<ErnmrVm> filteredData;
            if (String.IsNullOrWhiteSpace(request.Search.Value))
            {
                filteredData = data;
            }
            else
            {

                int a;
                bool result = int.TryParse(request.Search.Value, out a);

                if (result)
                {
                    filteredData = data.Where(_item => _item.FacilityId == a).ToList();
                }

                else if (!result)
                {
                    string search = request.Search.Value.Trim();
                    filteredData = data.Where(_item => _item.FacilityName != null && _item.FacilityName.ToLower().Contains(search.ToLower())
                    || _item.FacilityType != null && _item.FacilityType.Contains(search)
                    || _item.Province != null && _item.Province.Contains(search)
                    || _item.Implementer != null && _item.Implementer.Contains(search)
                    || _item.District != null && _item.District.Contains(search)).ToList();
                }
                else
                {
                    filteredData = data;
                }
            }

            var dataPage = filteredData.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);
            return new DataTablesJsonResult(response, true);
        }

        [Authorize(Roles = "dataentry,administrator")]
        public JsonResult districts(string ProvCode)
        {
            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts where dist.ProvCode == ProvCode select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "0", DistName = "select" });
            return Json(new SelectList(Districts, "DistCode", "DistName"));
        }
        [Authorize(Roles = "dataentry,administrator")]
        public JsonResult facilities(string DistCode)
        {
            List<ERFacilities>facilities = new List<ERFacilities>();
            facilities = (from fac in _context.ERFacilities where fac.DistCode == DistCode && fac.Status == "Y" select fac).ToList();
            facilities.Insert(0, new ERFacilities { FacilityId = 0, FacilityName = "select" });
            return Json(new SelectList(facilities, "FacilityId", "FacilityFull"));
        }
        // GET: FacilityInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityInfo = await _context.ERFacilities.SingleOrDefaultAsync(m => m.FacilityId == id);
            if (facilityInfo == null)
            {
                return NotFound();
            }
            List<FacilityTypes> facilityTypes = new List<FacilityTypes>();
            facilityTypes = (from factype in _context.FacilityTypes select factype).ToList();
            facilityTypes.Insert(0, new FacilityTypes { FacTypeCode = 0, FacType = "select" });

            List<Implementers> implementers = new List<Implementers>();
            implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpCode = 0, ImpAcronym = "select" });

            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "", DistName = "select" });

            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "", ProvName = "select" });

            ViewData["fac"] = new SelectList(facilityTypes, "FacTypeCode", "FacType");
            ViewData["imps"] = new SelectList(implementers, "ImpAcronym", "ImpAcronym");
            ViewData["provinces"] = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewData["Districts"] = new SelectList(Districts, "DistCode", "DistName");
            return View(facilityInfo);
        }

        // GET: FacilityInfo/Create
        public IActionResult Create()
        {
            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });

            ViewData["ProvList"] = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewData["biweekly"] = new SelectList(_context.tlkpbiweekly, "Id", "Name");
            ViewData["FacilityId"] = new SelectList(_context.ERFacilities, "FacilityId", "FacilityName");

            return View();
        }


        // POST: FacilityInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ernmr ernmr)
        {

            if (ModelState.IsValid)
            {
                ERFacilities erfac = _context.ERFacilities.Where(m => m.FacilityId == ernmr.FacilityId).FirstOrDefault();
                ernmr.FacilityId = ernmr.FacilityId;
                ernmr.FacilityType = erfac.FacilityType;
                ernmr.Implementer = erfac.Implementer ;
                ernmr.Year = ernmr.Year;
                ernmr.Month = ernmr.Month;

                int getYear = ernmr.Year;
                int setYear = 0;
                int getMonth = ernmr.Month;
                int setMonth = 0;
                if(getMonth<10)
                {
                    setYear = getYear + 621;
                    setMonth = getMonth + 3;
                }
                else
                {
                    setYear = getYear + 622;
                    setMonth = getMonth - 9;
                }

                ernmr.mYear = setYear;
                ernmr.mMonth = setMonth;
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                ernmr.UserName = user.UserName;
                ernmr.UpdateDate = DateTime.Now;
                ernmr.Tenant = user.TenantId;

                _context.Add(ernmr);
                await _context.SaveChangesAsync();
                return RedirectToAction("ErIndicators","Ernmr",new { id = ernmr.ErnmrId });
            }

            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewData["biweekly"] = new SelectList(_context.tlkpbiweekly, "Id", "Name");
            ViewData["FacilityId"] = new SelectList(_context.ERFacilities, "FacilityId", "FacilityName");
            return View(ernmr);
        }

        // GET: FacilityInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ernmr = await _context.Ernmr.SingleOrDefaultAsync(m => m.ErnmrId == id);
            HttpContext.Session.SetString("myernmrid", id.ToString());

            if (ernmr == null)
            {
                return NotFound();
            }

            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewData["biweekly"] = new SelectList(_context.tlkpbiweekly, "Id", "Name");
            ViewData["FacilityId"] = new SelectList(_context.ERFacilities, "FacilityId", "FacilityFull");

            return View(ernmr);
        }

        [Authorize(Roles = "dataentry,administrator")]
        [HttpGet("/ernmr/fillHFID")]
        public IActionResult fillHFID(int id)
        {
            int myid = (int)HttpContext.Session.GetInt32("myernmrid");
            id = myid;
            var data = _context.Ernmr.Where(f => f.ErnmrId ==id).Select(d => new
            {
                ProvCode = d.ErFacilityNavigation.DistNavigation.ProvCode + '-' + d.ErFacilityNavigation.DistNavigation.ProvCodeNavigation.ProvName,
                DistCode = d.ErFacilityNavigation.DistCode + "-" + d.ErFacilityNavigation.DistNavigation.DistName,
                FacilityId = d.FacilityId + "-" + d.ErFacilityNavigation.FacilityName
            }).ToList();
            return Ok(data);
        }
        // POST: FacilityInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ernmr ernmr)
        {
            //if (id != ernmr.er)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                var item = await _context.Ernmr.Include(m => m.ErFacilityNavigation).SingleOrDefaultAsync(m => m.ErnmrId == id);
                var item2 = await _context.Ernmr.Include(m => m.ErFacilityNavigation).SingleOrDefaultAsync(m => m.ErnmrId == id);
                if (item == null)
                {
                    return NotFound();
                }

                if (item.UserName != User.Identity.Name)
                {
                    return BadRequest();
                }

                try
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    item.UpdateDate = DateTime.Now;
                    item.Month = ernmr.Month;
                    item.Year = ernmr.Year;
                    item.Implementer = item2.ErFacilityNavigation.Implementer;
                    item.FacilityType = item2.FacilityType;
                    item.U5Screened = item2.U5Screened;
                    item.Biweekly = item2.Biweekly;
                    item.UserName = user.UserName;
                    item.Tenant = user.TenantId;

                    int getYear = ernmr.Year;
                    int setYear = 0;
                    int getMonth = ernmr.Month;
                    int setMonth = 0;
                    if (getMonth < 10)
                    {
                        setYear = getYear + 621;
                        setMonth = getMonth + 3;
                    }
                    else
                    {
                        setYear = getYear + 622;
                        setMonth = getMonth - 9;
                    }

                    item.mYear = setYear;
                    item.mMonth = setMonth;

                    //_context.Update(ernmr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityInfoExists(ernmr.FacilityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Provinces, "ProvCode", "ProvName");
            ViewData["biweekly"] = new SelectList(_context.tlkpbiweekly, "Id", "Name");
            ViewData["FacilityId"] = new SelectList(_context.ERFacilities, "FacilityId", "FacilityFull");
            return View(ernmr);
        }

        // GET: FacilityInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ern = await _context.Ernmr.SingleOrDefaultAsync(m => m.ErnmrId == id);
            if (ern == null)
            {
                return NotFound();
            }
            ViewData["biweekly"] = new SelectList(_context.tlkpbiweekly, "Id", "Name");
            return View(ern);
        }

        // POST: FacilityInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ern = await _context.Ernmr.SingleOrDefaultAsync(m => m.ErnmrId == id);
            _context.Ernmr.Remove(ern);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ErIndicators(int? id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var nmr = await _context.Ernmr.SingleOrDefaultAsync(m => m.ErnmrId == id);
            if (nmr == null)
            {
                return NotFound();
            }

            //if (User.Identity.Name != nmr.UserName) { return Unauthorized(); }
            var model = new EmrIndicators();
            model.ErnmrId = nmr.ErnmrId;
            model.UserName = nmr.UserName;
            return View("Indicators", model);
        }

        public async Task<IActionResult> Screenings(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var nmr = await _context.Ernmr.SingleOrDefaultAsync(m => m.ErnmrId == id);
            if (nmr == null)
            {
                return NotFound();
            }

            //if (User.Identity.Name != nmr.UserName) { return Unauthorized(); }
            var model = new EmrImamServices();
            model.ErnmrId = nmr.ErnmrId;
            model.UserName = nmr.UserName;
            return View("Screens", model);
        }
        private bool FacilityInfoExists(int id)
        {
            return _context.FacilityInfo.Any(e => e.FacilityId == id);
        }
    }

}
using DataSystem.Models;
using DataSystem.Models.ViewModels;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry,administrator")]
    public class NmrController : Controller
    {
        private readonly WebNutContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public NmrController(WebNutContext context, ILoggerFactory loggerFactory, UserManager<ApplicationUser> userManager)
        {
            _logger = loggerFactory.CreateLogger<NmrController>();
            _context = context;
            _userManager = userManager;

        }

        [Authorize(Roles = "dataentry")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "administrator")]
        public PartialViewResult Feedback(string id)
        {
            var commsub = _context.Feedback.Where(c => c.Nmrid == id);
            ViewBag.CommitmentId = id;
            return PartialView("Feedback", commsub);
        }

        [Authorize(Roles = "dataentry")]
        public IActionResult PageData(IDataTablesRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            _context.Database.SetCommandTimeout(5000);
            var user = User.Identity.Name;
            var data = _context.Nmr.Where(m => m.UserName.Equals(user)).Select(m => new NmrVm()
            {
                Nmrid = m.Nmrid,
                FacilityName = m.Facility.FacilityFull,
                Implementer = m.Implementer,
                HfStatus = m.HfactiveStatus.HfstatusDescription,
                stat = m.StatusId,
                hfstat = m.HfactiveStatusId,
                Month = m.Month,
                Year = m.Year,
                OpeningDate = m.OpeningDate,
                UpdateDate = m.UpdateDate,
                message = m.message
            }).OrderByDescending(m => m.UpdateDate).Where(m => m.stat != null).ToList();
            List<NmrVm> filteredData;
            if (String.IsNullOrWhiteSpace(request.Search.Value))
            {
                filteredData = data;
            }
            else
            {
                int a;
                int y;
                bool result = int.TryParse(request.Search.Value, out a);
                if (!request.Search.Value.Contains("/"))
                {
                    if (result)
                    {
                        filteredData = data.Where(_item => _item.Month == a || _item.Year == a).ToList();
                    }
                    else
                    {
                        string text = request.Search.Value.Trim().ToLower();
                        filteredData = data.Where(_item => _item.FacilityName != null && _item.FacilityName.ToLower().Contains(text)).ToList();
                    }
                }
                else if (request.Search.Value.Contains("/"))
                {
                    string search = request.Search.Value.Trim();
                    string[] words = search.Split('/');
                    int.TryParse(words[0], out y);
                    int.TryParse(words[1], out a);
                    filteredData = data.Where(_item => _item.Month == a && _item.Year == y).ToList();
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

        [Authorize(Roles = "administrator")]
        public IActionResult adminNmr()
        {
            return View();
        }
        [Authorize(Roles = "dataentry,administrator")]
        public IActionResult hfnmr()
        {
            return View();
        }
        //IDataTablesRequest request
        [Authorize(Roles = "administrator")]
        [HttpPost]
        public async Task<IActionResult> AdminPageData(DataTables.AspNet.Core.IDataTablesRequest request)
        {
            _context.Database.SetCommandTimeout(5000);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            List<NmrVm> data;
            IQueryable<Nmr> query = _context.Nmr;
            if (user.TenantId != 1)
            {
                query = _context.Nmr.Where(m => m.Tenant.Equals(user.TenantId));
            }

            
            data = await query.Select(m => new NmrVm()
            {
                Nmrid = m.Nmrid,
                FacilityId=m.FacilityId, 
                Province=m.Facility.DistNavigation.ProvCodeNavigation.ProvName,             
                FacilityName = m.Facility.FacilityName,
                TypeAbbrv =m.Facility.FacilityTypeNavigation.TypeAbbrv,
                Implementer = m.Implementer,
                HfStatus = m.HfactiveStatus.HfstatusDescription,
                stat = m.StatusId,
                Month = m.Month,
                Year = m.Year,
                OpeningDate = m.OpeningDate,
                UpdateDate = m.UpdateDate,
                message = m.message,
                PreparedBy = m.PreparedBy,
                username = m.UserName
            }).Where(m=>m.stat!=3).OrderByDescending(m => m.OpeningDate).AsNoTracking().ToListAsync();

            List<NmrVm> filteredData;
            if (String.IsNullOrWhiteSpace(request.Search.Value))
            {
                filteredData = data;
            }
            else
            {
                int a;
                int y;
                bool result = int.TryParse(request.Search.Value, out a);
                if (!request.Search.Value.Contains("/"))
                {
                    if (result)
                    {
                        filteredData = data.Where(_item => _item.Month == a || _item.Year == a).ToList();
                    }
                    else
                    {
                        string text = request.Search.Value.Trim().ToLower();
                        filteredData = data.Where(_item => _item.FacilityName != null && _item.FacilityName.ToLower().Contains(text)).ToList();
                    }
                }
                else if (request.Search.Value.Contains("/"))
                {
                    string search = request.Search.Value.Trim();
                    string[] words = search.Split('/');
                    int.TryParse(words[0], out y);
                    int.TryParse(words[1], out a);
                    filteredData = data.Where(_item => _item.Month == a && _item.Year == y).ToList();
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

        [Authorize(Roles = "dataentry")]
        public IActionResult Create()
        {
            List<Provinces> Provinces = new List<Provinces>();
            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Provinces, "ProvCode", "ProvName");

            ViewData["FacilityId"] = new SelectList(_context.FacilityInfo, "FacilityId", "FacilityName");
            var implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpAcronym = " ", ImpName = "select" });
            ViewData["HfactiveStatusId"] = new SelectList(_context.LkpHfstatus, "HfactiveStatusId", "HfstatusDescription");
            return View();
        }

        [Authorize(Roles = "dataentry")]
        public JsonResult districts(string ProvCode)
        {
            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts where dist.ProvCode == ProvCode select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "0", DistName = "select" });
            return Json(new SelectList(Districts, "DistCode", "DistName"));
        }
        [Authorize(Roles = "dataentry")]
        public JsonResult facilities(string DistCode)
        {
            List<FacilityInfo> facilities = new List<FacilityInfo>();
            facilities = (from fac in _context.FacilityInfo where fac.DistCode == DistCode && fac.ActiveStatus == "Y" select fac).ToList();
            facilities.Insert(0, new FacilityInfo { FacilityId = 0, FacilityName = "select" });
            return Json(new SelectList(facilities, "FacilityId", "FacilityFull"));
        }
        // POST: Nmr/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Create([Bind("Nmrid,Commen,FacilityId,Flanumber,HfactiveStatusId,Month,OpeningDate,PreparedBy,UpdateDate,UserName,Year,isHumanitarian,mYear,mMonth,FacilityType")] Nmr nmr)
        {
            var FacType = (from f in _context.FacilityInfo
                           where f.FacilityId == nmr.FacilityId
                           select f).FirstOrDefault();
            var fac = _context.FacilityInfo.Where(m => m.FacilityId.Equals(nmr.FacilityId)).SingleOrDefault();
            List<Provinces> provinces = new List<Provinces>();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                nmr.OpeningDate = DateTime.Now;
                nmr.Nmrid = String.Format("{0}-{1}-{2}", nmr.FacilityId, nmr.Year, nmr.Month);
                nmr.UserName = User.Identity.Name;
                nmr.Implementer = fac.Implementer;
                nmr.StatusId = 1;
                nmr.Tenant = user.TenantId;
                if (nmr.Month < 10)
                {
                    nmr.mYear = (nmr.Year + 621);
                    nmr.mMonth = (nmr.Month + 3);
                }
                else if (nmr.Month > 9)
                {
                    nmr.mYear = (nmr.Year + 622);
                    nmr.mMonth = (nmr.Month - 9);
                }
                nmr.FacilityType = FacType.FacilityType;
                _context.Add(nmr);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(String.Empty, "Only one report per month is allowed.");
                    provinces = (from prov in _context.Provinces select prov).ToList();
                    provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
                    ViewData["ProvList"] = new SelectList(provinces, "ProvCode", "ProvName");
                    ViewData["FacilityId"] = new SelectList(_context.FacilityInfo, "FacilityId", "FacilityName");
                    ViewData["HfactiveStatusId"] = new SelectList(_context.LkpHfstatus, "HfactiveStatusId", "HfstatusDescription");
                    return View(nmr);
                }

                return RedirectToAction("questions", new { id = nmr.Nmrid });
                
            }
            provinces = (from prov in _context.Provinces select prov).ToList();
            provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(provinces, "ProvCode", "ProvName");
            ViewData["FacilityId"] = new SelectList(_context.FacilityInfo, "FacilityId", "FacilityId", nmr.FacilityId);
            var implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpAcronym = " ", ImpName = "select" });
            ViewData["Implementers"] = new SelectList(implementers, "ImpAcronym", "ImpAcronym");
            ViewData["HfactiveStatusId"] = new SelectList(_context.LkpHfstatus, "HfactiveStatusId", "HfactiveStatusId", nmr.HfactiveStatusId);
            return View(nmr);
        }

        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nmr = await _context.Nmr.Include(m => m.Facility).SingleOrDefaultAsync(m => m.Nmrid == id);
            if (nmr == null)
            {
                return NotFound();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {

                return BadRequest();
            }
            if (nmr.UserName != User.Identity.Name)
            {
                return BadRequest();
            }
            //get nmrID for HFID
            HttpContext.Session.SetString("mynmrid", id);
            List<FacilityInfo> Facilities = new List<FacilityInfo>();
            List<Provinces> Provinces = new List<Provinces>();
            List<Districts> Districts = new List<Districts>();

            Provinces = (from prov in _context.Provinces select prov).ToList();
            Provinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Provinces, "ProvCode", "ProvName");

            Facilities = (from fac in _context.FacilityInfo select fac).ToList();
            Facilities.Insert(0, new FacilityInfo { FacilityId = 0, FacilityName = "Select" });
            ViewData["FacilityId"] = new SelectList(Facilities, "FacilityId", "FacilityFull");

            Districts = (from dist in _context.Districts select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "0", DistName = "Select" });
            ViewData["DistCode"] = new SelectList(Districts, "DistCode", "DistName");

            var implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpAcronym = " ", ImpName = "select" });
            ViewData["HfactiveStatusId"] = new SelectList(_context.LkpHfstatus, "HfactiveStatusId", "HfstatusDescription");
            fillHFID(id);
            return View(nmr);
        }


        [Authorize(Roles = "dataentry")]
        [HttpGet("/nmr/fillHFID")]
        public IActionResult fillHFID(string id)
        {
            var data = _context.Nmr.Include(d => d.Facility).Where(f => f.Nmrid == HttpContext.Session.GetString("mynmrid")).Select(d => new
            {
                ProvCode = d.Facility.DistNavigation.ProvCode + '-' + d.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                DistCode = d.Facility.DistCode + "-" + d.Facility.DistNavigation.DistName,
                FacilityId = d.FacilityId + "-" + d.Facility.FacilityName
            }).ToList();
            return Ok(data);
        }
        // POST: Nmr/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Edit(string id, [Bind("Nmrid,Commen,Month,Year,FacilityId,HfactiveStatusId,Implementer,PreparedBy,isHumanitarian,mYear,mMonth")] Nmr nmr)
        {
            if (id != nmr.Nmrid)
            {
                return BadRequest();
            }
            var FacType = (from f in _context.FacilityInfo
                           where f.FacilityId == nmr.FacilityId
                           select f).FirstOrDefault();
            var fac = _context.FacilityInfo.Where(m => m.FacilityId.Equals(nmr.FacilityId)).SingleOrDefault();


            if (ModelState.IsValid)
            {
                var item = await _context.Nmr.Include(m => m.Facility).SingleOrDefaultAsync(m => m.Nmrid == id);
                if (item == null)
                {
                    return NotFound();
                }
                if (item.StatusId == 3 || item.HfactiveStatusId != 1)
                {

                    return BadRequest();
                }
                if (item.UserName != User.Identity.Name)
                {
                    return BadRequest();
                }

                try
                {
                    item.UpdateDate = DateTime.Now;
                    item.Month = nmr.Month;
                    item.Year = nmr.Year;
                    item.Commen = nmr.Commen;
                    item.PreparedBy = nmr.PreparedBy;
                    item.FacilityId = nmr.FacilityId;
                    item.StatusId = 2;
                    item.HfactiveStatusId = nmr.HfactiveStatusId;
                    item.isHumanitarian = nmr.isHumanitarian;
                    item.Implementer = fac.Implementer;
                    if (nmr.Month < 10)
                    {
                        item.mYear = (nmr.Year + 621);
                        item.mMonth = (nmr.Month + 3);
                    }
                    else if (item.Month > 9)
                    {
                        item.mYear = (nmr.Year + 622);
                        item.mMonth = (nmr.Month - 9);
                    }
                    item.FacilityType = FacType.FacilityType;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NmrExists(nmr.Nmrid))
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
            ViewData["FacilityId"] = new SelectList(_context.FacilityInfo, "FacilityId", "FacilityName");
            var implementers = (from imp in _context.Implementers select imp).ToList();
            implementers.Insert(0, new Implementers { ImpAcronym = " ", ImpName = "select" });
            ViewData["HfactiveStatusId"] = new SelectList(_context.LkpHfstatus, "HfactiveStatusId", "HfstatusDescription");
            return View(nmr);
        }

        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nmr = await _context.Nmr.Include(m => m.Facility).SingleOrDefaultAsync(m => m.Nmrid == id);
            if (nmr == null)
            {
                return NotFound();
            }
            if (nmr.UserName != User.Identity.Name)
            {
                return BadRequest();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            return View(nmr);
        }

        // POST: Nmr/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == id);
            if (nmr.UserName != User.Identity.Name)
            {
                return BadRequest();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            _context.Nmr.Remove(nmr);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> reports(string nmrid)
        {
            if (nmrid == null)
            {
                return NotFound();
            }

            var items = _context.TblkpStatus.Select(s => new
            {
                StatusId = s.StatusId,
                StatusDescription = s.StatusDescription
            });
            ViewData["stat"] = new SelectList(items, "StatusId", "StatusDescription");
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<Nmr> query = _context.Nmr.Where(m => m.Nmrid == nmrid);
            if (user.TenantId != 1)
            {
                query = _context.Nmr.Where(m => m.Nmrid == nmrid && m.Tenant.Equals(user.TenantId));
            }
            var nmr = query.SingleOrDefault();
            return View(nmr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrator")]
        //[Authorize(Policy = "admin")]
        public async Task<IActionResult> verifyNmr([Bind("Nmrid,message,StatusId")] ReviewViewModel nmr)
        {
            if (nmr.Nmrid == null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var item = await _context.Nmr.Where(m => m.Nmrid == nmr.Nmrid).SingleOrDefaultAsync();
                if (item == null)
                {
                    return NotFound();
                }
                if (item.StatusId == 3)
                {
                    ModelState.AddModelError(string.Empty, "");
                    return View("reports", item);
                }
                if (nmr.StatusId == 3)
                {

                    var tSum = _context.TblOtp.Where(m => m.Nmrid == nmr.Nmrid).AsNoTracking();
                    var newNmrid = _context.TlkpOtptfu.Where(m => m.AgeGroup.ToLower().Contains("total")).AsNoTracking().SingleOrDefault();
                    if (tSum.Any() && newNmrid != null)
                    {
                        var newOtp = new TblOtp();
                        newOtp.Nmrid = nmr.Nmrid;
                        newOtp.Otpid = newNmrid.Otptfuid;
                        newOtp.Cured = tSum.Sum(m => m.Cured);
                        newOtp.NonCured = tSum.Sum(m => m.NonCured);
                        newOtp.Death = tSum.Sum(m => m.Death);
                        newOtp.Default = tSum.Sum(m => m.Default);
                        newOtp.Defaultreturn = tSum.Sum(m => m.Defaultreturn);
                        newOtp.Fromscotp = tSum.Sum(m => m.Fromscotp);
                        newOtp.Fromsfp = tSum.Sum(m => m.Fromsfp);
                        newOtp.Muac115 = tSum.Sum(m => m.Muac115);
                        newOtp.Z3score = tSum.Sum(m => m.Z3score);
                        newOtp.Odema = tSum.Sum(m => m.Odema);
                        newOtp.TFemale = tSum.Sum(m => m.TFemale);
                        newOtp.TMale = tSum.Sum(m => m.TMale);
                        newOtp.RefOut = tSum.Sum(m => m.RefOut);
                        newOtp.Totalbegin = tSum.Sum(m => m.Totalbegin);
                        newOtp.UserName = item.UserName;
                        try
                        {
                            _context.TblOtp.Add(newOtp);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!NmrExists(nmr.Nmrid))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    var tSumout = _context.TblOtptfu.Where(m => m.Nmrid == nmr.Nmrid).AsNoTracking();
                    if (tSumout.Any() && newNmrid != null)
                    {
                        var newOtptfu = new TblOtptfu();
                        newOtptfu.Nmrid = nmr.Nmrid;
                        newOtptfu.Otptfuid = newNmrid.Otptfuid;
                        newOtptfu.Cured = tSumout.Sum(m => m.Cured);
                        newOtptfu.NonCured = tSumout.Sum(m => m.NonCured);
                        newOtptfu.Death = tSumout.Sum(m => m.Death);
                        newOtptfu.Default = tSumout.Sum(m => m.Default);
                        newOtptfu.Defaultreturn = tSumout.Sum(m => m.Defaultreturn);
                        newOtptfu.Fromscotp = tSumout.Sum(m => m.Fromscotp);
                        newOtptfu.Fromsfp = tSumout.Sum(m => m.Fromsfp);
                        newOtptfu.Muac115 = tSumout.Sum(m => m.Muac115);
                        newOtptfu.Z3score = tSumout.Sum(m => m.Z3score);
                        newOtptfu.Odema = tSumout.Sum(m => m.Odema);
                        newOtptfu.TFemale = tSumout.Sum(m => m.TFemale);
                        newOtptfu.TMale = tSumout.Sum(m => m.TMale);
                        newOtptfu.RefOut = tSumout.Sum(m => m.RefOut);
                        newOtptfu.Totalbegin = tSumout.Sum(m => m.Totalbegin);
                        newOtptfu.UserName = item.UserName;
                        try
                        {
                            _context.TblOtptfu.Add(newOtptfu);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!NmrExists(nmr.Nmrid))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    var tMam = _context.TblMam.Include(m => m.Mam).Where(m => m.Nmrid == nmr.Nmrid).AsNoTracking();
                    var tChildid = _context.TlkpSfp.Where(m => m.AgeGroup.ToLower().Contains("total children")).AsNoTracking().SingleOrDefault();
                    var tWomanid = _context.TlkpSfp.Where(m => m.AgeGroup.ToLower().Contains("total women")).AsNoTracking().SingleOrDefault();
                    if (tMam.Any() && tChildid != null && tWomanid != null)
                    {
                        var tChild = new TblMam();
                        var tWomen = new TblMam();

                        tChild.Nmrid = nmr.Nmrid;
                        tChild.Mamid = tChildid.Sfpid;
                        tChild.Cured = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Cured);
                        tChild.NonCured = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.NonCured);
                        tChild.Deaths = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Deaths);
                        tChild.Defaulters = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Defaulters);
                        tChild.Absents = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Absents);
                        tChild.Transfers = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Transfers);
                        tChild.Muac12 = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Muac12);
                        tChild.Muac23 = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Muac23);
                        tChild.TFemale = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.TFemale);
                        tChild.Zscore23 = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Zscore23);
                        tChild.TMale = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.TMale);
                        tChild.ReferIn = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.ReferIn);
                        tChild.Totalbegin = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children")).Sum(m => m.Totalbegin);
                        tChild.UserName = item.UserName;

                        tWomen.Nmrid = nmr.Nmrid;
                        tWomen.Mamid = tWomanid.Sfpid;
                        tWomen.Cured = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Cured);
                        tWomen.NonCured = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.NonCured);
                        tWomen.Deaths = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Deaths);
                        tWomen.Defaulters = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Defaulters);
                        tWomen.Absents = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Absents);
                        tWomen.Transfers = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Transfers);
                        tWomen.ReferIn = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.ReferIn);
                        tWomen.Muac12 = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Muac12);
                        tWomen.Muac23 = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Muac23);
                        tWomen.TFemale = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.TFemale);
                        tWomen.Zscore23 = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Zscore23);
                        tWomen.TMale = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.TMale);
                        tWomen.Totalbegin = tMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")).Sum(m => m.Totalbegin);
                        tWomen.UserName = item.UserName;
                        try
                        {
                            _context.TblMam.Add(tChild);
                            _context.TblMam.Add(tWomen);
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!NmrExists(nmr.Nmrid))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
                try
                {
                    item.StatusId = nmr.StatusId;
                    item.message = nmr.message;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NmrExists(nmr.Nmrid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("reports",new {nmrid=item.Nmrid});

            }
            return View("reports");
        }

        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> samin(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var que = await _context.Nmr.Where(m => m.Nmrid == id).AsNoTracking().SingleOrDefaultAsync();
            if (que == null)
            {
                return NotFound();
            }

            if (que.StatusId == 3 || que.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (User.Identity.Name != que.UserName) { return BadRequest(); }
            qpartialVm item = new qpartialVm();
            item.Nmrid = que.Nmrid;
            item.IawgKwashiorkor = que.IawgKwashiorkor;
            item.IawgMarasmus = que.IawgMarasmus;
            item.IalsKwashiorkor = que.IalsKwashiorkor;
            item.IalsMarasmus = que.IalsMarasmus;
            item.IpdAdmissionsByChws = que.IpdAdmissionsByChws;
            item.IpdRutfstockOutWeeks = que.IpdRutfstockOutWeeks;
            item.UserName = que.UserName;
            return View("saminpatient", item);
        }
        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> samout(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var que = await _context.Nmr.Where(m => m.Nmrid == id).AsNoTracking().SingleOrDefaultAsync();
            if (que == null)
            {
                return NotFound();
            }

            if (que.StatusId == 3 || que.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (User.Identity.Name != que.UserName) { return BadRequest(); }
            qpartialVm item = new qpartialVm();
            item.Nmrid = que.Nmrid;
            item.OawgKwashiorkor = que.OawgKwashiorkor;
            item.OawgMarasmus = que.OawgMarasmus;
            item.OalsKwashiorkor = que.OalsKwashiorkor;
            item.OalsMarasmus = que.OalsMarasmus;
            item.OpdAdmissionsByChws = que.OpdAdmissionsByChws;
            item.OpdRutfstockOutWeeks = que.OpdRutfstockOutWeeks;
            item.UserName = que.UserName;
            return View("samOut", item);
        }
        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> msFormV(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == id);
            if (nmr == null)
            {
                return NotFound();
            }
            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (User.Identity.Name != nmr.UserName) { return Unauthorized(); }
            var model = new TblMn();
            model.Nmrid = nmr.Nmrid;
            model.UserName = nmr.UserName;
            return View("Micronutrients", model);
        }
        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> iycfForm(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == id);
            if (item == null)
            {
                return NotFound();
            }
            if (item.StatusId == 3 || item.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (User.Identity.Name != item.UserName) { return BadRequest(); }
            var model = new TblIycf();
            model.Nmrid = item.Nmrid;
            return View("iycf", model);
        }
        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> opdmam(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nmr = await _context.Nmr.AsNoTracking().SingleOrDefaultAsync(m => m.Nmrid == id);
            if (nmr == null)
            {
                return NotFound();
            }

            if (nmr.StatusId == 3 || nmr.HfactiveStatusId != 1)
            {
                return BadRequest();
            }
            if (User.Identity.Name != nmr.UserName) { return BadRequest(); }
            qpartialVm item = new qpartialVm();
            item.Nmrid = nmr.Nmrid;
            item.SfpAls = nmr.SfpAls;
            item.SfpAwg = nmr.SfpAwg;
            item.MamRusfstockoutWeeks = nmr.MamRusfstockoutWeeks;
            item.MamAddminsionByChws = nmr.MamAddminsionByChws;
            item.UserName = nmr.UserName;
            return View("opdMam", item);
        }
        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> questions(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == id);
            if (nmr == null)
            {
                return NotFound();
            }
            if (User.Identity.Name != nmr.UserName) { return BadRequest(); }
            return View("questions", nmr);
        }

        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> submission(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nmr = await _context.Nmr.SingleOrDefaultAsync(m => m.Nmrid == id);
            if (nmr == null)
            {
                return NotFound();
            }
            if (User.Identity.Name != nmr.UserName) { return BadRequest(); }
            return View("submission", nmr);
        }
        private bool NmrExists(string id)
        {
            return _context.Nmr.Any(e => e.Nmrid == id);
        }
    }
}

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
using System.Net.Http;
using Newtonsoft.Json;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator")]
    [Authorize(Policy = "admin")]
    
    public class FacilityInfoController : Controller
    {
        private readonly WebNutContext _context;

        public FacilityInfoController(WebNutContext context)
        {
            _context = context;
        }

        // GET: FacilityInfo
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult PageData(IDataTablesRequest request)
        {
            var data = _context.FacilityInfo.Include(m => m.FacilityTypeNavigation).AsNoTracking().ToList();
            List<FacilityInfo> filteredData;
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
                    filteredData = data.Where(_item => _item.FacilityName != null && _item.FacilityName.ToLower().Contains(search.ToLower()) || _item.FacilityNameDari != null && _item.FacilityNameDari.Contains(search)
                    || _item.FacilityNamePashto != null && _item.FacilityNamePashto.Contains(search)).ToList();
                }
                else
                {
                    filteredData = data;
                }
            }

            // Paging filtered data.
            // Paging is rather manual due to in-memmory (IEnumerable) data.
            var dataPage = filteredData.Skip(request.Start).Take(request.Length);

            // Response creation. To create your response you need to reference your request, to avoid
            // request/response tampering and to ensure response will be correctly created.
            var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);

            // Easier way is to return a new 'DataTablesJsonResult', which will automatically convert your
            // response to a json-compatible content, so DataTables can read it when received.
            return new DataTablesJsonResult(response, true);
        }
        // GET: FacilityInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityInfo = await _context.FacilityInfo.SingleOrDefaultAsync(m => m.FacilityId == id);
            if (facilityInfo == null)
            {
                return NotFound();
            }

            return View(facilityInfo);
        }

        // GET: FacilityInfo/Create
        public IActionResult Create()
        {
            var items = _context.FacilityTypes.Select(s => new
            {
                FacTypeCode = s.FacTypeCode,
                description = string.Format("{0} - {1}", s.FacTypeCatCode, s.FacType)
            });
            var imps = _context.Implementers.ToList();
            ViewData["fac"] = new SelectList(items, "FacTypeCode", "description");
            ViewData["imps"] = new SelectList(imps, "ImpAcronym", "ImpAcronym");

            ViewData["Districts"] = new SelectList(_context.Districts, "DistCode", "DistName");
            return View();
        }

        // POST: FacilityInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Implementer,FacilityId,ActiveStatus,DateEstablished,DistCode,FacilityName,FacilityNameDari,FacilityNamePashto,FacilityType,Gpslattitude,Gpslongtitude,Lat,Location,LocationDari,LocationPashto,Lon,SubImplementer,ViliCode")] FacilityInfo facilityInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facilityInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var items = _context.FacilityTypes.Select(s => new
            {
                FacTypeCode = s.FacTypeCode,
                description = string.Format("{0} - {1}", s.FacTypeCatCode, s.FacType)
            });
            var imps = _context.Implementers.ToList();
            ViewData["fac"] = new SelectList(items, "FacTypeCode", "description");
            ViewData["imps"] = new SelectList(imps, "ImpAcronym", "ImpAcronym");
            ViewData["Districts"] = new SelectList(_context.Districts, "DistCode", "DistName");
            return View(facilityInfo);
        }

        // GET: FacilityInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityInfo = await _context.FacilityInfo.SingleOrDefaultAsync(m => m.FacilityId == id);
            if (facilityInfo == null)
            {
                return NotFound();
            }
            var items = _context.FacilityTypes.Select(s => new
            {
                FacTypeCode = s.FacTypeCode,
                description = string.Format("{0} - {1}", s.FacTypeCatCode, s.FacType)
            });
            var imps = _context.Implementers.ToList();
            ViewData["fac"] = new SelectList(items, "FacTypeCode", "description");
            ViewData["imps"] = new SelectList(imps, "ImpAcronym", "ImpAcronym");
            ViewData["Districts"] = new SelectList(_context.Districts, "DistCode", "DistName");
            return View(facilityInfo);
        }

        // POST: FacilityInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Implementer,FacilityId,ActiveStatus,DateEstablished,DistCode,FacilityName,FacilityNameDari,FacilityNamePashto,FacilityType,Gpslattitude,Gpslongtitude,Lat,Location,LocationDari,LocationPashto,Lon,SubImplementer,ViliCode")] FacilityInfo facilityInfo)
        {
            if (id != facilityInfo.FacilityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facilityInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityInfoExists(facilityInfo.FacilityId))
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
            var items = _context.FacilityTypes.Select(s => new
            {
                FacTypeCode = s.FacTypeCode,
                description = string.Format("{0} - {1}", s.FacTypeCatCode, s.FacType)
            });
            ViewData["fac"] = new SelectList(items, "FacTypeCode", "description");
            ViewData["Districts"] = new SelectList(_context.Districts, "DistCode", "DistName");
            return View(facilityInfo);
        }

        // GET: FacilityInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facilityInfo = await _context.FacilityInfo.SingleOrDefaultAsync(m => m.FacilityId == id);
            if (facilityInfo == null)
            {
                return NotFound();
            }

            return View(facilityInfo);
        }

        // POST: FacilityInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facilityInfo = await _context.FacilityInfo.SingleOrDefaultAsync(m => m.FacilityId == id);
            _context.FacilityInfo.Remove(facilityInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool FacilityInfoExists(int id)
        {
            return _context.FacilityInfo.Any(e => e.FacilityId == id);
        }

        //public IActionResult TemFacilities()
        //{
        //var data = _context.TempFacilities.Select(m => new
        //    {
        //        FacilityId = m.FacilityId,
        //        DistCode =m.DistCode,
        //        FacilityName = m.FacilityName,
        //        FacilityNameDari=m.FacilityNameDari,
        //        FacilityNamePashto=m.FacilityNamePashto,
        //        FacilityType =m.FacilityType,
        //        Location=m.Location,
        //        LocationDari=m.LocationDari,
        //        LocationPashto=m.LocationPashto,
        //        ViliCode =m.ViliCode,
        //        Lat =m.Lat,
        //        Lon=m.Lon,
        //        Implementer = m.Implementer,
        //        SubImplementer=m.SubImplementer,
        //        ActiveStatus = m.ActiveStatus,
        //        DateEstablished=m.DateEstablished
        //    }).OrderBy(m =>m.FacilityId).ToList();
        //    return null;
        //}
        public async Task<IActionResult>  ImportFacilities()
        {
            var apiUrl = _context.Apistore.Where(m => m.filtervalue == "common").FirstOrDefault();

            _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [TempFacilities]");

            Facilities facilities = new Facilities();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiUrl.apiurl))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    facilities = JsonConvert.DeserializeObject<Facilities>(apiResponse);
                }
            }
            List<TempFacilities> tempFacilities = new List<TempFacilities>();
            foreach (var item in facilities.items)
            {
                tempFacilities.Add(new TempFacilities
                {
                    FacilityId = item.FacilityId,
                    DistrictCode = item.DistrictCode,
                    FacilityName = item.FacilityName,
                    FacilityNameDari = item.FacilityNameDari,
                    FacilityNamePashto = item.FacilityNamePashto,
                    Location = item.Location,
                    LocationDari = item.LocationDari,
                    LocationPashto = item.LocationPashto,
                    FacilityTypeId = item.FacilityTypeId,
                    IsActive = item.IsActive,
                    Implementer = item.Implementer,
                    DateEstablished = item.DateEstablished,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (tempFacilities.Count > 0)
                {
                    _context.AddRange(tempFacilities);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Import");
        }

        public IActionResult Import()
        {
            _context.Database.ExecuteSqlCommand("exec UpdateFacilityInfo");
            var data = _context.TempFacilities.ToList();
            ViewBag.totalfacilities = data.Count();
            _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [TempFacilities]");
            return View();
        }
        public class Facilities
        {
            public List<FacilityInfoApi> items;
        }

        private bool Exists(int id)
        {
            return _context.TempFacilities.Any(e => e.FacilityId == id);
        }
    }

}
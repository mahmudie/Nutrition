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

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy ="admin")]
    public class ERFacilitiesController : Controller
    {
        private readonly WebNutContext _context;

        public ERFacilitiesController(WebNutContext context)
        {
            _context = context;
        }

        // GET: FacilityInfo
        //[Authorize(Roles = "dataentry")]
        public IActionResult Index()
        {
            return View();
        }
        //[Authorize(Roles = "dataentry")]
        public IActionResult PageData(IDataTablesRequest request)
        {
            var data = _context.ERFacilities.Include(m => m.FacilityTypeNavigation).AsNoTracking().ToList();
            List<ERFacilities> filteredData;
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

            List<FacilityTypes> facilityTypes = new List<FacilityTypes>();
            facilityTypes = (from factype in _context.FacilityTypes select factype).ToList();
            facilityTypes.Insert(0, new FacilityTypes { FacTypeCode=0,FacType="select" });

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
            return View();
        }

        [Authorize(Roles = "dataentry,administrator")]
        public JsonResult districts(string ProvCode)
        {
            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts where dist.ProvCode == ProvCode select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "0", DistName = "select" });
            return Json(new SelectList(Districts, "DistCode", "DistName"));
        }

        // POST: FacilityInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ERFacilities erFacilities)
        {
            if (ModelState.IsValid)
            {
                erFacilities.Status = "Y";
                _context.Add(erFacilities);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
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
            return View(erFacilities);
        }

        // GET: FacilityInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: FacilityInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ERFacilities erFacilities)
        {
            if (id != erFacilities.FacilityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(erFacilities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityInfoExists(erFacilities.FacilityId))
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
            return View(erFacilities);
        }

        // GET: FacilityInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: FacilityInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facilityInfo = await _context.ERFacilities.SingleOrDefaultAsync(m => m.FacilityId == id);
            _context.ERFacilities.Remove(facilityInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool FacilityInfoExists(int id)
        {
            return _context.FacilityInfo.Any(e => e.FacilityId == id);
        }
    }

}
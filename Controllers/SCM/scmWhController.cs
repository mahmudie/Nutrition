using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class scmWhController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmWhController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var Regions = _context.scmRegions.ToList();
            var provinces = _context.Provinces.Select(m=>new
            {
                ProvinceId=m.ProvCode,
                ProvinceName=m.ProvName
            }).ToList();

            var ips = _context.Implementers.Select(m=>new
            {
                ImpId =m.ImpCode,
                ImpName =m.ImpAcronym
            }).ToList();

            ViewBag.ProvinceSource = provinces;
            ViewBag.RegionSource = Regions;
            ViewBag.IPSource = ips;

            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmWarehouses.ToList();
            IEnumerable DataSource = data;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<scmWarehouses>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }


        public IActionResult Insert([FromBody]CRUDModel<scmWarehouses> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmWarehouses scmWh = new scmWarehouses();
            if (scmWh == null) { return BadRequest(); }

            scmWh.RegionId = value.Value.RegionId;
            scmWh.ProvinceId = value.Value.ProvinceId;
            scmWh.ImpId = value.Value.ImpId;
            scmWh.Location = value.Value.Location;
            scmWh.Active = value.Value.Active;

            try
            {
                _context.Add(scmWh);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }

        public IActionResult Update([FromBody]CRUDModel<scmWarehouses> model)
        {
            var scmWh = _context.scmWarehouses.Where(cat => cat.WhId == model.Value.WhId).FirstOrDefault();
            if (scmWh != null)
            {
                scmWh.RegionId = model.Value.RegionId;
                scmWh.ProvinceId = model.Value.ProvinceId;
                scmWh.ImpId = model.Value.ImpId;
                scmWh.Location = model.Value.Location;
                scmWh.Active = model.Value.Active;
            }
            _context.Entry(scmWh).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(scmWh);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(model.Value.WhId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        public IActionResult Remove([FromBody]CRUDModel<scmWarehouses> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmWarehouses item = _context.scmWarehouses.Where(m => m.WhId.Equals(id)).FirstOrDefault();
                _context.scmWarehouses.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.scmWarehouses.Any(e => e.WhId == id);
        }
    }
}
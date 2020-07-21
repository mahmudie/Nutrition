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
    public class scmWarehousesController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmWarehousesController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var regions = _context.scmRegions.Select(m => new
            {
                RegionId = m.RegionId,
                RegionLong = m.RegionLong
            }).ToList();

            var implementers = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                ImplementerName = m.ImpAcronym
            }).ToList();

            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();


            var levels = _context.Lkpwhlevels.Select(m => new
            {
                LevelId = m.LevelId,
                LevelName = m.LevelDescriptoin
            }).ToList();

            ViewBag.RegionSource = regions;
            ViewBag.ImplementerSource = implementers;
            ViewBag.ProvinceSource = provinces;
            ViewBag.LevelSource = levels;

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

            scmWarehouses wrhouse = new scmWarehouses();
            if (wrhouse == null) { return BadRequest(); }

            wrhouse.RegionId = value.Value.RegionId;
            wrhouse.ProvinceId = value.Value.ProvinceId;
            wrhouse.ImpId = value.Value.ImpId;
            wrhouse.Location = value.Value.Location;
            wrhouse.Active = value.Value.Active;
            wrhouse.LevelId = value.Value.LevelId;
            wrhouse.WarehouseName = value.Value.WarehouseName;

            try
            {
                _context.Add(wrhouse);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<scmWarehouses> value)
        {
            var wrhouse = _context.scmWarehouses.Where(cat => cat.WhId == value.Value.WhId).FirstOrDefault();
            if (wrhouse != null)
            {
                wrhouse.RegionId = value.Value.RegionId;
                wrhouse.ProvinceId = value.Value.ProvinceId;
                wrhouse.ImpId = value.Value.ImpId;
                wrhouse.Location = value.Value.Location;
                wrhouse.Active = value.Value.Active;
                wrhouse.LevelId = value.Value.LevelId;
                wrhouse.WarehouseName = value.Value.WarehouseName;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(wrhouse).State = EntityState.Modified;

            try
            {
                _context.Update(wrhouse);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.WhId))
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
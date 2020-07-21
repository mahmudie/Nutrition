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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class scmStockaverageController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmStockaverageController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var items = _context.TlkpSstock.Select(m => new
            {
                SupplyId = m.SstockId,
                SupplyName = m.Item
            }).ToList();

            var facilityTypes = _context.FacilityTypes.Select(m => new
            {
                FacilityTypeId = m.FacTypeCode,
                FacilityTypeName = m.TypeAbbrv + " - "+m.FacType
            }).ToList();

            //ViewBag.ItemSource = new SelectList(items, "SupplyId", "SupplyName");
            ViewBag.ItemSource = items;
            ViewBag.FacilityTypeSource = facilityTypes;


            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmStockaverage.ToList();
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
            int count = DataSource.Cast<scmStockaverage>().Count();
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

        public ActionResult Averagecalculation(int id)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("exec dbo.AddAverage");

            }
            catch (Exception)
            {

                throw;
            }

           return RedirectToAction("Index");
        }
        public ActionResult RunIndex()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Insert([FromBody]CRUDModel<scmStockaverage> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmStockaverage stockavg = new scmStockaverage();
            if (stockavg == null) { return BadRequest(); }

            stockavg.type = value.Value.type;
            stockavg.year = value.Value.year;
            stockavg.supplyId = value.Value.supplyId;
            stockavg.program = value.Value.program;
            stockavg.totalNeeds = value.Value.totalNeeds;
            stockavg.facilityTypeId = value.Value.facilityTypeId;

            try
            {
                _context.Add(stockavg);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<scmStockaverage> value)
        {
            var stockavg = _context.scmStockaverage.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (stockavg != null)
            {
                stockavg.type = value.Value.type;
                stockavg.year = value.Value.year;
                stockavg.supplyId = value.Value.supplyId;
                stockavg.program = value.Value.program;
                stockavg.totalNeeds = value.Value.totalNeeds;
                stockavg.facilityTypeId = value.Value.facilityTypeId;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(stockavg).State = EntityState.Modified;

            try
            {
                _context.Update(stockavg);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.id))
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

        public IActionResult Remove([FromBody]CRUDModel<scmStockaverage> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmStockaverage item = _context.scmStockaverage.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmStockaverage.Remove(item);
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
            return _context.scmStockaverage.Any(e => e.id == id);
        }
    }
}
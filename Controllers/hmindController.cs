using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using System.Collections;
using System;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy ="admin")]
    public class hmindController : Controller
    {
        private readonly WebNutContext _context;

        public hmindController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Hmisindicators.ToList();
            ViewBag.DataSource = data;
            return View();
        }
        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.Hmisindicators.ToList();
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
            int count = DataSource.Cast<hmisindicators>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<hmisindicators> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            hmisindicators categ = new hmisindicators();
            if (categ == null) { return BadRequest(); }

            categ.IndicatorId = value.Value.IndicatorId;
            categ.IndicatorDescription = value.Value.IndicatorDescription;
            categ.IndDataSource = value.Value.IndDataSource;
            categ.IndType = value.Value.IndType;
            categ.IndCaluculation = value.Value.IndCaluculation;


            try
            {
                _context.Add(categ);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<hmisindicators> model)
        {
            var data = _context.Hmisindicators.Where(cat => cat.IndicatorId == model.Value.IndicatorId).FirstOrDefault();
            if (data != null)
            {
                data.IndicatorDescription = model.Value.IndicatorDescription;
                data.IndDataSource = model.Value.IndDataSource;
                data.IndType = model.Value.IndType;
                data.IndCaluculation = model.Value.IndCaluculation;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(data).State = EntityState.Modified;

            try
            {
                _context.Update(data);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(model.Value.IndicatorId))
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
        public IActionResult Remove([FromBody]CRUDModel<hmisindicators> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                hmisindicators item = _context.Hmisindicators.Where(m => m.IndicatorId.Equals(id)).FirstOrDefault();
                _context.Hmisindicators.Remove(item);
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
            return _context.Hmisindicators.Any(e => e.IndicatorId == id);
        }
    }
}

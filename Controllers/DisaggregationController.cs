using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using System.Collections;
using System;
using DataSystem.Models.ViewModels;
using System.Collections.Generic;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class DisaggregationController : Controller
    {
        private readonly WebNutContext _context;

        public DisaggregationController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<CategoriesVm> catvm = new List<CategoriesVm>();

            var catdata= _context.LkpCategories.ToList();
            foreach(var i in catdata)
            {
                catvm.Add( new CategoriesVm { CategoryId=i.CategoryId,CategoryName=i.CategoryName});
            }

            var data = _context.LkpDisaggregations.ToList();
            ViewBag.DataSource = data;
            ViewBag.CategSource = catvm;
            return View();
        }
        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.LkpDisaggregations.ToList();
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
            int count = DataSource.Cast<LkpDisaggregation>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<LkpDisaggregation> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LkpDisaggregation categ = new LkpDisaggregation();
            if (categ == null) { return BadRequest(); }

            categ.DisaggregId = value.Value.DisaggregId;
            categ.CategoryId = value.Value.CategoryId;
            categ.Disaggregation = value.Value.Disaggregation;
            categ.Ordno = value.Value.Ordno;


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
        public IActionResult Update([FromBody]CRUDModel<LkpDisaggregation> model)
        {
            var data = _context.LkpDisaggregations.Where(cat => cat.DisaggregId == model.Value.DisaggregId).FirstOrDefault();
            if (data != null)
            {
                data.Disaggregation = model.Value.Disaggregation;
                data.CategoryId = model.Value.CategoryId;
                data.Ordno = model.Value.Ordno;
                data.Disaggregation = model.Value.Disaggregation;
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
                if (!Exists(model.Value.CategoryId))
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
        public IActionResult Remove([FromBody]CRUDModel<LkpDisaggregation> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                LkpDisaggregation item = _context.LkpDisaggregations.Where(m => m.DisaggregId.Equals(id)).FirstOrDefault();
                _context.LkpDisaggregations.Remove(item);
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
            return _context.LkpDisaggregations.Any(e => e.DisaggregId == id);
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Syncfusion.EJ2.Base;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator")]
    [Authorize(Policy = "admin")]
    
    public class ZchecklistController : Controller
    {
        private readonly WebNutContext _context;
        public ZchecklistController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpOtptfus
        public IActionResult Index()
        {
            var data = _context.GetLkpChecklists.ToList();
            ViewBag.DataSource = data;
            return View();
        }
        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.GetLkpChecklists.ToList();
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
            int count = DataSource.Cast<lkpChecklist>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<lkpChecklist> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            lkpChecklist categ = new lkpChecklist();
            if (categ == null) { return BadRequest(); }

            categ.IntId = value.Value.IntId;
            categ.OrderId = value.Value.OrderId;
            categ.Stringorder = value.Value.Stringorder;
            categ.Description = value.Value.Description;
            categ.DescriptionDari = value.Value.DescriptionDari;
            categ.Active = value.Value.Active;
            categ.Type = value.Value.Type;

            try
            {
                _context.Add(categ);
                 _context.SaveChanges();
            }
            catch(Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<lkpChecklist> value)
        {
            var data = _context.GetLkpChecklists.Where(cat=>cat.IntId==value.Value.IntId).FirstOrDefault();
            if (data != null)
            {
                data.OrderId = value.Value.OrderId;
                data.Stringorder = value.Value.Stringorder;
                data.Description = value.Value.Description;
                data.DescriptionDari = value.Value.DescriptionDari;
                data.Active = value.Value.Active;
                data.Type = value.Value.Type;
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
                if (!Exists(value.Value.IntId))
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

        public IActionResult Remove([FromBody]CRUDModel<lkpChecklist> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if(Exists(id))
            {
                lkpChecklist item = _context.GetLkpChecklists.Where(m => m.IntId.Equals(id)).FirstOrDefault();
                _context.GetLkpChecklists.Remove(item);
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
            return _context.GetLkpChecklists.Any(e => e.IntId == id);
        }
    }
}

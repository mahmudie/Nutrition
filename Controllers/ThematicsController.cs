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
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class ThematicsController : Controller
    {
        private readonly WebNutContext _context;
        public ThematicsController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpOtptfus
        public IActionResult Index()
        {
            var data = _context.LkpThematicAreas.ToList();
            ViewBag.DataSource = data;
            return View();
        }
        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.LkpThematicAreas.ToList();
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
            int count = DataSource.Cast<lkpThematicArea>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<lkpThematicArea> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            lkpThematicArea categ = new lkpThematicArea();
            if (categ == null) { return BadRequest(); }

            categ.ThemeId = value.Value.ThemeId;
            categ.ThematicArea = value.Value.ThematicArea;

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
        public IActionResult Update([FromBody]CRUDModel<lkpThematicArea> model)
        {
            var data = _context.LkpThematicAreas.Where(t=>t.ThemeId==model.Value.ThemeId).FirstOrDefault();
            if (data != null)
            {
                data.ThematicArea = model.Value.ThematicArea;
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
                if (!Exists(model.Value.ThemeId))
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

        public IActionResult Remove([FromBody]CRUDModel<lkpThematicArea> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if(Exists(id))
            {
                lkpThematicArea item = _context.LkpThematicAreas.Where(m => m.ThemeId.Equals(id)).FirstOrDefault();
                _context.LkpThematicAreas.Remove(item);
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
            return _context.LkpThematicAreas.Any(e => e.ThemeId == id);
        }
    }
}

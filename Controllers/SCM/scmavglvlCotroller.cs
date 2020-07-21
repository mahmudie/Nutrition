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
    public class scmavglvlController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmavglvlController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
            {
                ViewBag.gridAdd = false;
                ViewBag.gridEdit = false;
                ViewBag.gridDelete = false;
            }
            else if ((User.IsInRole("administrator")|| User.IsInRole("unicef")|| User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
            {
                ViewBag.gridAdd = true;
                ViewBag.gridEdit = true;
                ViewBag.gridDelete = true;
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                ViewBag.gridAdd = false;
                ViewBag.gridEdit = false;
                ViewBag.gridDelete = false;
            }

            var averageleveler = _context.scmStockaverage
                .GroupBy(p => new { p.year, p.type })
                .Select(g => new { AveragelevelId = g.Key.year, AverageLevelName = g.Key.year+"-"+ g.Key.type }).ToList();

            ViewBag.averagelevels = averageleveler;

            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmAveragelevel.ToList();
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
            int count = DataSource.Cast<scmAveragelevel>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmAveragelevel> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmAveragelevel lvl = new scmAveragelevel();
            if (lvl == null) { return BadRequest(); }

            lvl.averagelevelId = value.Value.averagelevelId;
            lvl.isActive = value.Value.isActive;
            lvl.UserName = user.UserName;
            lvl.UpdateDate = DateTime.Now.Date;

            try
            {
                if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Add(lvl);
                    _context.SaveChanges();
                }
                RunAverageProcedure(lvl.id);
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmAveragelevel> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var lvl = _context.scmAveragelevel.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (lvl != null)
            {
                lvl.averagelevelId = value.Value.averagelevelId;
                lvl.isActive = value.Value.isActive;
                lvl.UserName = user.UserName;
                lvl.UpdateDate = DateTime.Now.Date;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(lvl).State = EntityState.Modified;

            try
            {
                if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Update(lvl);
                    _context.SaveChanges();
                }

                RunAverageProcedure(value.Value.id);
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

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove([FromBody]CRUDModel<scmAveragelevel> Value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmAveragelevel item = _context.scmAveragelevel.Where(m => m.id.Equals(id)).FirstOrDefault();
                if ((User.IsInRole("administrator") || User.IsInRole("unicef") || User.IsInRole("pnd")) && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.scmAveragelevel.Remove(item);
                    _context.SaveChanges();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.scmAveragelevel.Any(e => e.id == id);
        }

        public void RunAverageProcedure (int id)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("exec dbo.AdjustAverageLevel {0}", id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
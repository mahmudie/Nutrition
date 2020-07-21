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
    [Authorize(Policy ="admin")]
    public class whlevelsController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public whlevelsController(WebNutContext context, UserManager<ApplicationUser> userManager)
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
            else if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
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
            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.Lkpwhlevels.ToList();
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
            int count = DataSource.Cast<Lkpwhlevels>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<Lkpwhlevels> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Lkpwhlevels lvl = new Lkpwhlevels();
            if (lvl == null) { return BadRequest(); }

            lvl.LevelDescriptoin = value.Value.LevelDescriptoin;

            try
            {
                if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Add(lvl);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<Lkpwhlevels> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var lvl = _context.Lkpwhlevels.Where(cat => cat.LevelId == value.Value.LevelId).FirstOrDefault();
            if (lvl != null)
            {
                lvl.LevelDescriptoin = value.Value.LevelDescriptoin;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(lvl).State = EntityState.Modified;

            try
            {
                if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Update(lvl);
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.LevelId))
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

        public async Task<IActionResult> Remove([FromBody]CRUDModel<Lkpwhlevels> Value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                Lkpwhlevels item = _context.Lkpwhlevels.Where(m => m.LevelId.Equals(id)).FirstOrDefault();
                if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Lkpwhlevels.Remove(item);
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
            return _context.Lkpwhlevels.Any(e => e.LevelId == id);
        }
    }
}
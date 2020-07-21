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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class CategoriesController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CategoriesController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TlkpOtptfus
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
            var data = _context.LkpCategories.ToList();
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
            int count = DataSource.Cast<LkpCategory>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<LkpCategory> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LkpCategory categ = new LkpCategory();
            if (categ == null) { return BadRequest(); }

            categ.CategoryId = value.Value.CategoryId;
            categ.CategoryName = value.Value.CategoryName;

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
        public IActionResult Update([FromBody]CRUDModel<LkpCategory> model)
        {
            var data = _context.LkpCategories.Where(cat=>cat.CategoryId==model.Value.CategoryId).FirstOrDefault();
            if (data != null)
            {
                data.CategoryName = model.Value.CategoryName;
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

        public IActionResult Remove([FromBody]CRUDModel<LkpCategory> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if(Exists(id))
            {
                LkpCategory item = _context.LkpCategories.Where(m => m.CategoryId.Equals(id)).FirstOrDefault();
                _context.LkpCategories.Remove(item);
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
            return _context.LkpCategories.Any(e => e.CategoryId == id);
        }
    }
}

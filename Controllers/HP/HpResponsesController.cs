using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.HP;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace DataSystem.Controllers.SCM
{
    [Authorize(Roles = "administrator")]
    [Authorize(Policy = "admin")]
    public class HpResponsesController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HpResponsesController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.HpResponses.ToList();
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
            int count = DataSource.Cast<HpResponses>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<HpResponses> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HpResponses lkp = new HpResponses();
            if (lkp == null) { return BadRequest(); }

            lkp.ResponseName = value.Value.ResponseName;
            lkp.IsActive = value.Value.IsActive;

            try
            {
                if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Add(lkp);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<HpResponses> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var lkp = _context.HpResponses.Where(cat => cat.ResponseId == value.Value.ResponseId).FirstOrDefault();
            if (lkp != null)
            {
                lkp.ResponseName = value.Value.ResponseName;
                lkp.IsActive = value.Value.IsActive;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(lkp).State = EntityState.Modified;

            try
            {
                if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.Update(lkp);
                    _context.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(value.Value.ResponseId))
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

        public async Task<IActionResult> Remove([FromBody]CRUDModel<HpResponses> Value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                HpResponses item = _context.HpResponses.Where(m => m.ResponseId.Equals(id)).FirstOrDefault();
                if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.HpResponses.Remove(item);
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
            return _context.HpResponses.Any(e => e.ResponseId == id);
        }
    }
}
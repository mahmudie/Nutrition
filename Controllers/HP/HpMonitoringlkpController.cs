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
    public class HpMonitoringlkpController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HpMonitoringlkpController(WebNutContext context, UserManager<ApplicationUser> userManager)
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
            var data = _context.HpMonitoringlkp.ToList();
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
            int count = DataSource.Cast<HpMonitoringlkp>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<HpMonitoringlkp> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HpMonitoringlkp lkp = new HpMonitoringlkp();
            if (lkp == null) { return BadRequest(); }

            lkp.PartCode = value.Value.PartCode;
            lkp.Questionname = value.Value.Questionname;
            lkp.VerificationSource = value.Value.VerificationSource;
            lkp.PossibleReponse = value.Value.PossibleReponse;
            lkp.IsActive = value.Value.IsActive;
            lkp.Section = value.Value.Section;
            lkp.Comment = value.Value.Comment;

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
        public async Task<IActionResult> Update([FromBody]CRUDModel<HpMonitoringlkp> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var lkp = _context.HpMonitoringlkp.Where(cat => cat.Id == value.Value.Id).FirstOrDefault();
            if (lkp != null)
            {
                lkp.PartCode = value.Value.PartCode;
                lkp.Questionname = value.Value.Questionname;
                lkp.VerificationSource = value.Value.VerificationSource;
                lkp.PossibleReponse = value.Value.PossibleReponse;
                lkp.IsActive = value.Value.IsActive;
                lkp.Section = value.Value.Section;
                lkp.Comment = value.Value.Comment;
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
                if (!Exists(value.Value.Id))
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

        public async Task<IActionResult> Remove([FromBody]CRUDModel<HpMonitoringlkp> Value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                HpMonitoringlkp item = _context.HpMonitoringlkp.Where(m => m.Id.Equals(id)).FirstOrDefault();
                if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
                {
                    _context.HpMonitoringlkp.Remove(item);
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
            return _context.HpMonitoringlkp.Any(e => e.Id == id);
        }
    }
}
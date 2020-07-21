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
    [Authorize(Roles = "administrator")]
    public class scmwastagesController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmwastagesController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.scmWastages.ToList();
            if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.UserName == user.UserName).ToList();
            }
            else if ((user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                data = data.Where(m => m.UserName == user.UserName).ToList();
            }

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
            int count = DataSource.Cast<scmWastages>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmWastages> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmWastages wst = new scmWastages();
            if (wst == null) { return BadRequest(); }

            wst.IPDistributionId = int.Parse(value.Params["ID"].ToString()); 
            wst.WasteId = value.Value.WasteId;
            wst.DateWasted = value.Value.DateWasted;
            wst.Quantity = value.Value.Quantity;
            wst.Reason = value.Value.Reason;
            wst.ActionTaken = value.Value.ActionTaken;
            wst.TenantId = user.TenantId;
            wst.UserName = user.UserName;
            wst.UpdateDate = DateTime.Now.Date;
            
            try
            {
                if ( (user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Add(wst);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmWastages> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var wst = _context.scmWastages.Where(cat => cat.Id == value.Value.Id).FirstOrDefault();
            if (wst != null)
            {
                wst.IPDistributionId = int.Parse(value.Params["ID"].ToString());
                wst.WasteId = value.Value.WasteId;
                wst.DateWasted = value.Value.DateWasted;
                wst.Quantity = value.Value.Quantity;
                wst.Reason = value.Value.Reason;
                wst.ActionTaken = value.Value.ActionTaken;
                wst.TenantId = user.TenantId;
                wst.UserName = user.UserName;
                wst.UpdateDate = DateTime.Now.Date;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(wst).State = EntityState.Modified;

            try
            {
                if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.Update(wst);
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

        public async Task<IActionResult> Remove([FromBody]CRUDModel<scmWastages> Value)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmWastages item = _context.scmWastages.Where(m => m.Id.Equals(id)).FirstOrDefault();

                if ( (user.Unicef == 1 || user.Pnd == 1))
                {
                    return NoContent();
                }
                else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
                {
                    _context.scmWastages.Remove(item);
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
            return _context.scmWastages.Any(e => e.Id == id);
        }
    }
}
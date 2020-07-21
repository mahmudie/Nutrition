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
    public class scmNotifylistController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmNotifylistController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userslist = _context.scmUsersset.Where(w=> w.IsUnicefPnd.Equals(0)).Select(m => new
            {
                UserId = m.Id,
                UserName = m.UserName
            }).ToList();

            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();
            
            var imps = _context.Implementers.Select(m => new
            {
                ImpId = m.ImpCode,
                ImpName = m.ImpAcronym
            }).ToList();

            ViewBag.UsersSource = userslist;
            ViewBag.ProvinceSource = provinces;
            ViewBag.ImpSource = imps;

            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmNotificationlist.ToList();
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
            int count = DataSource.Cast<scmNotificationlist>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmNotificationlist> value)
        {
            var user = await _userManager.FindByNameAsync(value.Value.Username);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmNotificationlist notylist = new scmNotificationlist();
            if (notylist == null) { return BadRequest(); }

            notylist.Username = user.UserName;
            notylist.TenantId = user.TenantId;
            notylist.ProvinceId = value.Value.ProvinceId;
            notylist.ImpId = value.Value.ImpId;
            notylist.IsActive = value.Value.IsActive;
            try
            {
                _context.Add(notylist);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }

        public async Task<IActionResult> Update([FromBody]CRUDModel<scmNotificationlist> value)
        {
            var user = await _userManager.FindByNameAsync(value.Value.Username);
            var notylist = _context.scmNotificationlist.Where(cat => cat.Id == value.Value.Id).FirstOrDefault();
            if (notylist != null)
            {
                notylist.Username = user.UserName;
                notylist.TenantId = user.TenantId;
                notylist.ProvinceId = value.Value.ProvinceId;
                notylist.ImpId = value.Value.ImpId;
                notylist.IsActive = value.Value.IsActive;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(notylist).State = EntityState.Modified;

            try
            {
                _context.Update(notylist);
                _context.SaveChanges();
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

        public IActionResult Remove([FromBody]CRUDModel<scmNotificationlist> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmNotificationlist item = _context.scmNotificationlist.Where(m => m.Id.Equals(id)).FirstOrDefault();
                _context.scmNotificationlist.Remove(item);
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
            return _context.scmNotificationlist.Any(e => e.Id == id);
        }


        public ActionResult NotificationPage()
        {
            return RedirectToAction("Index", "Estnotification");
        }
    }
}
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

    public class ApistoreController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ApistoreController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: TlkpOtptfus
        [Authorize(Roles = "administrator,unicef,pnd")]
        [Authorize(Policy = "admin")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.Apistore.ToList();
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
            int count = DataSource.Cast<Apistore>().Count();
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

        [Authorize(Roles = "administrator,unicef,pnd")]
        [Authorize(Policy = "admin")]
        public IActionResult Insert([FromBody]CRUDModel<Apistore> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Apistore item = new Apistore();
            if (item == null) { return BadRequest(); }

            item.apiurl = value.Value.apiurl;
            item.filtervalue = value.Value.filtervalue;
            item.isActive = value.Value.isActive;
            item.lastDate = DateTime.Now.Date;

            try
            {
                _context.Add(item);
                 _context.SaveChanges();
            }
            catch(Exception ex)
            {

            }

            return NoContent();
        }
        [Authorize(Roles = "administrator,unicef,pnd")]
        [Authorize(Policy = "admin")]
        public IActionResult Update([FromBody]CRUDModel<Apistore> value)
        {
            var item = _context.Apistore.Where(cat=>cat.id==value.Value.id).FirstOrDefault();
            if (item != null)
            {
                item.apiurl = value.Value.apiurl;
                item.filtervalue = value.Value.filtervalue;
                item.isActive = value.Value.isActive;
                item.lastDate = DateTime.Now.Date;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            _context.Entry(item).State = EntityState.Modified;

            try
            {
                _context.Update(item);
                 _context.SaveChanges();
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

            return NoContent();
        }
        [Authorize(Roles = "administrator,unicef,pnd")]
        [Authorize(Policy = "admin")]
        public IActionResult Remove([FromBody]CRUDModel<Apistore> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if(Exists(id))
            {
                Apistore item = _context.Apistore.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.Apistore.Remove(item);
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
            return _context.Apistore.Any(e => e.id == id);
        }

        public IActionResult Api()
        {
            var data = _context.Apistore.ToList();
            return Ok(new { data = data });
        }
    }
}

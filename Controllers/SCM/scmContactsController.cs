using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    public class scmContactsController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmContactsController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmContacts.ToList();
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
            int count = DataSource.Cast<scmContacts>().Count();
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


        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmContacts> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int RequestId = int.Parse(value.Params["ID"].ToString());
            scmContacts item = new scmContacts();
            if (item == null) { return BadRequest(); }

            item.requestId = RequestId;
            item.position = value.Value.position;
            item.firstName = value.Value.firstName;
            item.lastName = value.Value.lastName;
            item.phone1 = value.Value.phone1;
            item.phone2 = value.Value.phone2;
            item.email = value.Value.email;
            item.userName = value.Value.userName;
            item.updateDate = DateTime.Now.Date;

            try
            {
                if (user.Unicef == 0 && user.Pnd == 0)
                {
                    _context.Add(item);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }

        public async Task<IActionResult> Update([FromBody]CRUDModel<scmContacts> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);


            var item = _context.scmContacts.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (item != null)
            {

                item.requestId = value.Value.requestId;
                item.position = value.Value.position;
                item.firstName = value.Value.firstName;
                item.lastName = value.Value.lastName;
                item.phone1 = value.Value.phone1;
                item.phone2 = value.Value.phone2;
                item.email = value.Value.email;
                item.userName = value.Value.userName;
                item.updateDate = DateTime.Now.Date;
            }

            _context.Entry(item).State = EntityState.Modified;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (user.Unicef == 0 && user.Pnd == 0)
                {
                    _context.Update(item);
                    _context.SaveChanges();
                }
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

        public IActionResult Remove([FromBody]CRUDModel<scmContacts> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmContacts item = _context.scmContacts.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmContacts.Remove(item);
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
            return _context.scmContacts.Any(e => e.id == id);
        }
        //Delete whole Request - GridEight
        public async Task<IActionResult> AddContacts(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            AddUpdateContacts(id, user.UserName);
            return Ok();
        }

        // Method to add contacts details - GridEight
        public void AddUpdateContacts(int RequestId, string UserName)
        {
            try
            {
                _context.Database.ExecuteSqlCommand("exec dbo.AddUpdateContacts {0},{1}", RequestId, UserName);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
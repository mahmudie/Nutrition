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
    [Authorize(Roles = "dataentry,administrator")]
    [Authorize(Policy = "admin")]
    
    public class emailController : Controller
    {
        private readonly WebNutContext _context;
        public emailController(WebNutContext context)
        {
            _context = context;    
        }

        // GET: TlkpOtptfus
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.masteremails.ToList();
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
            int count = DataSource.Cast<masteremails>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<masteremails> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            masteremails email = new masteremails();
            if (email == null) { return BadRequest(); }

            email.emailaccount = value.Value.emailaccount;
            email.smtp = value.Value.smtp;
            email.port = value.Value.port;
            email.ssl = value.Value.ssl;
            email.issender = value.Value.issender;
            email.isactive = value.Value.isactive;

            try
            {
                _context.Add(email);
                 _context.SaveChanges();
            }
            catch(Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<masteremails> model)
        {
            var data = _context.masteremails.Where(cat=>cat.Id==model.Value.Id).FirstOrDefault();
            if (data != null)
            {
                data.emailaccount = model.Value.emailaccount;
                data.smtp = model.Value.smtp;
                data.port = model.Value.port;
                data.ssl = model.Value.ssl;
                data.issender = model.Value.issender;
                data.isactive = model.Value.isactive;
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
                if (!Exists(model.Value.Id))
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

        public IActionResult Remove([FromBody]CRUDModel<masteremails> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if(Exists(id))
            {
                masteremails item = _context.masteremails.Where(m => m.Id.Equals(id)).FirstOrDefault();
                _context.masteremails.Remove(item);
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
            return _context.masteremails.Any(e => e.Id == id);
        }
    }
}

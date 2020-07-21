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
    [Authorize(Roles = "administrator,unicef,pnd")]
    [Authorize(Policy = "admin")]
    public class scmDocsController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmDocsController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var doctypes = _context.scmDoctypes.Select(m => new
            {
                DocId = m.DocId,
                DocumentType = m.DocumentType
            }).ToList();

            ViewBag.DocSource = doctypes;
            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmDocs.ToList();
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
            int count = DataSource.Cast<scmDocs>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmDocs> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmDocs docs = new scmDocs();
            if (docs == null) { return BadRequest(); }
                docs.distributionId = int.Parse(value.Params["ID"].ToString()); ;
                docs.documentName = value.Value.documentName;
                docs.message = value.Value.message;
                docs.dateSent = DateTime.Now;
                docs.updateDate = value.Value.updateDate;
                docs.userName = Crrentuser.UserName;

            try
            {
                _context.Add(docs);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }

        public async Task<IActionResult> Update([FromBody]CRUDModel<scmDocs> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            
            var docs = _context.scmDocs.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (docs != null)
            {
                docs.distributionId = int.Parse(value.Params["ID"].ToString());
                docs.documentName = value.Value.documentName;
                docs.message = value.Value.message;
                docs.updateDate = value.Value.updateDate;
                docs.userName = Crrentuser.UserName;
            }
            _context.Entry(docs).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Update(docs);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocExists(value.Value.id))
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

        public IActionResult Remove([FromBody]CRUDModel<scmDocs> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (DocExists(id))
            {
                scmDocs item = _context.scmDocs.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmDocs.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }


            return NoContent();
        }

        private bool DocExists(int id)
        {
            return _context.scmDocs.Any(e => e.id == id);
        }
    }
}
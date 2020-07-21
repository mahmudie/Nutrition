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
    [Authorize(Policy ="admin")]
    public class AppynotifyController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppynotifyController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //public IActionResult Index()
        //{
        //    var rounds = _context.scmRounds.Select(m => new
        //    {
        //        RoundId = m.RoundId,
        //        RoundName=m.RoundDescription
        //    }).ToList();

        //    var imps = _context.Implementers.Select(m => new
        //    {
        //        ImpId = m.ImpCode,
        //        ImpName = m.ImpAcronym
        //    }).ToList();

        //    var provinces = _context.Provinces.Select(m => new
        //    {
        //        ProvCode = m.ProvCode,
        //        ProvName = m.ProvName
        //    }).ToList();


        //    //Child table
        //    var childtable = _context.scmEstNotification.ToList();

        //    ViewBag.RoudSource = rounds;
        //    ViewBag.ImpSource = imps;
        //    ViewBag.ProvinceSource = provinces;
        //    ViewBag.dataSource = childtable;


        //    return View();
        //}

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.vmEstNotification.ToList();
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
            int count = DataSource.Cast<vmEstNotification>().Count();
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

        //public async Task<IActionResult> Insert([FromBody]CRUDModel<scmEstsubmission> value)
        //{

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);


        //    scmEstsubmission estsub = new scmEstsubmission();
        //    if (estsub == null) { return BadRequest(); }

        //    estsub.roundId = value.Value.roundId;
        //    estsub.startDate = value.Value.startDate;
        //    estsub.deadlineDate = value.Value.deadlineDate;
        //    estsub.userName = user.UserName;
        //    estsub.updateDate = DateTime.Now;
        //    estsub.tenantId = user.TenantId;

        //    try
        //    {
        //        _context.Add(estsub);
        //        _context.SaveChanges();
        //        _context.Database.ExecuteSqlCommand("exec dbo.Add_To_scmEstNotification {0}", estsub.id);

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return NoContent();
        //}


        //public async Task<IActionResult> Update([FromBody]CRUDModel<scmEstsubmission> value)
        //{
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);

        //    var estsub = _context.scmEstsubmission.Where(cat => cat.id == value.Value.id).FirstOrDefault();
        //    if (estsub != null)
        //    {
        //        estsub.roundId = value.Value.roundId;
        //        estsub.startDate = value.Value.startDate;
        //        estsub.deadlineDate = value.Value.deadlineDate;
        //        estsub.completed = value.Value.completed;
        //        estsub.datecompleted = value.Value.datecompleted;
        //        estsub.userName = user.UserName;
        //        estsub.updateDate = DateTime.Now;
        //        estsub.tenantId = user.TenantId;
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    _context.Entry(estsub).State = EntityState.Modified;

        //    try
        //    {
        //        _context.Update(estsub);
        //        _context.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!Exists(value.Value.id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //public IActionResult Remove([FromBody]CRUDModel<scmEstsubmission> Value)
        //{
        //    Int64 getId = (Int64)Value.Key;
        //    int id = (int)getId;
        //    if (Exists(id))
        //    {
        //        scmEstsubmission item = _context.scmEstsubmission.Where(m => m.id.Equals(id)).FirstOrDefault();
        //        _context.scmEstsubmission.Remove(item);
        //        _context.SaveChanges();
        //    }
        //    else
        //    {
        //        return BadRequest(ModelState);
        //    }


        //    return NoContent();
        //}

        private bool Exists(int id)
        {
            return _context.scmEstsubmission.Any(e => e.id == id);
        }

        private bool Existssub(int id)
        {
            return _context.scmEstNotification.Any(e => e.id == id);
        }
    }
}
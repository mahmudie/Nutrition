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
    public class scmDistributionFacilitiesController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmDistributionFacilitiesController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            ViewBag.gridAdd = false;
            ViewBag.gridEdit = true;
            ViewBag.gridDelete = false;
            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmDistributionFacilities.ToList();
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
            int count = DataSource.Cast<scmDistributionFacilities>().Count();
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

        public async Task<IActionResult> Insert([FromBody]CRUDModel<scmDistributionFacilities> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmDistributionFacilities dist = new scmDistributionFacilities();
            if (dist == null) { return BadRequest(); }

            dist.ipdistributionId = value.Value.ipdistributionId;
            dist.supplyId = value.Value.supplyId;
            dist.facilityId = value.Value.facilityId;
            dist.facilityTypeId = value.Value.facilityTypeId;
            dist.estimation = value.Value.estimation;
            dist.distribution = value.Value.distribution;
            dist.distributionDate = value.Value.distributionDate;
            dist.program = value.Value.program;
            dist.userName = user.UserName;
            dist.tenantId = user.TenantId;
            dist.updateDate = DateTime.Now ;
            dist.distributionb = value.Value.distributionb;
            dist.distributionbDate = value.Value.distributionbDate;

            try
            {
                _context.Add(dist);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public async Task<IActionResult> Update([FromBody]CRUDModel<scmDistributionFacilities> value)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var dist = _context.scmDistributionFacilities.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (dist != null)
            {
                dist.ipdistributionId = value.Value.ipdistributionId;
                dist.supplyId = value.Value.supplyId;
                dist.facilityId = value.Value.facilityId;
                dist.facilityTypeId = value.Value.facilityTypeId;
                dist.estimation = value.Value.estimation;
                dist.distribution = value.Value.distribution;
                dist.distributionDate = value.Value.distributionDate;
                dist.program = value.Value.program;
                dist.userName = user.UserName;
                dist.tenantId = user.TenantId;
                dist.updateDate = DateTime.Now;
                dist.distributionb = value.Value.distributionb;
                dist.distributionbDate = value.Value.distributionbDate;
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(dist).State = EntityState.Modified;

            try
            {
                _context.Update(dist);
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

        public IActionResult Remove([FromBody]CRUDModel<scmDistributionFacilities> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmDistributionFacilities item = _context.scmDistributionFacilities.Where(m => m.id.Equals(id)).FirstOrDefault();
                _context.scmDistributionFacilities.Remove(item);
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
            return _context.scmDistributionFacilities.Any(e => e.id == id);
        }

        public async Task<IActionResult> BatchUpdate([FromBody]CRUDModel value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (value.Changed != null)
            {
                for (var i = 0; i < value.Changed.Count(); i++)
                {
                    try
                    {
                        var req = value.Changed[i];
                        scmDistributionFacilities hfreqs = _context.scmDistributionFacilities.Where(or => or.id == req.id && or.userName==Crrentuser.UserName).FirstOrDefault();
                        hfreqs.facilityId = req.facilityId;
                        hfreqs.supplyId = req.supplyId;
                        hfreqs.facilityTypeId = req.facilityTypeId;
                        hfreqs.facilityTypeId = req.facilityTypeId;
                        hfreqs.estimation = req.estimation;
                        hfreqs.distribution = req.distribution;
                        hfreqs.distributionDate = req.distributionDate;
                        hfreqs.program = req.program;
                        hfreqs.ipdistributionId = req.ipdistributionId;
                        hfreqs.tenantId = Crrentuser.TenantId;
                        hfreqs.updateDate = DateTime.Now.Date;
                        hfreqs.userName = Crrentuser.UserName;
                        hfreqs.distributionb = value.Value.distributionb;
                        hfreqs.distributionbDate = value.Value.distributionbDate;
                        _context.Update(hfreqs);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }
            if (value.Deleted != null)
            {
                for (var i = 0; i < value.Deleted.Count(); i++)
                {
                    _context.scmDistributionFacilities.Remove(_context.scmDistributionFacilities.Where(or => or.id == value.Deleted[i].id).FirstOrDefault());
                    _context.SaveChanges();
                }
            }
            if (value.Added != null)
            {
                for (var i = 0; i < value.Added.Count(); i++)
                {
                    _context.scmDistributionFacilities.Add(value.Added[i]);
                    _context.SaveChanges();
                }
            }
            var data = _context.scmDistributionFacilities.ToList();
            return Json(data);
        }

        public class CRUDModel
        {
            public List<scmDistributionFacilities> Added { get; set; }
            public List<scmDistributionFacilities> Changed { get; set; }
            public List<scmDistributionFacilities> Deleted { get; set; }
            public scmDistributionFacilities Value { get; set; }
            public int key { get; set; }
            public string action { get; set; }
        }
    }
}
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
    public class scmpocController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmpocController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var facility = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                FacilityName =  m.FacilityName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                DistrictName = m.DistName
            }).ToList();


            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();

            var facilitytypes = _context.FacilityTypes.Select(m => new
            {
                FacilityTypeId = m.FacTypeCode,
                FacilityTypeName = m.TypeAbbrv
            }).ToList();

            ViewBag.FacilitySource = facility;
            ViewBag.DistrictSource = districts;
            ViewBag.ProvinceSource = provinces;
            ViewBag.FacilityTypeSource = facilitytypes;

            if (User.Identity.IsAuthenticated & User.IsInRole("dataentry"))
            {
                ViewBag.gridAdd = false;
                ViewBag.gridEdit = false;
                ViewBag.gridDelete = false;
            }
            else if ((user.Unicef == 1 || user.Pnd == 1))
            {
                ViewBag.gridAdd = false;
                ViewBag.gridEdit = false;
                ViewBag.gridDelete = false;
            }
            else if ((user.Unicef == 0 && user.Pnd == 0))
            {
                ViewBag.gridAdd = true;
                ViewBag.gridEdit = true;
                ViewBag.gridDelete = true;
            }

            return View();
        }

        public async Task<IActionResult> Facils()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int TenantId;
            string ProvinceId;

            TenantId = user.TenantId;

            var tenants = _context.Tenants.Where(m => m.Id == TenantId).FirstOrDefault();
            var provinces = _context.Provinces.Where(w => w.ProvName.Equals(tenants.Name)).FirstOrDefault();
            ProvinceId = provinces.ProvCode;

            var data = _context.FacilityInfo.Include(m => m.DistNavigation).Where(m => m.DistNavigation.ProvCode ==ProvinceId).Select(n => new
            {
                FacilityId = n.FacilityId,
                FacilityName = n.FacilityName,
                FacilityType =n.FacilityTypeNavigation.FacType,
                District =n.DistNavigation.DistName,
                Province=n.DistNavigation.ProvCodeNavigation.ProvName,
                Implementer =n.Implementer
            }).ToList();

            ViewBag.dataSource = data;
            return View();
        }

        public async Task<IActionResult> updateData([FromBody]List<FacilitViewModel> myObject)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int TenantId;
            string ProvinceId;

            TenantId = user.TenantId;

            var tenants = _context.Tenants.Where(m => m.Id == TenantId).FirstOrDefault();
            var provinces = _context.Provinces.Where(w => w.ProvName.Equals(tenants.Name)).FirstOrDefault();
            ProvinceId = provinces.ProvCode;
            try
            {
                foreach (var temp in myObject)
                {
                    var districtId = _context.FacilityInfo.Where(m => m.FacilityId.Equals(temp.FacilityId)).FirstOrDefault();
                    var rmb = _context.ScmPOCs.Where(m => m.FacilityId == temp.FacilityId & m.TenantId == TenantId & m.ProvinceId == ProvinceId).FirstOrDefault();
                    if (rmb == null)
                    {
                        scmPOC item = new scmPOC();
                        item.FacilityId = temp.FacilityId;
                        item.ProvinceId = ProvinceId;
                        item.UpdateDate = DateTime.Now;
                        item.UserName = user.UserName;
                        item.FacilityTypeId =(int)districtId.FacilityType;
                        item.TenantId = TenantId;
                        item.DistrictId = districtId.DistCode;

                        _context.Add(item);
                        _context.SaveChanges();
                    }

                }
                
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("Index","scmpoc");
        }


        // CRUD FOR scm POC
        public async Task <IActionResult> UrlDatasource([FromBody]DataManagerRequest dm)
        {
           
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.ScmPOCs.ToList();
            try
            {
                if (User.IsInRole("administrator"))
                {
                    data = data.Where(m => m.UserName == user.UserName).ToList();
                }
                else if ((user.Unicef == 1 || user.Pnd == 1))
                {
                    data = data.ToList();
                }
                else
                {
                    data = data.Where(m => m.TenantId == user.TenantId).ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
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
            int count = DataSource.Cast<scmPOC>().Count();
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

        public IActionResult Insert([FromBody]CRUDModel<scmPOC> value)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            scmPOC categ = new scmPOC();
            if (categ == null) { return BadRequest(); }

            categ.FacilityId = value.Value.FacilityId;

            try
            {
                _context.Add(categ);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return NoContent();
        }
        public IActionResult Update([FromBody]CRUDModel<scmPOC> model)
        {
            var data = _context.ScmPOCs.Where(cat => cat.PocId == model.Value.PocId).FirstOrDefault();
            if (data != null)
            {
                data.FacilityId = model.Value.FacilityId;
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
                if (!Exists(model.Value.PocId))
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

        public IActionResult Remove([FromBody]CRUDModel<scmPOC> Value)
        {
            Int64 getId = (Int64)Value.Key;
            int id = (int)getId;
            if (Exists(id))
            {
                scmPOC item = _context.ScmPOCs.Where(m => m.PocId.Equals(id)).FirstOrDefault();
                _context.ScmPOCs.Remove(item);
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
            return _context.ScmPOCs.Any(e => e.PocId == id);
        }
    }
}
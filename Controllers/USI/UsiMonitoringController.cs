using DataSystem.Models;
using DataSystem.Models.HP;
using DataSystem.Models.USI;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Navigations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry, administrator")]
    public class UsiMonitoringController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsiMonitoringController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public class Months
        {
            public int MonthId { get; set; }
            public int Month { get; set; }

            public List<Months> MonthList()
            {
                List<Months> endmonth = new List<Months>();
                endmonth.Add(new Months() { MonthId = 1, Month = 1 });
                endmonth.Add(new Months() { MonthId = 2, Month = 2 });
                endmonth.Add(new Months() { MonthId = 3, Month = 3 });
                endmonth.Add(new Months() { MonthId = 4, Month = 4 });
                endmonth.Add(new Months() { MonthId = 5, Month = 5 });
                endmonth.Add(new Months() { MonthId = 6, Month = 6 });
                endmonth.Add(new Months() { MonthId = 7, Month = 7 });
                endmonth.Add(new Months() { MonthId = 8, Month = 8 });
                endmonth.Add(new Months() { MonthId = 9, Month = 9 });
                endmonth.Add(new Months() { MonthId = 10, Month = 10 });
                endmonth.Add(new Months() { MonthId = 11, Month = 11 });
                endmonth.Add(new Months() { MonthId = 12, Month = 12 });
                return endmonth;
            }
        }

        public IActionResult Index()
        {
            if (User.IsInRole("dataentry"))
            {
                ViewBag.edit = true;
            }
            else
            {
                ViewBag.edit = false;
            }
                var data = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();

            var years = _context.Yearlist.Select(m => new
            {
                YearId = m.YearId,
                YearName = m.YearName
            }).ToList();

            Months months = new Months();

            var lookup = _context.Usilookup.Select(m => new
            {
                IndicatorId = m.id,
                IndicatorName = m.indicatorName
            }).ToList();

            ViewBag.ProvinceSource = data.ToList();
            ViewBag.MonthSource = months.MonthList().ToList();
            ViewBag.YearSource = years.ToList();
            ViewBag.LookupSource = lookup.ToList();


            return View();
        }

        public IActionResult Indicators()
        {
            return View();
        }

        public async Task<IActionResult> UsiMonitoringSource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.UsiMonitoring.ToList();
            if (User.IsInRole("dataentry"))
            {
                data = data.Where(m => m.userName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef != 1 || user.Pnd != 1))
            {
                data = data.Where(m => m.tenantId == user.TenantId).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 1 || user.Pnd == 1))
            {
                data = data.ToList();
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
            int count = DataSource.Cast<UsiMonitoring>().Count();
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

        public async Task<IActionResult> UsiMonitoringInsert([FromBody]CRUDModel<UsiMonitoring> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UsiMonitoring items = new UsiMonitoring();
            if (items == null) { return BadRequest(); }
            items.provinceId = value.Value.provinceId;
            items.year = value.Value.year;
            items.month = value.Value.month;
            items.tenantId = Crrentuser.TenantId;
            items.updateDate = DateTime.Now;
            items.userName = Crrentuser.UserName;

            try
            {
                if (User.IsInRole("dataentry"))
                {
                    _context.Add(items);
                    _context.SaveChanges();

                    appendIndicators(items.usiId);
                }
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }

        public async Task<IActionResult> UsiMonitoringUpdate([FromBody]CRUDModel<UsiMonitoring> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.UsiMonitoring.Where(cat => cat.usiId == value.Value.usiId).FirstOrDefault();
            if (items != null)
            {
                items.provinceId = value.Value.provinceId;
                items.year = value.Value.year;
                items.month = value.Value.month;
                items.tenantId = Crrentuser.TenantId;
                items.updateDate = DateTime.Now;
                items.userName = Crrentuser.UserName;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (User.IsInRole("dataentry"))
                {
                    _context.Update(items);
                    _context.SaveChanges();

                    appendIndicators(items.usiId);
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public IActionResult UsiMonitoringRemove([FromBody]CRUDModel<UsiMonitoring> Value)
        {
            if (UsidMonitoringExists(Value.Value.usiId))
            {
                UsiMonitoring item = _context.UsiMonitoring.Where(m => m.usiId.Equals(Value.Value.usiId)).FirstOrDefault();
                _context.UsiMonitoring.Remove(item);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        public void appendIndicators(int id)
        {
            DateTime updateDate;
            var user = _userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    updateDate = DateTime.Now;
                    _context.Database.ExecuteSqlCommand("exec dbo.AddUsiIndicators {0}", id);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // Creating Sub forms
        // HpScreening
        public async Task<IActionResult> IndicatorsSource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.Usiindicators.ToList();
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
            int count = DataSource.Cast<Usiindicators>().Count();
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

        public async Task<IActionResult> IndicatorsInsert([FromBody]CRUDModel<Usiindicators> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Usiindicators items = new Usiindicators();
            if (items == null) { return BadRequest(); }
            items.indicatorId = value.Value.indicatorId;
            items.value = value.Value.value;
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    _context.Add(items);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }

        public async Task<IActionResult> IndicatorsUpdate([FromBody]CRUDModel<Usiindicators> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.Usiindicators.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (items != null)
            {
                items.usiId = value.Value.usiId;
                items.indicatorId = value.Value.indicatorId;
                items.value = value.Value.value;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (User.IsInRole("dataentry"))
                {
                    _context.Update(items);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public IActionResult IndicatorsRemove([FromBody]CRUDModel<Usiindicators> Value)
        {
            if (IndicatorsExists(Value.Value.id))
            {
                if (User.IsInRole("dataentry"))
                {
                    Usiindicators item = _context.Usiindicators.Where(m => m.id.Equals(Value.Value.id)).FirstOrDefault();
                    _context.Usiindicators.Remove(item);
                    _context.SaveChanges();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }


        public async Task<IActionResult> LookupSource([FromBody]DataManagerRequest dm)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var data = _context.Usilookup.ToList();
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
            int count = DataSource.Cast<Usilookup>().Count();
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
        public async Task<IActionResult> LookupInsert([FromBody]CRUDModel<Usilookup> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Usilookup items = new Usilookup();
            if (items == null) { return BadRequest(); }
            items.indicatorName = value.Value.indicatorName;
            items.isActive = value.Value.isActive;
            try
            {
                if (User.IsInRole("dataentry"))
                {
                    _context.Add(items);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
            return NoContent();
        }
        public async Task<IActionResult> LookupUpdate([FromBody]CRUDModel<Usilookup> value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.Usilookup.Where(cat => cat.id == value.Value.id).FirstOrDefault();
            if (items != null)
            {
                items.indicatorName = value.Value.indicatorName;
                items.isActive = value.Value.isActive;
            }
            _context.Entry(items).State = EntityState.Modified;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (User.IsInRole("dataentry"))
                {
                    _context.Update(items);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }

            return NoContent();
        }

        public IActionResult LookupRemove([FromBody]CRUDModel<Usilookup> Value)
        {
            if (LookupExists(Value.Value.id))
            {
                if (User.IsInRole("dataentry"))
                {
                    Usilookup item = _context.Usilookup.Where(m => m.id.Equals(Value.Value.id)).FirstOrDefault();
                    _context.Usilookup.Remove(item);
                    _context.SaveChanges();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }

            return NoContent();
        }
        private bool UsidMonitoringExists(int id)
        {
            return _context.UsiMonitoring.Any(e => e.usiId == id);
        }
        private bool IndicatorsExists(int id)
        {
            return _context.Usiindicators.Any(e => e.id == id);
        }
        private bool LookupExists(int id)
        {
            return _context.Usilookup.Any(e => e.id == id);
        }
    }
}
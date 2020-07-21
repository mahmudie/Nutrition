using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using System.Collections.Generic;
using DataSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DataSystem.Controllers
{
    [Authorize(Roles = "dataentry,administrator")]
    public class QnrController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QnrController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Index()
        {
            var data = await _context.TblQnr.Where(m => m.UserName == User.Identity.Name)
                .Select(m => new qnrDto()
                {
                    Qnrid = m.Qnrid,
                    Province = m.ProvNavigation.ProvName,
                    Implementer = m.ImpNavigation.ImpName,
                    ReportMonth = m.ReportMonth,
                    ReportYear = m.ReportYear,
                    Updated = m.UpdateDate,
                    ReportingDate = m.ReportingDate,
                    status = m.StatusId,
                    Message = m.message

                }).OrderByDescending(m => m.Updated).ToListAsync();
            return View(data);
        }


        [Authorize(Roles = "administrator")]
        public IActionResult adminView()
        {

            return View();
        }

        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> admindata(IDataTablesRequest request)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<TblQnr> query = _context.TblQnr;
            if (user.TenantId != 1)
            {
                query = _context.TblQnr.Where(m => m.Tenant.Equals(user.TenantId));
            }

            var data = query.Select(m => new qnrDto()
            {
                Qnrid = m.Qnrid,
                Province = m.ProvNavigation.ProvName,
                Implementer = m.ImpNavigation.ImpName,
                ReportMonth = m.ReportMonth,
                ReportYear = m.ReportYear,
                Updated = m.UpdateDate,
                ReportingDate = m.ReportingDate,
                status = m.StatusId,
                Message = m.message

            }).OrderByDescending(m => m.Updated).ToList();

            List<qnrDto> filteredData = new List<qnrDto>();
            if (String.IsNullOrWhiteSpace(request.Search.Value))
            {
                filteredData = data;
            }
            else
            {
                int a;
                int y;
                bool result = int.TryParse(request.Search.Value, out a);
                if (!request.Search.Value.Contains("/"))
                {
                    if (result)
                    {
                        filteredData = data.Where(_item => _item.ReportMonth == a || _item.ReportYear == a).ToList();
                    }
                    else
                    {
                        string text = request.Search.Value.Trim().ToLower();
                        filteredData = data.Where(_item => _item.Implementer != null && _item.Implementer.ToLower().Contains(text)).ToList();
                    }
                }
                else if (request.Search.Value.Contains("/"))
                {
                    string search = request.Search.Value.Trim();
                    string[] words = search.Split('/');
                    int.TryParse(words[0], out y);
                    int.TryParse(words[1], out a);
                    filteredData = data.Where(_item => _item.ReportMonth == a && _item.ReportYear == y).ToList();
                }
                else
                {
                    filteredData = data;
                }

            }

            var dataPage = filteredData.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);
            return new DataTablesJsonResult(response, true);

        }
        [Authorize(Roles = "administrator")]
        [HttpGet]
        public async Task<IActionResult> Verify(int id)
        {


            var Qnr = await _context.TblQnr.Where(m => m.Qnrid == id).Select(m => new QnrReview()
            {
                Qnrid = m.Qnrid,
                Highlights = m.Highlights,
                IpdsamAdmissionsTrend = m.IpdsamAdmissionsTrend,
                IpdsamPerformanceTrend = m.IpdsamPerformanceTrend,
                OpdsamAdmissionsTrend = m.OpdsamAdmissionsTrend,
                OpdsamPerformanceTrend = m.OpdsamPerformanceTrend,
                OpdmamAdmissionsTrend = m.OpdmamAdmissionsTrend,
                OpdmamPerformanceTrend = m.OpdmamPerformanceTrend,
                Iycf = m.Iycf,
                Micronutrients = m.Micronutrients,
                Province = m.ProvNavigation.ProvName,
                Month = m.ReportMonth,
                Year = m.ReportYear,
                Implementer = m.ImpNavigation.ImpName,
                StatusId = m.StatusId,
                message = m.message

            }).SingleOrDefaultAsync();

            return View(Qnr);
        }


        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyQnr([Bind("Qnrid,message,StatusId")] ReviewViewModel nmr)
        {


            var Qnr = await _context.TblQnr.SingleOrDefaultAsync(m => m.Qnrid == nmr.Qnrid);
            if (rowStat(Qnr) == false)
            {
                return BadRequest();
            }
            Qnr.StatusId = nmr.StatusId;
            Qnr.message = nmr.message;
            // Qnr.message=nmr.message;
            await _context.SaveChangesAsync();

            return RedirectToAction("adminview");
        }
        [Authorize(Roles = "dataentry")]
        public IActionResult Create()
        {
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            }).ToList();
            items.Insert(0, new { ProvCode = "0", description = "select" });
            ViewData["province"] = new SelectList(items, "ProvCode", "description");
            ViewData["imp"] = new SelectList(_context.Implementers.Where(m => m.ImpAcronym != null), "ImpCode", "ImpAcronym");
            return View();
        }

        // POST: Qnr/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Create([Bind("Highlights,Implementer,IpdsamAdmissionsTrend,IpdsamPerformanceTrend,Iycf,Micronutrients,OpdmamAdmissionsTrend,OpdmamPerformanceTrend,OpdsamAdmissionsTrend,OpdsamPerformanceTrend,Province,Month,Year")] qnrViewModel item)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                TblQnr newReport = new TblQnr();
                newReport.ReportingDate = DateTime.Now;
                newReport.UserName = User.Identity.Name;
                newReport.StatusId = 1;
                newReport.ReportMonth = item.Month;
                newReport.ReportYear = item.Year;
                newReport.UserName = User.Identity.Name;
                newReport.Highlights = item.Highlights;
                newReport.Implementer = item.Implementer;
                newReport.IpdsamAdmissionsTrend = item.IpdsamAdmissionsTrend;
                newReport.IpdsamPerformanceTrend = item.IpdsamPerformanceTrend;
                newReport.OpdsamAdmissionsTrend = item.OpdsamAdmissionsTrend;
                newReport.OpdsamPerformanceTrend = item.OpdsamPerformanceTrend;
                newReport.OpdmamAdmissionsTrend = item.OpdmamAdmissionsTrend;
                newReport.OpdmamPerformanceTrend = item.OpdmamPerformanceTrend;
                newReport.Iycf = item.Iycf;
                newReport.Micronutrients = item.Micronutrients;
                newReport.Province = item.Province;
                newReport.Tenant=user.TenantId;

                _context.TblQnr.Add(newReport);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            });
            ViewData["province"] = new SelectList(items, "ProvCode", "description");
            ViewData["imp"] = new SelectList(_context.Implementers.Where(m => m.ImpAcronym != null), "ImpCode", "ImpAcronym");
            return View(item);
        }

        // GET: Qnr/Edit/5
        public async Task<IActionResult> Edit(int id)
        {


            var tblQnr = await _context.TblQnr.SingleOrDefaultAsync(m => m.Qnrid == id);
            if (rowStat(tblQnr) == false || tblQnr.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            }).ToList();
            items.Insert(0, new { ProvCode = "0", description = "select" });
            ViewData["province"] = new SelectList(items, "ProvCode", "description");
            ViewData["imp"] = new SelectList(_context.Implementers.Where(m => m.ImpAcronym != null), "ImpCode", "ImpAcronym");
            return View(tblQnr);
        }

        // POST: Qnr/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Qnrid,FacilityId,Highlights,Implementer,IpdsamAdmissionsTrend,IpdsamPerformanceTrend,Iycf,Micronutrients,OpdmamAdmissionsTrend,OpdmamPerformanceTrend,OpdsamAdmissionsTrend,OpdsamPerformanceTrend,ReportMonth,ReportYear")] TblQnr item)
        {

            var qnr = await _context.TblQnr.SingleOrDefaultAsync(m => m.Qnrid == id);
            if (rowStat(qnr) == false || qnr.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

                try
                {
                    qnr.UpdateDate = DateTime.Now;
                    qnr.UserName = User.Identity.Name;
                    qnr.StatusId = 2;
                    qnr.ReportMonth = item.ReportMonth;
                    qnr.ReportYear = item.ReportYear;
                    qnr.Implementer = item.Implementer;
                    qnr.StatusId = 2;
                    qnr.Highlights = item.Highlights;
                    qnr.Implementer = item.Implementer;
                    qnr.IpdsamAdmissionsTrend = item.IpdsamAdmissionsTrend;
                    qnr.IpdsamPerformanceTrend = item.IpdsamPerformanceTrend;
                    qnr.OpdsamAdmissionsTrend = item.OpdsamAdmissionsTrend;
                    qnr.OpdsamPerformanceTrend = item.OpdsamPerformanceTrend;
                    qnr.OpdmamAdmissionsTrend = item.OpdmamAdmissionsTrend;
                    qnr.OpdmamPerformanceTrend = item.OpdmamPerformanceTrend;
                    qnr.Iycf = item.Iycf;
                    qnr.Micronutrients = item.Micronutrients;
                    _context.Update(qnr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblQnrExists(qnr.Qnrid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            var items = _context.Provinces.Select(s => new
            {
                ProvCode = s.ProvCode,
                description = string.Format("{0} - {1}", s.ProvCode, s.ProvName)
            }).ToList();
            items.Insert(0, new { ProvCode = "0", description = "select" });
            ViewData["province"] = new SelectList(items, "ProvCode", "description");
            ViewData["imp"] = new SelectList(_context.Implementers.Where(m => m.ImpAcronym != null), "ImpAcronym", "ImpAcronym");
            return View(qnr);
        }

        // GET: Qnr/Delete/5
        public async Task<IActionResult> Delete(int id)
        {


            var item = await _context.TblQnr.SingleOrDefaultAsync(m => m.Qnrid == id);
            if (rowStat(item) == false || item.UserName != User.Identity.Name)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Qnr/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.TblQnr.SingleOrDefaultAsync(m => m.Qnrid == id);
            if (rowStat(item) == false || item.UserName != User.Identity.Name)
            {
                return NotFound();
            }
            _context.TblQnr.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TblQnrExists(int id)
        {
            return _context.TblQnr.Any(e => e.Qnrid == id);
        }

        private bool rowStat(TblQnr item)
        {
            if (item == null || item.StatusId == 3)
            {
                return false;
            }
            return true;
        }
    }
}

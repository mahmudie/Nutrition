using DataSystem.Models;
using DataSystem.Models.ViewModels;
using DataTables.AspNet.AspNetCore;
using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Samreq.Where(m => m.UserName == User.Identity.Name);
            return View(await myDbContext.ToListAsync());
        }

        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> PageData(IDataTablesRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            IQueryable<Samreq> query = _context.Samreq;
            if (user.TenantId != 1)
            {
                query = _context.Samreq.Where(m => m.Tenant.Equals(user.TenantId));
            }
            var data = query.Select(m => new smVm()
            {
                Rid = m.Rid,
                ProvCode = m.ProvCode,
                ImpCode = m.ImpCode,
                Year = m.Year,
                Month = m.Month,
                ReqBy = m.ReqBy,
                MonthFrom = m.MonthFrom,
                YearFrom = m.YearFrom,
                MonthTo = m.MonthTo,
                YearTo = m.YearTo,
                UpdateDate = m.UpdateDate,
                Ph = m.Ph,
                Dh = m.Dh,
                Shc = m.Shc,
                Mht = m.Mht,
                Chc = m.Chc,
            }).OrderByDescending(m => m.UpdateDate).ToList();

            List<smVm> filteredData = data;
            if (data.Any())
            {
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
                            filteredData = data.Where(_item => _item.Month == a || _item.Year == a).ToList();
                        }
                        else
                        {
                            string text = request.Search.Value.Trim().ToLower();
                            filteredData = data.Where(_item => _item.ProvCode != null && _item.ImpCode.ToLower().Contains(text)).ToList();
                        }
                    }
                    else if (request.Search.Value.Contains("/"))
                    {
                        string search = request.Search.Value.Trim();
                        string[] words = search.Split('/');
                        int.TryParse(words[0], out y);
                        int.TryParse(words[1], out a);
                        filteredData = data.Where(_item => _item.Month == a && _item.Year == y).ToList();
                    }
                    else
                    {
                        filteredData = data;
                    }

                }
            }

            var dataPage = filteredData.Skip(request.Start).Take(request.Length);
            var response = DataTablesResponse.Create(request, data.Count(), filteredData.Count(), dataPage);
            return new DataTablesJsonResult(response, true);
        }
        [Authorize(Roles = "administrator")]
        public IActionResult adminView()
        {
            return View();
        }


        [Authorize(Roles = "dataentry,administrator")]
        public async Task<IActionResult> Details(int id)
        {
            var reports = new Samreq();

            if (User.IsInRole("dataentry"))
            {
                reports = await _context.Samreq.SingleOrDefaultAsync(m => m.Rid == id && m.UserName == User.Identity.Name);
            }

            if (User.IsInRole("administrator"))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                IQueryable<Samreq> query = _context.Samreq.Where(m => m.Rid == id);
                if (user.TenantId != 1)
                {
                    query = _context.Samreq.Where(m => m.Rid == id && m.Tenant.Equals(user.TenantId));
                }
                reports = await query.SingleOrDefaultAsync();
            }

            if (reports == null)
            {
                return NotFound();
            }

            return View("samdetail", reports);
        }

        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Delete(int id)
        {

            var report = await _context.Samreq.SingleOrDefaultAsync(m => m.Rid == id && m.UserName == User.Identity.Name);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Samreq.SingleOrDefaultAsync(m => m.Rid == id && m.UserName == User.Identity.Name);
            _context.Samreq.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> edit(int id, [FromBody] samdet report)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.SamreqDetails.Where(m => m.Id == id && m.UserName == User.Identity.Name).SingleOrDefaultAsync();

            if (item.UserName == User.Identity.Name)
            {
                item.Adjustment = report.Adjustment;
                item.AdjustmentComment = report.AdjustmentComment;
                item.CurrentBalance = report.CurrentBalance;
                _context.SamreqDetails.Update(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                return Unauthorized();
            }

        }

        [Authorize(Roles = "dataentry")]

        public async Task<IActionResult> Create()
        {

            var myuser = await  _userManager.FindByNameAsync(User.Identity.Name);

            var data = _context.Nmr.Include(m => m.Facility.DistNavigation.ProvCodeNavigation).Where(m => m.Tenant.Equals(myuser.TenantId)).GroupBy(g => new
            {
                ProvId = g.Facility.DistNavigation.ProvCodeNavigation.ProvName +"-"+ g.Facility.DistNavigation.ProvCode,
                Province = g.Facility.DistNavigation.ProvCodeNavigation.ProvName
            }).Select(n => new
            {
                ProvCode = n.Key.ProvId,
                Province = n.Key.Province
            }).ToList();


            ViewData["ProvList"] = new SelectList(data, "ProvCode", "Province");

            var imp = _context.Nmr.Where(m => m.Tenant.Equals(myuser.TenantId)).GroupBy(g => new
            {
                ImpAcronym = g.Implementer
            }).Select(n => new
            {
                ImpAcronym = n.Key.ImpAcronym,
                ImpName = n.Key.ImpAcronym
            }).ToList();

           
            ViewData["Implementers"] = new SelectList(imp, "ImpAcronym", "ImpName");
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var items = new List<smxlVm>();
            if (User.IsInRole("dataentry"))
            {

                items = await _context.SamreqDetails.Include(mamStock => mamStock.R).Where(m => m.Rid.Equals(id) && m.UserName.Equals(User.Identity.Name)).Include(m => m.SId).AsNoTracking()
               .Select(m => new smxlVm()
               {
                   id = m.Id,
                   item = m.SId.Item,
                   buffer = m.SId.Buffer,
                   u6 = m.U6.GetValueOrDefault(),
                   o6 = m.O6.GetValueOrDefault(),
                   zarib = m.FormName.ToLower().Equals("ipd-sam") ? m.SId.IPDSAMZarib : m.SId.OPDSAMZarib,
                   adj = m.Adjustment.GetValueOrDefault(),
                   balance = m.CurrentBalance.GetValueOrDefault(),
                   comment = m.AdjustmentComment,
                   formname = m.FormName,

               }).ToListAsync();
                return Json(items);
            }


            if (User.IsInRole("administrator"))
            {
                IQueryable<SamreqDetails> query = _context.SamreqDetails.Where(m => m.Rid.Equals(id));
                if (user.TenantId != 1)
                {
                    query = _context.SamreqDetails.Where(m => m.Rid == id && m.R.Tenant.Equals(user.TenantId));
                }
                items = await query.Include(m => m.SId).AsNoTracking()
               .Select(m => new smxlVm()
               {
                   id = m.Id,
                   item = m.SId.Item,
                   buffer = m.SId.Buffer,
                   u6 = m.U6.GetValueOrDefault(),
                   o6 = m.O6.GetValueOrDefault(),
                   zarib = m.FormName.ToLower().Equals("ipd-sam") ? m.SId.IPDSAMZarib : m.SId.OPDSAMZarib,
                   adj = m.Adjustment.GetValueOrDefault(),
                   balance = m.CurrentBalance.GetValueOrDefault(),
                   comment = m.AdjustmentComment,
                   formname = m.FormName,

               }).ToListAsync();
                return Json(items);
            }
            return Json(items);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Create([Bind("ProvCode,ImpCode,Year,Month,ReqYear,ReqMonth,ReqBy")] Samreq report)
        {
            if (ModelState.IsValid)
            {
                report.UserName = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                report.Tenant = user.TenantId;
                report.UpdateDate = DateTime.Now;
                int year, month;
                month = report.Month;
                year = report.Year;

                IQueryable<vsamavail> item0 = _context.vsamavail.Where(m => m.Year == year && (m.Month == month - 1 || m.Month == month - 2 || m.Month == month - 3));
                IQueryable<TblOtp> item2 = _context.TblOtp.Where(m => m.Nmr.Year == year && (m.Nmr.Month == month - 1 || m.Nmr.Month == month - 2 || m.Nmr.Month == month - 3));
                IQueryable<TblOtptfu> item3 = _context.TblOtptfu.Where(m => m.Nmr.Year == year && (m.Nmr.Month == month - 1 || m.Nmr.Month == month - 2 || m.Nmr.Month == month - 3));

                if (month - 4 >= 0)
                {
                    report.MonthFrom = (short)(month - 3);
                    report.YearFrom = (short)(year);

                    report.MonthTo = (short)(month - 1);
                    report.YearTo = (short)(year);

                }


                if (month - 4 < 0)
                {

                    switch (month - 4)
                    {
                        case -1:
                            item0 = _context.vsamavail.Where(m => (m.Year == year && (m.Month == 2 || m.Month == 1)) || (m.Year == year - 1 && m.Month == 12));
                            item2 = _context.TblOtp.Where(m => (m.Nmr.Year == year && (m.Nmr.Month == 2 || m.Nmr.Month == 1)) || (m.Nmr.Year == year - 1 && m.Nmr.Month == 12));
                            item3 = _context.TblOtptfu.Where(m => (m.Nmr.Year == year && (m.Nmr.Month == 2 || m.Nmr.Month == 1)) || (m.Nmr.Year == year - 1 && m.Nmr.Month == 12));

                            report.MonthFrom = (short)(12);
                            report.YearFrom = (short)(year - 1);

                            report.MonthTo = (short)(2);
                            report.YearTo = (short)(year);

                            break;
                        case -2:
                            item0 = _context.vsamavail.Where(m => (m.Year == year && m.Month == 1) || (m.Year == year - 1 && (m.Month == 12 || m.Month == 11)));
                            item2 = _context.TblOtp.Where(m => (m.Nmr.Year == year && m.Nmr.Month == 1) || (m.Nmr.Year == year - 1 && (m.Nmr.Month == 12 || m.Nmr.Month == 11)));
                            item3 = _context.TblOtptfu.Where(m => (m.Nmr.Year == year && m.Nmr.Month == 1) || (m.Nmr.Year == year - 1 && (m.Nmr.Month == 12 || m.Nmr.Month == 11))); ;

                            report.MonthFrom = (short)(11);
                            report.YearFrom = (short)(year - 1);

                            report.MonthTo = (short)(1);
                            report.YearTo = (short)(year);
                            break;


                        case -3:
                            item0 = _context.vsamavail.Where(m => m.Year == year - 1 && (m.Month == 12 || m.Month == 11 || m.Month == 10));
                            item2 = _context.TblOtp.Where(m => m.Nmr.Year == year - 1 && (m.Nmr.Month == 12 || m.Nmr.Month == 11 || m.Nmr.Month == 10));
                            item3 = _context.TblOtptfu.Where(m => m.Nmr.Year == year - 1 && (m.Nmr.Month == 12 || m.Nmr.Month == 11 || m.Nmr.Month == 10));
                            report.MonthFrom = (short)(10);
                            report.YearFrom = (short)(year - 1);

                            report.MonthTo = (short)(12);
                            report.YearTo = (short)(year - 1);
                            break;


                    }
                }

                string[] prov = report.ProvCode.Split('-'); 

                var nmrs = await item0
                .Where(m => m.Implementer == report.ImpCode && m.ProvCode == prov[1].Trim())
                   .Select(m => new
                   {
                       FacType = m.FacilityType,
                       FacilityId = m.FacilityId.ToString()

                   }).ToListAsync();

                var opds = await item2
                    .Where(m => m.Nmr.Implementer == report.ImpCode && m.Nmr.Facility.DistNavigation.ProvCode == prov[1].Trim())
                   .Select(m => new SamreqVm()
                   {
                       FacType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                       Muac115 = m.Muac115.GetValueOrDefault(),
                       Z3score = m.Z3score.GetValueOrDefault(),
                       Odema = m.Odema.GetValueOrDefault(),
                       Fromsfp = m.Fromsfp.GetValueOrDefault(),
                       Fromscotp = m.Fromscotp.GetValueOrDefault(),
                       Defaultreturn = m.Defaultreturn.GetValueOrDefault(),
                       AgeGroup = m.Otp.AgeGroup
                   }).ToListAsync();
                var ipds = await item3
                    .Where(m => m.Nmr.Implementer == report.ImpCode && m.Nmr.Facility.DistNavigation.ProvCode == prov[1].Trim())
                   .Select(m => new SamreqVm()
                   {
                       FacType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                       Muac115 = m.Muac115.GetValueOrDefault(),
                       Z3score = m.Z3score.GetValueOrDefault(),
                       Odema = m.Odema.GetValueOrDefault(),
                       Defaultreturn = m.Defaultreturn.GetValueOrDefault(),
                       Fromscotp = m.Fromscotp.GetValueOrDefault(),
                       Fromsfp = m.Fromsfp.GetValueOrDefault(),
                       AgeGroup = m.Otptfu.AgeGroup
                   }).ToListAsync();



                if (nmrs.Any())
                { 
                    report.Ph = (short)nmrs.Where(m =>m.FacType.ToLower().Contains("ph") & m.FacilityId.Any()).Distinct().Count();
                    report.Dh = (short)nmrs.Where(m => m.FacType.ToLower().Contains("dh") & m.FacilityId.Any()).Distinct().Count();
                    report.Chc = (short)nmrs.Where(m => m.FacType.ToLower().Contains("chc") & m.FacilityId.Any()).Distinct().Count();
                    report.Bhc = (short)nmrs.Where(m => m.FacType.ToLower().Contains("bhc") & m.FacilityId.Any()).Distinct().Count();
                    report.Shc = (short)nmrs.Where(m => m.FacType.ToLower().Contains("shc") & m.FacilityId.Any()).Distinct().Count();
                    report.Mht = (short)nmrs.Where(m => m.FacType.ToLower().Contains("mht") & m.FacilityId.Any()).Distinct().Count();
                }
                _context.Samreq.Add(report);

                var items = _context.TlkpSstock.Where(m => m.IPDSAMZarib > 0 || m.OPDSAMZarib > 0).ToList();
                try
                {
                    if ((opds.Count+ipds.Count)>0)
                    {
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Zero beneficiary found for the selected parameters ( " + report.ProvCode + ", " + report.ImpCode + ", year = " + report.Year + " and month = " + report.Month);
                        var myusers = await _userManager.FindByNameAsync(User.Identity.Name);

                    var datas = _context.Nmr.Include(m => m.Facility.DistNavigation.ProvCodeNavigation).Where(m => m.Tenant.Equals(myusers.TenantId)).GroupBy(g => new
                    {
                        ProvId = g.Facility.DistNavigation.ProvCodeNavigation.ProvName + "-" + g.Facility.DistNavigation.ProvCode,
                        Province = g.Facility.DistNavigation.ProvCodeNavigation.ProvName
                    }).Select(n => new
                    {
                        ProvCode = n.Key.ProvId,
                        Province = n.Key.Province
                    }).ToList();

                    ViewData["ProvList"] = new SelectList(datas, "ProvCode", "Province");

                    var imps = _context.Nmr.Where(m => m.Tenant.Equals(myusers.TenantId)).GroupBy(g => new
                    {
                        ImpAcronym = g.Implementer
                    }).Select(n => new
                    {
                        ImpAcronym = n.Key.ImpAcronym,
                        ImpName = n.Key.ImpAcronym
                    }).ToList();


                    ViewData["Implementers"] = new SelectList(imps, "ImpAcronym", "ImpName");
                    return View(report);
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(String.Empty, "You can create max 1 request per month. If you want to update or edit just delete a required month and re-create.");
                    var myusers = await _userManager.FindByNameAsync(User.Identity.Name);

                    var datas = _context.Nmr.Include(m => m.Facility.DistNavigation.ProvCodeNavigation).Where(m => m.Tenant.Equals(myusers.TenantId)).GroupBy(g => new
                    {
                        ProvId = g.Facility.DistNavigation.ProvCodeNavigation.ProvName + "-" + g.Facility.DistNavigation.ProvCode,
                        Province = g.Facility.DistNavigation.ProvCodeNavigation.ProvName
                    }).Select(n => new
                    {
                        ProvCode = n.Key.ProvId,
                        Province = n.Key.Province
                    }).ToList();

                    ViewData["ProvList"] = new SelectList(datas, "ProvCode", "Province");

                    var imps = _context.Nmr.Where(m => m.Tenant.Equals(myusers.TenantId)).GroupBy(g => new
                    {
                        ImpAcronym = g.Implementer
                    }).Select(n => new
                    {
                        ImpAcronym = n.Key.ImpAcronym,
                        ImpName = n.Key.ImpAcronym
                    }).ToList();


                    ViewData["Implementers"] = new SelectList(imps, "ImpAcronym", "ImpName");
                    return View(report);
                }

                nextfunction:
                if (items.Any())
                {
                    foreach (var item in items)
                    {
                        if (item.OPDSAMZarib > 0 && opds.Any())
                        {
                            var row = new SamreqDetails();
                            row.SupplyId = item.SstockId;
                            row.FormName = "OPD-SAM";
                            row.Rid = report.Rid;
                            row.U6 = 0;
                            row.O6 = opds.Sum(m => m.Muac115 + m.Z3score + m.Odema + m.Defaultreturn + m.Fromscotp + m.Fromsfp);
                            row.UpdateDate = DateTime.Now;
                            row.UserName = User.Identity.Name;
                            _context.SamreqDetails.Add(row);
                        }
                        if (item.IPDSAMZarib > 0 && ipds.Any())
                        {
                            var row = new SamreqDetails();
                            row.SupplyId = item.SstockId;
                            row.FormName = "IPD-SAM";
                            row.Rid = report.Rid;
                            row.UpdateDate = DateTime.Now;
                            row.UserName = User.Identity.Name;
                            row.U6 = ipds.Where(m => m.AgeGroup.ToLower().Contains("6 month"))
                              .Sum(m => m.Muac115 + m.Odema + m.Z3score + m.Defaultreturn + m.Fromscotp + m.Fromsfp);
                            row.O6 = ipds.Where(m => !m.AgeGroup.ToLower().Contains("6 month"))
                               .Sum(m => m.Muac115 + m.Odema + m.Z3score + m.Defaultreturn + m.Fromscotp + m.Fromsfp);
                            _context.SamreqDetails.Add(row);
                        }
                    }
                    await _context.SaveChangesAsync();
                }



                return RedirectToAction("index");
            }


            var myuser = await _userManager.FindByNameAsync(User.Identity.Name);

            var data = _context.Nmr.Include(m => m.Facility.DistNavigation.ProvCodeNavigation).Where(m => m.Tenant.Equals(myuser.TenantId)).GroupBy(g => new
            {
                ProvId = g.Facility.DistNavigation.ProvCodeNavigation.ProvName + "-" + g.Facility.DistNavigation.ProvCode,
                Province = g.Facility.DistNavigation.ProvCodeNavigation.ProvName
            }).Select(n => new
            {
                ProvCode = n.Key.ProvId,
                Province = n.Key.Province
            }).ToList();


            ViewData["ProvList"] = new SelectList(data, "ProvCode", "Province");

            var imp = _context.Nmr.Where(m => m.Tenant.Equals(myuser.TenantId)).GroupBy(g => new
            {
                ImpAcronym = g.Implementer
            }).Select(n => new
            {
                ImpAcronym = n.Key.ImpAcronym,
                ImpName = n.Key.ImpAcronym
            }).ToList();


            ViewData["Implementers"] = new SelectList(imp, "ImpAcronym", "ImpName");

            return View(report);
        }

        public async Task<IActionResult> samxl(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.SamreqDetails.Include(mamStock => mamStock.R).Where(m => m.Rid.Equals(id) && (m.UserName.Equals(User.Identity.Name) || m.R.Tenant.Equals(user.TenantId))).Include(m => m.SId).AsNoTracking()
            .Select(m => new smxlVm()
            {
                item = m.SId.Item,
                buffer = m.SId.Buffer,
                u6 = m.U6.GetValueOrDefault(),
                o6 = m.O6.GetValueOrDefault(),
                zarib = m.FormName.ToLower().Equals("ipd-sam") ? m.SId.IPDSAMZarib : m.SId.OPDSAMZarib,
                adj = m.Adjustment.GetValueOrDefault(),
                balance = m.CurrentBalance.GetValueOrDefault(),
                comment = m.AdjustmentComment,
                formname = m.FormName,

            }).ToList();
            var report = _context.Samreq.Where(m => m.Rid.Equals(id) && (m.UserName.Equals(User.Identity.Name) || m.Tenant.Equals(user.TenantId))).SingleOrDefault();


            if (!items.Any())
            {
                return BadRequest();
            }

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook = application.Workbooks.Create(1);

            IWorksheet sheet = workbook.Worksheets[0];
            sheet.IsGridLinesVisible = false;

            FileStream file = new FileStream(@"Data/pic.png", FileMode.Open, FileAccess.Read);
            byte[] byteArray = new byte[file.Length];
            file.Read(byteArray, 0, (int)file.Length);
            file.Dispose();
            if (byteArray != null)
            {
                MemoryStream s = new MemoryStream(byteArray);

                Syncfusion.Drawing.Image img = Syncfusion.Drawing.Image.FromStream(s);
                IPictureShape shape = sheet.Pictures.AddPicture(2, 11, img);
            }

            sheet["A3:B3"].Merge();
            sheet["C3:D3"].Merge();
            sheet["A3"].Text = "Province";
            sheet.Range["C3"].Text = report.ProvCode;
            sheet.Range["C3"].CellStyle.Font.Italic = true;

            sheet["A4:B4"].Merge();
            sheet["C4:D4"].Merge();
            sheet["A4"].Text = "Implementing Agency";
            sheet.Range["C4"].Text = report.ImpCode;
            sheet.Range["C4"].CellStyle.Font.Italic = true;

            sheet["A5:B5"].Merge();
            sheet["C5:D5"].Merge();
            sheet["A5"].Text = "Request Year";
            sheet.Range["C5"].Value = report.YearFrom.ToString();
            sheet.Range["C5"].CellStyle.Font.Italic = true;

            sheet["A6:B6"].Merge();
            sheet["C6:D6"].Merge();
            sheet["A6"].Text = "Request Month";
            sheet.Range["C6"].Value = report.Month.ToString();
            sheet.Range["C6"].CellStyle.Font.Italic = true;

            sheet.Range["A3:D6"].BorderAround();
            sheet.Range["A3:D6"].BorderInside(ExcelLineStyle.Thin, ExcelKnownColors.Black);


            sheet["T4:U4"].Merge();
            sheet["V4:W4"].Merge();
            sheet["T4"].Text = "Year From";
            sheet.Range["V4"].Value = report.YearFrom.ToString();
            sheet.Range["V4"].CellStyle.Font.Italic = true;

            sheet["T5:U5"].Merge();
            sheet["V5:W5"].Merge();
            sheet["T5"].Text = "Month From";
            sheet.Range["V5"].Value = report.MonthFrom.ToString();
            sheet.Range["V5"].CellStyle.Font.Italic = true;

            sheet["T6:U6"].Merge();
            sheet["V6:W6"].Merge();
            sheet["T6"].Text = "Year To";
            sheet.Range["V6"].Value = report.YearTo.ToString();
            sheet.Range["V6"].CellStyle.Font.Italic = true;

            sheet["T7:U7"].Merge();
            sheet["V7:W7"].Merge();
            sheet["T7"].Text = "Month To";
            sheet.Range["V7"].Value = report.MonthTo.ToString();
            sheet.Range["V7"].CellStyle.Font.Italic = true;

            sheet["T8:U8"].Merge();
            sheet["V8:W8"].Merge();
            sheet["T8"].Text = "Request Date";
            sheet.Range["V8"].Text = report.UpdateDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            sheet.Range["V8"].CellStyle.Font.Italic = true;

            sheet.Range["T4:W8"].BorderAround();
            sheet.Range["T4:W8"].BorderInside(ExcelLineStyle.Thin, ExcelKnownColors.Black);

            sheet["J5:M5"].Merge();
            sheet["J5"].Text = "Ministry of Public Health";

            sheet["I6:N6"].Merge();
            sheet["I6"].Text = "General Directorate of Preventive Medicine";
            sheet["j7:M7"].Merge();
            sheet["j7"].Text = "Public Nutrition Directorate ";

            sheet["j8:M8"].Merge();
            sheet["j8"].Text = "SAM Monthly Request Form";

            sheet["A10:C10"].Merge();
            sheet["A10"].Text = "Number of Health Facilities Covered ";
            
            sheet["A11:B11"].Merge();
            sheet["A12:B12"].Merge();
            sheet["A11"].Text = "PH";
            sheet["A11"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["A12"].Value = report.Ph.ToString();

            sheet["C11:D11"].Merge();
            sheet["C12:D12"].Merge();
            sheet["C11"].Text = "DH";
            sheet["C11"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["C12"].Value = report.Dh.ToString();

            sheet["E11:F11"].Merge();
            sheet["E12:F12"].Merge();
            sheet["E11"].Text = "CHC";
            sheet["E11"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["E12"].Value = report.Chc.ToString();

            sheet["G11:H11"].Merge();
            sheet["G12:H12"].Merge();
            sheet["G11"].Text = "BHC";
            sheet["G11"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["G12"].Value = report.Bhc.ToString();

            sheet["I11:J11"].Merge();
            sheet["I12:J12"].Merge();
            sheet["I11"].Text = "SHC";
            sheet["I11"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["I12"].Value = report.Shc.ToString();

            sheet["K11:L11"].Merge();
            sheet["K12:L12"].Merge();
            sheet["K11"].Text = "MHT";
            sheet["K11"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["K12"].Value = report.Mht.ToString();

            sheet.Range["A11:L12"].BorderAround();


            sheet["A16:F16"].Merge();
            sheet["A16"].Text = "Requested supply for this period";

            sheet["A17:C17"].Merge();
            sheet["A17:C17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["A17"].Text = "Requested Item";

            sheet["D17:E17"].Merge();
            sheet["D17:E17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["D17"].Text = "Program";

            sheet["F17:H17"].Merge();
            sheet["F17:H17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["F17"].Text = "Balance from Last Release";

            sheet["I17:J17"].Merge();
            sheet["I17:J17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["I17"].Text = "# of Children Under \n 6 Months";

            sheet["K17:L17"].Merge();
            sheet["K17:L17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["K17"].Text = "# of Children Over \n 6 Months";


            sheet["M17:O17"].Merge();
            sheet["M17:O17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["M17"].Text = "# of Children Expected to \n be admitted next quarter";


            sheet["P17:R17"].Merge();
            sheet["P17:R17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["P17"].Text = "Needed Quantity for \n Next 3 Months";

            sheet["S17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["S17"].Text = "Buffer(%)";

            sheet["T17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["T17"].Text = "Adjustment";

            sheet["U17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["U17"].Text = "Total";

            sheet["V17:W17"].Merge();
            sheet["V17:W17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["V17"].Text = "Comment";

            int i = 1;
            foreach (var item in items)
            {
                string c = (17 + i).ToString();
                sheet[$"A{c}:C{c}"].Merge();
                sheet[$"A{c}"].Text = item.item;

                sheet[$"D{c}:E{c}"].Merge();
                sheet[$"D{c}"].Text = item.formname;

                sheet[$"F{c}:H{c}"].Merge();
                sheet[$"F{c}"].Value = item.balance.ToString();

                sheet[$"I{c}:J{c}"].Merge();
                sheet[$"I{c}"].Value = item.u6.ToString();

                sheet[$"K{c}:L{c}"].Merge();
                sheet[$"K{c}"].Value = item.o6.ToString();

                sheet[$"M{c}:O{c}"].Merge();
                sheet[$"M{c}"].Value = (item.o6 + item.u6).ToString();


                sheet[$"P{c}:R{c}"].Merge();
                float needed = (item.u6 + item.o6) * item.zarib;
                sheet[$"P{c}"].Value = needed.ToString();


                sheet[$"S{c}"].Value = item.buffer.ToString();

                sheet[$"T{c}"].Value = item.adj.ToString();

                sheet[$"U{c}"].Value = (needed + item.adj + item.balance + (needed * item.buffer)).ToString();

                sheet[$"V{c}:W{c}"].Merge();
                sheet[$"V{c}"].Text = item.comment;

                i++;
            }
            sheet.SetRowHeight(17, 35);
            sheet.Range["A17:W17"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
            string rows = (items.Count() + 17).ToString();
            sheet.Range[$"A17:W{rows}"].BorderAround();
            sheet.Range[$"A17:W{rows}"].BorderInside(ExcelLineStyle.Thin, ExcelKnownColors.Black);


            string ContentType = "Application/msexcel";
            string filename = "NMR.xlsx";
            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

    }
}

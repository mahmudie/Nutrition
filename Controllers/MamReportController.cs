using DataSystem.Models;
using DataSystem.Models.ViewModels;
using DataSystem.ViewModels;
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
    public class MamReportController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public MamReportController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.Mamreq.Where(m => m.UserName == User.Identity.Name).AsNoTracking();
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
            IQueryable<Mamreq> query = _context.Mamreq;
            if (user.TenantId != 1)
            {
                query = _context.Mamreq.Where(m => m.Tenant.Equals(user.TenantId));
            }
            var data = query.Select(m => new smVm()
            {
                Rid = m.Rid,
                ProvCode = m.ProvCode,
                ImpCode = m.ImpCode,
                Year = m.Year,
                Month = m.Month,
                ReqBy = m.ReqBy,
                MonthFrom = m.ReqMonth,
                YearFrom = (short)m.ReqYear.GetValueOrDefault(),
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
            var reports = new Mamreq();

            if (User.IsInRole("dataentry"))
            {
                reports = await _context.Mamreq.SingleOrDefaultAsync(m => m.Rid == id && m.UserName == User.Identity.Name);
            }

            if (User.IsInRole("administrator"))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                IQueryable<Mamreq> query = _context.Mamreq.Where(m => m.Rid == id);
                if (user.TenantId != 1)
                {
                    query = _context.Mamreq.Where(m => m.Rid == id && m.Tenant.Equals(user.TenantId));
                }
                else
                {
                    query = _context.Mamreq.Where(m => m.Rid == id);
                }
                reports = await query.SingleOrDefaultAsync();
            }

            if (reports == null)
            {
                return NotFound();
            }

            return View("mamdetail", reports);
        }

        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Delete(int id)
        {

            var report = await _context.Mamreq.SingleOrDefaultAsync(m => m.Rid == id && m.UserName == User.Identity.Name);
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
            var report = await _context.Mamreq.SingleOrDefaultAsync(m => m.Rid == id && m.UserName == User.Identity.Name);
            _context.Mamreq.Remove(report);
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

            var item = await _context.MamreqDetails.Include(m => m.R).Where(m => m.Id == id && m.R.UserName == User.Identity.Name).AsNoTracking().SingleOrDefaultAsync();

            if (item != null && item.R.UserName == User.Identity.Name)
            {
                item.Adjustment = report.Adjustment;
                item.AdjustmentComment = report.AdjustmentComment;
                item.CurrentBalance = report.CurrentBalance;
                _context.MamreqDetails.Update(item);
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
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var items = new List<mamxlVm>();
            if (User.IsInRole("dataentry"))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                items = _context.MamreqDetails.Where(m => m.Rid.Equals(id) && (m.R.UserName.Equals(User.Identity.Name) || m.R.Tenant.Equals(user.TenantId))).Include(m => m.SId).AsNoTracking()
               .Select(m => new mamxlVm()
               {
                   id = m.Id,
                   item = m.SId.Item,
                   formname = m.FormName,
                   buffer = m.SId.Buffer,
                   benefactories = m.NoOfBenificiaries.GetValueOrDefault(),
                   zarib = m.SId.Zarib,
                   adj = m.Adjustment.GetValueOrDefault(),
                   balance = m.CurrentBalance.GetValueOrDefault(),
                   comment = m.AdjustmentComment,
               }).ToList();
                return Json(items);
            }

            if (User.IsInRole("administrator"))
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                IQueryable<MamreqDetails> query = _context.MamreqDetails.Where(m => m.Rid.Equals(id));
                if (user.TenantId != 1)
                {
                    query = _context.MamreqDetails.Where(m => m.Rid == id && m.R.Tenant.Equals(user.TenantId));
                }
                items = query.Include(m => m.SId).AsNoTracking()
               .Select(m => new mamxlVm()
               {
                   id = m.Id,
                   formname = m.FormName,
                   item = m.SId.Item,
                   buffer = m.SId.Buffer,

                   benefactories = m.NoOfBenificiaries.GetValueOrDefault(),
                   zarib = m.SId.Zarib,
                   adj = m.Adjustment.GetValueOrDefault(),
                   balance = m.CurrentBalance.GetValueOrDefault(),
                   comment = m.AdjustmentComment,
               }).ToList();
                return Json(items);
            }
            return Json(items);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "dataentry")]
        public async Task<IActionResult> Create([Bind("ProvCode,ImpCode,Year,Month,ReqBy,numMonth")] mreqvm req)
        {
            if (ModelState.IsValid)
            {
                Mamreq report = new Mamreq();
                report.UserName = User.Identity.Name;
                report.UpdateDate = DateTime.Now;
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                report.Tenant = user.TenantId;
                var nmrs = new List<MamreqVm>();
                var mams = new List<MamreqVm>();
                var nmrs2 = new List<MamreqVm>();
                var mams2 = new List<MamreqVm>();
                report.ReqBy = req.ReqBy;
                int year = req.Year;
                var numMonth = new List<int>();
                var numMonth2 = new List<int>();
                int year2 = 0;
                int month2=0;
                month2=req.numMonth;
                while (req.numMonth != 0)
                {
                    if (req.Month - req.numMonth > 0)
                    {
                        numMonth.Add(req.Month - req.numMonth);
                    }
                    else
                    {
                        switch (req.Month - req.numMonth)
                        {
                            case 0:
                                numMonth.Add(12);
                                break;
                            case -1:
                                numMonth.Add(11);
                                break;
                            case -2:
                                numMonth.Add(10);
                                break;
                            case -3:
                                numMonth.Add(9);
                                break;

                        }
                    }

                    req.numMonth = req.numMonth - 1;

                }
                if (numMonth.Contains(12))
                {

                    year2 = req.Year - 1;
                    foreach (var item in numMonth.ToList())
                    {
                        if (item == 12||item == 11 || item == 10 || item == 9)
                        numMonth.Remove(item);
                        numMonth2.Add(item);
                    }
                }

                string[] prov = req.ProvCode.Split('-');

                var rep = _context.vmamavail.Where(m => m.Year == year && numMonth.Contains(m.Month) && m.Implementer == req.ImpCode && m.ProvCode == prov[1].Trim());

               // nmrs = _context.vmamavail.Where(m => m.Year == year && numMonth.Contains(m.Month))
               //.Where(m => m.Implementer == req.ImpCode && m.ProvCode == prov.Trim())
               //.Select(m => new MamreqVm()
               //{
               //    FacType = m.FacilityType,
               //    FacilityName = m.FacilityName

               //}).ToList();

                mams = _context.TblMam.Where(m => m.Nmr.Year == year && numMonth.Contains(m.Nmr.Month))
                .Where(m => m.Nmr.Implementer == req.ImpCode && m.Nmr.Facility.DistNavigation.ProvCode == prov[1].Trim())
               .Select(m => new MamreqVm()
               {
                   Muac12 = m.Muac12.GetValueOrDefault(),
                   Muac23 = m.Muac23.GetValueOrDefault(),
                   Zscore23 = m.Zscore23.GetValueOrDefault(),
                   refIn = m.ReferIn.GetValueOrDefault(),
                   AgeGroup = m.Mam.AgeGroup,
               }).ToList();


                nmrs2 = _context.Nmr.Where(m => m.Year == year2 && numMonth2.Contains(m.Month))
               .Where(m => m.Implementer == req.ImpCode && m.Facility.DistNavigation.ProvCode == prov[1].Trim())
               .Select(m => new MamreqVm()
               {
                   FacType = m.Facility.FacilityTypeNavigation.FacType,

               }).ToList();

                mams2 = _context.TblMam.Where(m => m.Nmr.Year == year2 && numMonth2.Contains(m.Nmr.Month))
                .Where(m => m.Nmr.Implementer == req.ImpCode && m.Nmr.Facility.DistNavigation.ProvCode == prov[1].Trim())
               .Select(m => new MamreqVm()
               {
                   Muac12 = m.Muac12.GetValueOrDefault(),
                   Muac23 = m.Muac23.GetValueOrDefault(),
                   Zscore23 = m.Zscore23.GetValueOrDefault(),
                   refIn = m.ReferIn.GetValueOrDefault(),
                   AgeGroup = m.Mam.AgeGroup,
               }).ToList();
               mams.AddRange(mams2);
               nmrs.AddRange(nmrs2);

                if (rep.Any())
                {
                    report.Ph = (short)rep.Where(m => m.FacilityType.ToLower().Contains("ph")).Select(m => m.FacilityType + m.FacilityName).Distinct().Count();
                    report.Dh = (short)rep.Where(m => m.FacilityType.ToLower().Contains("dh")).Select(m => m.FacilityType + m.FacilityName).Distinct().Count();
                    report.Chc = (short)rep.Where(m => m.FacilityType.ToLower().Contains("chc")).Select(m => m.FacilityType + m.FacilityName).Distinct().Count();
                    report.Bhc = (short)rep.Where(m => m.FacilityType.ToLower().Contains("bhc")).Select(m => m.FacilityType + m.FacilityName).Distinct().Count();
                    report.Shc = (short)rep.Where(m => m.FacilityType.ToLower().Contains("shc")).Select(m => m.FacilityType + m.FacilityName).Distinct().Count();
                    report.Mht = (short)rep.Where(m => m.FacilityType.ToLower().Contains("mht")).Select(m => m.FacilityType + m.FacilityName).Distinct().Count();
                }
                report.ProvCode = prov[0]+"-"+prov[1];
                report.ImpCode = req.ImpCode;
                report.Year = req.Year;
                report.Month =(short)month2;
                report.ReqYear = req.Year;
                report.ReqMonth = (short)req.Month;
                _context.Mamreq.Add(report);

                var mamitems = _context.TlkpFstock.Where(m => m.Zarib > 0).ToList();

                try
                {
                    if (mams.Count>0)
                    {
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Zero beneficiary found for the selected parameters ( " + report.ProvCode +", "+report.ImpCode +", year = "+ report.Year +" and month = "+report.ReqMonth);
                        var myusers2 = await _userManager.FindByNameAsync(User.Identity.Name);

                        var datas2 = _context.Nmr.Include(m => m.Facility.DistNavigation.ProvCodeNavigation).Where(m => m.Tenant.Equals(myusers2.TenantId)).GroupBy(g => new
                        {
                            ProvId = g.Facility.DistNavigation.ProvCodeNavigation.ProvName + "-" + g.Facility.DistNavigation.ProvCode,
                            Province = g.Facility.DistNavigation.ProvCodeNavigation.ProvName
                        }).Select(n => new
                        {
                            ProvCode = n.Key.ProvId,
                            Province = n.Key.Province
                        }).ToList();

                        ViewData["ProvList"] = new SelectList(datas2, "ProvCode", "Province");

                        var imps2 = _context.Nmr.Where(m => m.Tenant.Equals(myusers2.TenantId)).GroupBy(g => new
                        {
                            ImpAcronym = g.Implementer
                        }).Select(n => new
                        {
                            ImpAcronym = n.Key.ImpAcronym,
                            ImpName = n.Key.ImpAcronym
                        }).ToList();


                        ViewData["Implementers"] = new SelectList(imps2, "ImpAcronym", "ImpName");
                        return View(req);
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
                    return View(req);
                }

                if (mamitems.Any())
                {
                    foreach (var item in mamitems)
                    {
                        if (item.Zarib > 0 && mams.Any())
                        {
                            if (item.Item.ToLower().Contains("supercereal"))
                            {
                                var row = new MamreqDetails();
                                row.SupplyId = item.StockId;
                                row.Rid = report.Rid;
                                row.NoOfBenificiaries = mams.Where(m => m.AgeGroup.ToLower().Contains("women")).Sum(m => m.Muac12 + m.Muac23 + m.Zscore23 + m.refIn);
                                row.FormName = "WOMEN";
                                _context.MamreqDetails.Add(row);
                            }
                            if (item.Item.ToLower().Contains("rusf"))
                            {
                                var row2 = new MamreqDetails();
                                row2.SupplyId = item.StockId;
                                row2.Rid = report.Rid;
                                row2.NoOfBenificiaries = mams.Where(m => m.AgeGroup.ToLower().Contains("children")).Sum(m => m.Muac12 + m.Muac23 + m.Zscore23 + m.refIn);
                                row2.FormName = "Children";
                                _context.MamreqDetails.Add(row2);
                            }
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
            return View(req);
        }

        public async Task<IActionResult> mamxl(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var items = _context.MamreqDetails.Include(mamStock => mamStock.R).Where(m => m.Rid.Equals(id) && (m.R.UserName.Equals(User.Identity.Name) || m.R.Tenant.Equals(user.TenantId))).Include(m => m.SId).AsNoTracking()
            .Select(m => new mamxlVm()
            {
                item = m.SId.Item,
                buffer = m.SId.Buffer,
                formname = m.FormName,
                benefactories = m.NoOfBenificiaries.GetValueOrDefault(),
                zarib = m.SId.Zarib,
                adj = m.Adjustment.GetValueOrDefault(),
                balance = m.CurrentBalance.GetValueOrDefault(),
                comment = m.AdjustmentComment,
            }).ToList();
            var report = _context.Mamreq.Where(m => m.Rid.Equals(id) && (m.UserName.Equals(User.Identity.Name) || m.Tenant.Equals(user.TenantId))).SingleOrDefault();


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
            sheet.Range["C5"].Value = report.ReqYear.ToString();
            sheet.Range["C5"].CellStyle.Font.Italic = true;

            sheet["A6:B6"].Merge();
            sheet["C6:D6"].Merge();
            sheet["A6"].Text = "Request Month";
            sheet.Range["C6"].Value = report.ReqMonth.ToString();
            sheet.Range["C6"].CellStyle.Font.Italic = true;

            sheet.Range["A3:D6"].BorderAround();
            sheet.Range["A3:D6"].BorderInside(ExcelLineStyle.Thin, ExcelKnownColors.Black);


            sheet["T4:U4"].Merge();
            sheet["V4:W4"].Merge();
            sheet["T4"].Text = "No of months";
            sheet.Range["V4"].Value = report.Month.ToString();
            sheet.Range["V4"].CellStyle.Font.Italic = true;

            sheet["T5:U5"].Merge();
            sheet["V5:W5"].Merge();
            sheet["T5"].Text = "Requested by";
            sheet.Range["V5"].Text = report.ReqBy;
            sheet.Range["V5"].CellStyle.Font.Italic = true;

            sheet["T6:U6"].Merge();
            sheet["V6:W6"].Merge();
            sheet["T6"].Text = "Request Date";
            sheet.Range["V6"].Text = report.UpdateDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture); ;
            sheet.Range["V6"].CellStyle.Font.Italic = true;

            sheet.Range["T4:W6"].BorderAround();
            sheet.Range["T4:W6"].BorderInside(ExcelLineStyle.Thin, ExcelKnownColors.Black);




            sheet["J5:L5"].Merge();
            sheet["J5"].Text = "          Ministry of Public Health";
            sheet["I6:M6"].Merge();
            sheet["I6"].Text = "        General Directorate of Preventive Medicine";
            sheet["j7:L7"].Merge();
            sheet["j7"].Text = "        Public Nutrition Directorate ";

            sheet["j8:L8"].Merge();
            sheet["j8"].Text = "      MAM Monthly Request Form";



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

            sheet["I17:K17"].Merge();
            sheet["I17:K17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["I17"].Text = "# of beneficiaries";

            sheet["L17:N17"].Merge();
            sheet["L17:N17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["L17"].Text = "Needed Quant for Next "+ report.Month.ToString() +" Month" ;


            sheet["O17:P17"].Merge();
            sheet["O17:P17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["O17"].Text = "Buffer (%)";


            sheet["Q17:R17"].Merge();
            sheet["Q17:R17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["Q17"].Text = "Adjustment";

            sheet["S17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["S17"].Text = "Total Qty Needed";

            sheet["T17:U17"].CellStyle.ColorIndex = ExcelKnownColors.Aqua;
            sheet["T17"].Text = "Comment/Remarks";


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

                sheet[$"I{c}:K{c}"].Merge();
                sheet[$"I{c}"].Value = item.benefactories.ToString();
                float needed = item.benefactories * item.zarib;

                sheet[$"L{c}:N{c}"].Merge();
                sheet[$"L{c}"].Value = (needed).ToString();

                sheet[$"O{c}:P{c}"].Merge();
                sheet[$"O{c}"].Value = "0";

                sheet[$"Q{c}:R{c}"].Merge();
                sheet[$"Q{c}"].Value = item.adj.ToString();

                sheet[$"S{c}"].Value = (needed + item.balance + item.adj).ToString();

                sheet[$"T{c}:U{c}"].Merge();
                sheet[$"T{c}"].Text = item.comment;


                i++;
            }

            sheet.SetRowHeight(17, 35);
            sheet.Range["A17:U17"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;


            string rows = (items.Count() + 17).ToString();
            sheet.Range[$"A17:U{rows}"].BorderAround();
            sheet.Range[$"A17:U{rows}"].BorderInside(ExcelLineStyle.Thin, ExcelKnownColors.Black);


            string ContentType = "Application/msexcel";
            string filename = "mamreq.xlsx";
            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

    }
}

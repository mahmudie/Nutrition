using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;
using System.IO;
using DataSystem.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataSystem.Models.ViewModels.PivotTable;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using DataSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using DataSystem.Models.ViewModels.Export;
using System.Net;

namespace DataSystem.Controllers
{
    [Authorize]
    public class FacilityReportController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FacilityReportController(WebNutContext context, IMapper mapper, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            this._context.Database.SetCommandTimeout(3000);

            List<Provinces> Povinces = _context.Provinces.ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");

            List<mamcommon> mamcommons = _context.Mamcommons.ToList();
            mamcommons.Insert(0, new mamcommon { FacilityId = 0, FacilityName = "select" });
            ViewData["FacilityList"] = new SelectList(mamcommons, "FacilityId", "FacilityName");

            List<FormatYear> FormatYears = _context.FormatYears.OrderBy(m=>m.YearFrom).ToList();
            FormatYears.Insert(0, new FormatYear { YearFrom = 0, YearTo = "select" });
            ViewData["FormatYearList"] = new SelectList(FormatYears, "YearFrom", "YearTo");
            ViewData["FormatYearToList"] = new SelectList(FormatYears, "YearFrom", "YearTo");


            List<ImpFilter> vImplementers = _context.ImpFilter.ToList();
            vImplementers.Insert(0, new ImpFilter { ImpCode = "0", Implementer = "select" });
            ViewData["ImplementerList"] = new SelectList(vImplementers, "ImpCode", "Implementer");

            return View();
        }

        public IActionResult Province()
        {
            this._context.Database.SetCommandTimeout(3000);

            List<Provinces> Povinces = _context.Provinces.ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");

            List<FormatYear> FormatYears = _context.FormatYears.OrderBy(m => m.YearFrom).ToList();
            FormatYears.Insert(0, new FormatYear { YearFrom = 0, YearTo = "select" });
            ViewData["FormatYearList"] = new SelectList(FormatYears, "YearFrom", "YearTo");
            ViewData["FormatYearToList"] = new SelectList(FormatYears, "YearFrom", "YearTo");


            List<ImpFilter> vImplementers = _context.ImpFilter.ToList();
            vImplementers.Insert(0, new ImpFilter { ImpCode = "0", Implementer = "select" });
            ViewData["ImplementerList"] = new SelectList(vImplementers, "ImpCode", "Implementer");

            return View();
        }

        public IActionResult District()
        {
            this._context.Database.SetCommandTimeout(3000);

            List<Provinces> Povinces = _context.Provinces.ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "select" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");

            List<Districts> districts = _context.Districts.ToList();
            districts.Insert(0, new Districts { DistCode = "0", DistName = "select" });
            ViewData["DistrictList"] = new SelectList(districts, "DistCode", "DistName");

            List<FormatYear> FormatYears = _context.FormatYears.OrderBy(m => m.YearFrom).ToList();
            FormatYears.Insert(0, new FormatYear { YearFrom = 0, YearTo = "select" });
            ViewData["FormatYearList"] = new SelectList(FormatYears, "YearFrom", "YearTo");
            ViewData["FormatYearToList"] = new SelectList(FormatYears, "YearFrom", "YearTo");


            List<ImpFilter> vImplementers = _context.ImpFilter.ToList();
            vImplementers.Insert(0, new ImpFilter { ImpCode = "0", Implementer = "select" });
            ViewData["ImplementerList"] = new SelectList(vImplementers, "ImpCode", "Implementer");

            return View();
        }

        public IActionResult Partner()
        {
            this._context.Database.SetCommandTimeout(3000);

            List<FormatYear> FormatYears = _context.FormatYears.OrderBy(m => m.YearFrom).ToList();
            FormatYears.Insert(0, new FormatYear { YearFrom = 0, YearTo = "select" });
            ViewData["FormatYearList"] = new SelectList(FormatYears, "YearFrom", "YearTo");
            ViewData["FormatYearToList"] = new SelectList(FormatYears, "YearFrom", "YearTo");


            List<ImpFilter> vImplementers = _context.ImpFilter.ToList();
            vImplementers.Insert(0, new ImpFilter { ImpCode = "0", Implementer = "select" });
            ViewData["ImplementerList"] = new SelectList(vImplementers, "ImpCode", "Implementer");

            return View();
        }

        [Authorize(Roles = "dataentry")]
        public JsonResult districts(string ProvCode)
        {
            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts where dist.ProvCode == ProvCode select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "0", DistName = "select" });
            return Json(new SelectList(Districts, "DistCode", "DistName"));
        }
        [Authorize(Roles = "dataentry")]
        public JsonResult facilities(string DistCode)
        {
            List<FacilityInfo> facilities = new List<FacilityInfo>();
            facilities = (from fac in _context.FacilityInfo where fac.DistCode == DistCode && fac.ActiveStatus == "Y" select fac).ToList();
            facilities.Insert(0, new FacilityInfo { FacilityId = 0, FacilityName = "select" });
            return Json(new SelectList(facilities, "FacilityId", "FacilityFull"));
        }

        public JsonResult implementers(int FacilityId)
        {
            var implementers = _context.VHFImplementers.AsNoTracking().Where(w => w.FacilityId.Equals(FacilityId)).GroupBy(g => new
            {
                ImpCode = g.ImpCode,
                Implementer = g.Implementer
            }).Select(m => new Imps
            {
                ImpCode = m.Key.ImpCode,
                Implementer = m.Key.Implementer
            }).ToList();

            List<Imps> hFImplementers = new List<Imps>();
            hFImplementers = implementers;
            hFImplementers.Insert(0, new Imps { ImpCode = "0", Implementer = "select" });
            return Json(new SelectList(hFImplementers, "ImpCode", "Implementer"));
        }

        //District level implementer filter
        public JsonResult distimplementers(string DistCode)
        {
            var implementers = _context.VDistImplementers.AsNoTracking().Where(w => w.DistCode.Contains(DistCode)).GroupBy(g => new
            {
                ImpCode = g.ImpCode,
                Implementer = g.Implementer
            }).Select(m => new Imps
            {
                ImpCode = m.Key.ImpCode,
                Implementer = m.Key.Implementer
            }).ToList();

            List<Imps> hFImplementers = new List<Imps>();
            hFImplementers = implementers;
            hFImplementers.Insert(0, new Imps { ImpCode = "0", Implementer = "select" });
            return Json(new SelectList(hFImplementers, "ImpCode", "Implementer"));
        }

        //Province level implementer filter
        public JsonResult provimplementers(string ProvCode)
        {
            var implementers = _context.VProvImplementers.AsNoTracking().Where(w => w.ProvCode.Contains(ProvCode)).GroupBy(g => new
            {
                ImpCode = g.ImpCode,
                Implementer = g.Implementer
            }).Select(m => new Imps
            {
                ImpCode = m.Key.ImpCode,
                Implementer = m.Key.Implementer
            }).ToList();

            List<Imps> hFImplementers = new List<Imps>();
            hFImplementers = implementers;
            hFImplementers.Insert(0, new Imps { ImpCode = "0", Implementer = "select" });
            return Json(new SelectList(hFImplementers, "ImpCode", "Implementer"));
        }
        //Facility level
        public IActionResult FacilityReport(FormatFilterReq req)
        {
            string ProveCode;
            int FacilityId;
            int YearFrom;
            int YearTo;
            int MonthFrom;
            int MonthTo;
            int TimeFrom, TimeTo;
            string impcode;

            ProveCode = req.ProvCode;
            FacilityId = req.FacilityId;
            YearFrom = req.YearFrom;
            YearTo = req.YearTo;
            MonthFrom = req.MonthFrom;
            MonthTo = req.MonthTo;
            impcode = req.Implementer;

            TimeFrom = YearFrom * 100 + MonthFrom;
            TimeTo = YearTo * 100 + MonthTo;

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            FileStream inputStream = new FileStream(ResolveApplicationPath("TSFP_HF.xlsx"), FileMode.Open, FileAccess.Read);
            IWorkbook workbook = application.Workbooks.Open(inputStream);
            IWorksheet worksheet =  workbook.Worksheets["TSFP"];
            this._context.Database.SetCommandTimeout(3000);
            var data = _context.Formatmamreports.AsNoTracking().Where(m => m.FacilityId.Equals(FacilityId) && m.Time>= TimeFrom && m.Time<= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g=>new
            {
                FacilityId=g.FacilityId,
                FacilityName = g.FacilityName,
                FacilityIdName = g.FacilityId.ToString() + '-' + g.FacilityName,
                Province = g.Province,
                ProvCode = g.ProvCode,
                DistCode = g.DistCode,
                District = g.District,
                TypeAbbrv = g.TypeAbbrv,
                SFPID=g.SFPID,
                Implementer=g.Implementer
            }).Select(m => new
            {
                FacilityId = m.Key.FacilityId,
                FacilityName = m.Key.FacilityName,
                FacilityIdName = m.Key.FacilityId.ToString() + '-' + m.Key.FacilityName,
                Province = m.Key.Province,
                ProvCode = m.Key.ProvCode,
                DistCode = m.Key.DistCode,
                District = m.Key.District,
                Implementer = m.Key.Implementer,
                SFPID = m.Key.SFPID,
                MUAC12 =    m.Sum(i=>i.MUAC12),
                MUAC23 =    m.Sum(i=>i.MUAC23),
                Absents =    m.Sum(i=>i.Absents),
                Cured =     m.Sum(i=>i.Cured),
                Deaths =     m.Sum(i=>i.Deaths),
                Defaulters = m.Sum(i=>i.Defaulters),
                NonCured =   m.Sum(i=>i.NonCured),
                ReferIn =    m.Sum(i=>i.ReferIn),
                tFemale =    m.Sum(i=>i.tFemale),
                tMale =      m.Sum(i=>i.tMale),
                totalbegin = m.Sum(i=>i.totalbegin),
                Transfers =  m.Sum(i=>i.Transfers),
                Zscore23 =   m.Sum(i=>i.Zscore23),
                SFP_ALS =    m.Sum(i=>i.SFP_ALS),
                SFP_AWG = m.Sum(i => i.SFP_AWG),

            }).OrderBy(o=>o.SFPID).ToList();

            if (data.Count == 0)
            {
                return RedirectToAction("Index");
            }
            var facilityDetail = data.GroupBy(m => new
            {
                FacilityId=m.FacilityId,
                FacilityName=m.FacilityIdName,
                ProvCode =m.ProvCode,
                Province=m.Province,
                DistCode =m.DistCode,
                District =m.District,
                Implementer=m.Implementer,
            }).Select(m=>new
            {
                FacilityId = m.Key.FacilityId,
                FacilityName = m.Key.FacilityName,
                ProvCode = m.Key.ProvCode,
                Province = m.Key.Province,
                DistCode = m.Key.DistCode,
                District = m.Key.District,
                Implementer = m.Key.Implementer,
                SFP_ALS=m.Average(n=>n.SFP_ALS),
                SFP_AWG=m.Average(n=>n.SFP_AWG)
            }).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            var mamstockdata = _context.Formatmamstockreports.AsNoTracking().Where(m => m.FacilityId.Equals(FacilityId) && m.Time >= TimeFrom && m.Time <= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g=>new {
                FacilityId = g.FacilityId,
                Province = g.Province,
                ProvCode = g.ProvCode,
                DistCode = g.DistCode,
                stockID = g.stockID
            }).Select(m => new
            {
                FacilityId = m.Key.FacilityId,
                Province = m.Key.Province,
                ProvCode = m.Key.ProvCode,
                DistCode = m.Key.DistCode,
                StockId=m.Key.stockID,
                ExpectedRecepients =m.Sum(i=>i.ExpectedRecepients),
                Losses = m.Sum(i => i.Losses),
                OpeningBalance = m.Sum(i => i.OpeningBalance),
                QuantityDistributed = m.Sum(i => i.QuantityDistributed),
                QuantityReceived =  m.Sum(i=>i.QuantityReceived),
                QuantityReferin =   m.Sum(i=>i.QuantityReferin),
                QuantityReturned = m.Sum(i => i.QuantityReturned),
                QuantityTransferred = m.Sum(i => i.QuantityTransferred)

            }).OrderBy(o => o.StockId).ToList();

            foreach (var i in facilityDetail)
            {
                if(i.FacilityId>0)
                {
                    worksheet.Range["C1"].Text = i.ProvCode + '-'+i.Province;
                    worksheet.Range["C2"].Text = i.DistCode + "-" + i.District;
                    worksheet.Range["C3"].Text = i.Implementer;
                    worksheet.Range["R1"].Text = i.FacilityName;
                    worksheet.Range["R2"].Text = i.FacilityId.ToString();
                    worksheet.Range["R3"].Text = MonthFrom+"/"+YearFrom;
                    worksheet.Range["R4"].Text = MonthTo+"/"+YearTo;
                    worksheet.Range["A18"].Number = i.SFP_ALS;
                    worksheet.Range["A18"].NumberFormat = "#,###.#";
                    worksheet.Range["E18"].Number = i.SFP_AWG;
                    worksheet.Range["E18"].NumberFormat = "#,###.#";

                }
            }

            int startRow = 9;
            foreach(var j in data)
            {
                if(j.FacilityId>0)
                {
                    if(j.SFPID==1)
                    {
                        worksheet.Range["C" + startRow].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;
                        
                        worksheet.Range["D" + startRow].Number = j.Zscore23;
                        worksheet.Range["E" + startRow].Number = j.MUAC12;
                        worksheet.Range["G" + startRow].Number = j.tMale;
                        worksheet.Range["H" + startRow].Number = j.tFemale;
                        worksheet.Range["P" + startRow].Number = j.Transfers;              
                        worksheet.Range["J" + startRow].Number = j.ReferIn;
                        worksheet.Range["M" + startRow].Number = j.Cured;
                        worksheet.Range["N" + startRow].Number = j.Deaths;
                        worksheet.Range["O" + startRow].Number = j.Defaulters;
                        worksheet.Range["Q" + startRow].Number = j.NonCured;
                    }
                    if(j.SFPID==2)
                    {
                        worksheet.Range["C" + (startRow + 1)].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;

                        worksheet.Range["D" + (startRow+1)].Number = j.Zscore23;
                        worksheet.Range["E" + (startRow+1)].Number = j.MUAC12;
                        worksheet.Range["G" + (startRow+1)].Number = j.tMale;
                        worksheet.Range["H" + (startRow+1)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow+1)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow+1)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow+1)].Number = j.Cured;
                        worksheet.Range["N" + (startRow+1)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow+1)].Number = j.Defaulters;
                        worksheet.Range["Q" + (startRow + 1)].Number = j.NonCured;
                    }
                    if(j.SFPID==3)
                    {
                        worksheet.Range["C" + (startRow+2)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow+2)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow+2)].Number = j.Absents;
                        worksheet.Range["H" + (startRow+2)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow+2)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow+2)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow+2)].Number = j.Cured;
                        worksheet.Range["N" + (startRow+2)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow +2)].Number = j.Defaulters;
                    }
                    if (j.SFPID == 4)
                    {
                        worksheet.Range["C" + (startRow+3)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow+3)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow+3)].Number = j.Absents;
                        worksheet.Range["H" + (startRow+3)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow+3)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow+3)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow+3)].Number = j.Cured;
                        worksheet.Range["N" + (startRow+3)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow +3)].Number = j.Defaulters;
                    }
                    //startRow += 1;
                }
            }

            int stockRow = 20;
            foreach(var h in mamstockdata)
            {
                if (h.StockId == 1)
                {
                    worksheet.Range["L" + stockRow].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + stockRow].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + stockRow].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + stockRow].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + stockRow].Number = (double)h.Losses;
                    worksheet.Range["Q" + stockRow].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + stockRow].Number = (double)h.ExpectedRecepients;
                }
                if(h.StockId==2)
                {
                    worksheet.Range["L" +(stockRow+1)].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + (stockRow+1)].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + (stockRow+1)].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + (stockRow+1)].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + (stockRow+1)].Number = (double)h.Losses;
                    worksheet.Range["Q" + (stockRow+1)].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + (stockRow+1)].Number = (double)h.ExpectedRecepients;
                }
                //stockRow += 1;
            }

            string ContentType = "Application/msexcel";
            string filename = "IMAM_" + "Report_TSFP_Facility" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";


            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }
        
        //District level 
        public IActionResult DistrictReport(FormatFilterReq req)
        {
            string ProveCode,DistCode;
            int FacilityId;
            int YearFrom;
            int YearTo;
            int MonthFrom;
            int MonthTo;
            int TimeFrom, TimeTo;
            string impcode;

            ProveCode = req.ProvCode;
            FacilityId = req.FacilityId;
            YearFrom = req.YearFrom;
            YearTo = req.YearTo;
            MonthFrom = req.MonthFrom;
            MonthTo = req.MonthTo;
            impcode = req.Implementer;
            DistCode = req.DistCode;

            TimeFrom = YearFrom * 100 + MonthFrom;
            TimeTo = YearTo * 100 + MonthTo;

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            FileStream inputStream = new FileStream(ResolveApplicationPath("TSFP_District.xlsx"), FileMode.Open, FileAccess.Read);
            IWorkbook workbook = application.Workbooks.Open(inputStream);
            IWorksheet worksheet = workbook.Worksheets["TSFP"];
            this._context.Database.SetCommandTimeout(3000);
            var data = _context.Formatmamreports.AsNoTracking().Where(m => m.DistCode.Contains(DistCode) && m.Time >= TimeFrom && m.Time <= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g => new
            {
                Province = g.Province,
                ProvCode = g.ProvCode,
                DistCode = g.DistCode,
                District = g.District,
                SFPID = g.SFPID,
                Implementer = g.Implementer
            }).Select(m => new
            {
                Province = m.Key.Province,
                ProvCode = m.Key.ProvCode,
                DistCode = m.Key.DistCode,
                District = m.Key.District,
                Implementer = m.Key.Implementer,
                SFPID = m.Key.SFPID,
                MUAC12 = m.Sum(i => i.MUAC12),
                MUAC23 = m.Sum(i => i.MUAC23),
                Absents = m.Sum(i => i.Absents),
                Cured = m.Sum(i => i.Cured),
                Deaths = m.Sum(i => i.Deaths),
                Defaulters = m.Sum(i => i.Defaulters),
                NonCured = m.Sum(i => i.NonCured),
                ReferIn = m.Sum(i => i.ReferIn),
                tFemale = m.Sum(i => i.tFemale),
                tMale = m.Sum(i => i.tMale),
                totalbegin = m.Sum(i => i.totalbegin),
                Transfers = m.Sum(i => i.Transfers),
                Zscore23 = m.Sum(i => i.Zscore23),
                SFP_ALS = m.Sum(i => i.SFP_ALS),
                SFP_AWG = m.Sum(i => i.SFP_AWG),

            }).OrderBy(o => o.SFPID).ToList();

            if (data.Count == 0)
            {
                return RedirectToAction("District");
            }
            var facilityDetail = data.GroupBy(m => new
            {
                ProvCode = m.ProvCode,
                Province = m.Province,
                DistCode = m.DistCode,
                District = m.District,
                Implementer = m.Implementer,
            }).Select(m => new
            {
                ProvCode = m.Key.ProvCode,
                Province = m.Key.Province,
                DistCode = m.Key.DistCode,
                District = m.Key.District,
                Implementer = m.Key.Implementer,
                SFP_ALS = m.Average(n => n.SFP_ALS),
                SFP_AWG = m.Average(n => n.SFP_AWG)
            }).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            var mamstockdata = _context.Formatmamstockreports.AsNoTracking().Where(m => m.DistCode.Contains(DistCode) && m.Time >= TimeFrom && m.Time <= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g => new {
                Province = g.Province,
                ProvCode = g.ProvCode,
                DistCode = g.DistCode,
                stockID = g.stockID
            }).Select(m => new
            {
                Province = m.Key.Province,
                ProvCode = m.Key.ProvCode,
                DistCode = m.Key.DistCode,
                StockId = m.Key.stockID,
                ExpectedRecepients = m.Sum(i => i.ExpectedRecepients),
                Losses = m.Sum(i => i.Losses),
                OpeningBalance = m.Sum(i => i.OpeningBalance),
                QuantityDistributed = m.Sum(i => i.QuantityDistributed),
                QuantityReceived = m.Sum(i => i.QuantityReceived),
                QuantityReferin = m.Sum(i => i.QuantityReferin),
                QuantityReturned = m.Sum(i => i.QuantityReturned),
                QuantityTransferred = m.Sum(i => i.QuantityTransferred)

            }).OrderBy(o => o.StockId).ToList();

            foreach (var i in facilityDetail)
            {
                if (i.DistCode.Length > 0)
                {
                    worksheet.Range["C1"].Text = i.ProvCode + '-' + i.Province;
                    worksheet.Range["C2"].Text = i.DistCode + "-" + i.District;
                    worksheet.Range["C3"].Text = i.Implementer;
                    //worksheet.Range["R1"].Text = i.FacilityName;
                    //worksheet.Range["R2"].Text = i.FacilityId.ToString();
                    worksheet.Range["R3"].Text = MonthFrom + "/" + YearFrom;
                    worksheet.Range["R4"].Text = MonthTo + "/" + YearTo;
                    worksheet.Range["A18"].Number = i.SFP_ALS;
                    worksheet.Range["A18"].NumberFormat = "#,###.#";
                    worksheet.Range["E18"].Number = i.SFP_AWG;
                    worksheet.Range["E18"].NumberFormat = "#,###.#";

                }
            }

            int startRow = 9;
            foreach (var j in data)
            {
                if (j.DistCode.Length > 0)
                {
                    if (j.SFPID == 1)
                    {
                        worksheet.Range["C" + startRow].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;

                        worksheet.Range["D" + startRow].Number = j.Zscore23;
                        worksheet.Range["E" + startRow].Number = j.MUAC12;
                        worksheet.Range["G" + startRow].Number = j.tMale;
                        worksheet.Range["H" + startRow].Number = j.tFemale;
                        worksheet.Range["P" + startRow].Number = j.Transfers;
                        worksheet.Range["J" + startRow].Number = j.ReferIn;
                        worksheet.Range["M" + startRow].Number = j.Cured;
                        worksheet.Range["N" + startRow].Number = j.Deaths;
                        worksheet.Range["O" + startRow].Number = j.Defaulters;
                        worksheet.Range["Q" + startRow].Number = j.NonCured;
                    }
                    if (j.SFPID == 2)
                    {
                        worksheet.Range["C" + (startRow + 1)].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;

                        worksheet.Range["D" + (startRow + 1)].Number = j.Zscore23;
                        worksheet.Range["E" + (startRow + 1)].Number = j.MUAC12;
                        worksheet.Range["G" + (startRow + 1)].Number = j.tMale;
                        worksheet.Range["H" + (startRow + 1)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 1)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 1)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 1)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 1)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 1)].Number = j.Defaulters;
                        worksheet.Range["Q" + (startRow + 1)].Number = j.NonCured;
                    }
                    if (j.SFPID == 3)
                    {
                        worksheet.Range["C" + (startRow + 2)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow + 2)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow + 2)].Number = j.Absents;
                        worksheet.Range["H" + (startRow + 2)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 2)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 2)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 2)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 2)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 2)].Number = j.Defaulters;
                    }
                    if (j.SFPID == 4)
                    {
                        worksheet.Range["C" + (startRow + 3)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow + 3)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow + 3)].Number = j.Absents;
                        worksheet.Range["H" + (startRow + 3)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 3)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 3)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 3)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 3)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 3)].Number = j.Defaulters;
                    }
                    //startRow += 1;
                }
            }

            int stockRow = 20;
            foreach (var h in mamstockdata)
            {
                if (h.StockId == 1)
                {
                    worksheet.Range["L" + stockRow].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + stockRow].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + stockRow].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + stockRow].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + stockRow].Number = (double)h.Losses;
                    worksheet.Range["Q" + stockRow].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + stockRow].Number = (double)h.ExpectedRecepients;
                }
                if (h.StockId == 2)
                {
                    worksheet.Range["L" + (stockRow + 1)].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + (stockRow + 1)].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + (stockRow + 1)].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + (stockRow + 1)].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + (stockRow + 1)].Number = (double)h.Losses;
                    worksheet.Range["Q" + (stockRow + 1)].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + (stockRow+1)].Number = (double)h.ExpectedRecepients;
                }
                //stockRow += 1;
            }

            string ContentType = "Application/msexcel";
            string filename = "IMAM_" + "Report_TSFP_District" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

        //Province level
        public IActionResult ProvinceReport(FormatFilterReq req)
        {
            string ProveCode, DistCode;
            int FacilityId;
            int YearFrom;
            int YearTo;
            int MonthFrom;
            int MonthTo;
            int TimeFrom, TimeTo;
            string impcode;

            ProveCode = req.ProvCode;
            FacilityId = req.FacilityId;
            YearFrom = req.YearFrom;
            YearTo = req.YearTo;
            MonthFrom = req.MonthFrom;
            MonthTo = req.MonthTo;
            impcode = req.Implementer;
            DistCode = req.DistCode;

            TimeFrom = YearFrom * 100 + MonthFrom;
            TimeTo = YearTo * 100 + MonthTo;

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            FileStream inputStream = new FileStream(ResolveApplicationPath("TSFP_Province.xlsx"), FileMode.Open, FileAccess.Read);
            IWorkbook workbook = application.Workbooks.Open(inputStream);
            IWorksheet worksheet = workbook.Worksheets["TSFP"];
            this._context.Database.SetCommandTimeout(3000);
            var data = _context.Formatmamreports.AsNoTracking().Where(m => m.ProvCode.Contains(ProveCode) && m.Time >= TimeFrom && m.Time <= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g => new
            {
                Province = g.Province,
                ProvCode = g.ProvCode,
                SFPID = g.SFPID,
                Implementer = g.Implementer
            }).Select(m => new
            {
                Province = m.Key.Province,
                ProvCode = m.Key.ProvCode,
                Implementer = m.Key.Implementer,
                SFPID = m.Key.SFPID,
                MUAC12 = m.Sum(i => i.MUAC12),
                MUAC23 = m.Sum(i => i.MUAC23),
                Absents = m.Sum(i => i.Absents),
                Cured = m.Sum(i => i.Cured),
                Deaths = m.Sum(i => i.Deaths),
                Defaulters = m.Sum(i => i.Defaulters),
                NonCured = m.Sum(i => i.NonCured),
                ReferIn = m.Sum(i => i.ReferIn),
                tFemale = m.Sum(i => i.tFemale),
                tMale = m.Sum(i => i.tMale),
                totalbegin = m.Sum(i => i.totalbegin),
                Transfers = m.Sum(i => i.Transfers),
                Zscore23 = m.Sum(i => i.Zscore23),
                SFP_ALS = m.Sum(i => i.SFP_ALS),
                SFP_AWG = m.Sum(i => i.SFP_AWG),

            }).OrderBy(o => o.SFPID).ToList();

            if (data.Count == 0)
            {
                return RedirectToAction("Province");
            }

            var facilityDetail = data.GroupBy(m => new
            {
                ProvCode = m.ProvCode,
                Province = m.Province,
                Implementer = m.Implementer,
            }).Select(m => new
            {
                ProvCode = m.Key.ProvCode,
                Province = m.Key.Province,
                Implementer = m.Key.Implementer,
                SFP_ALS = m.Average(n => n.SFP_ALS),
                SFP_AWG = m.Average(n => n.SFP_AWG)
            }).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            var mamstockdata = _context.Formatmamstockreports.AsNoTracking().Where(m => m.ProvCode.Contains(ProveCode) && m.Time >= TimeFrom && m.Time <= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g => new {
                Province = g.Province,
                ProvCode = g.ProvCode,
                stockID = g.stockID
            }).Select(m => new
            {
                Province = m.Key.Province,
                ProvCode = m.Key.ProvCode,
                StockId = m.Key.stockID,
                ExpectedRecepients = m.Sum(i => i.ExpectedRecepients),
                Losses = m.Sum(i => i.Losses),
                OpeningBalance = m.Sum(i => i.OpeningBalance),
                QuantityDistributed = m.Sum(i => i.QuantityDistributed),
                QuantityReceived = m.Sum(i => i.QuantityReceived),
                QuantityReferin = m.Sum(i => i.QuantityReferin),
                QuantityReturned = m.Sum(i => i.QuantityReturned),
                QuantityTransferred = m.Sum(i => i.QuantityTransferred)

            }).OrderBy(o => o.StockId).ToList();

            foreach (var i in facilityDetail)
            {
                if (i.ProvCode.Length > 0)
                {
                    worksheet.Range["C1"].Text = i.ProvCode + '-' + i.Province;
                    //worksheet.Range["C2"].Text = i.DistCode + "-" + i.District;
                    worksheet.Range["C3"].Text = i.Implementer;
                    //worksheet.Range["R1"].Text = i.FacilityName;
                    //worksheet.Range["R2"].Text = i.FacilityId.ToString();
                    worksheet.Range["R3"].Text = MonthFrom + "/" + YearFrom;
                    worksheet.Range["R4"].Text = MonthTo + "/" + YearTo;
                    worksheet.Range["A18"].Number = i.SFP_ALS;
                    worksheet.Range["E18"].Number = i.SFP_AWG;

                }
            }

            int startRow = 9;
            foreach (var j in data)
            {
                if (j.ProvCode.Length > 0)
                {
                    if (j.SFPID == 1)
                    {
                        worksheet.Range["C" + startRow].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;

                        worksheet.Range["D" + startRow].Number = j.Zscore23;
                        worksheet.Range["E" + startRow].Number = j.MUAC12;
                        worksheet.Range["G" + startRow].Number = j.tMale;
                        worksheet.Range["H" + startRow].Number = j.tFemale;
                        worksheet.Range["P" + startRow].Number = j.Transfers;
                        worksheet.Range["J" + startRow].Number = j.ReferIn;
                        worksheet.Range["M" + startRow].Number = j.Cured;
                        worksheet.Range["N" + startRow].Number = j.Deaths;
                        worksheet.Range["O" + startRow].Number = j.Defaulters;
                        worksheet.Range["Q" + startRow].Number = j.NonCured;
                    }
                    if (j.SFPID == 2)
                    {
                        worksheet.Range["C" + (startRow + 1)].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;

                        worksheet.Range["D" + (startRow + 1)].Number = j.Zscore23;
                        worksheet.Range["E" + (startRow + 1)].Number = j.MUAC12;
                        worksheet.Range["G" + (startRow + 1)].Number = j.tMale;
                        worksheet.Range["H" + (startRow + 1)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 1)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 1)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 1)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 1)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 1)].Number = j.Defaulters;
                        worksheet.Range["Q" + (startRow + 1)].Number = j.NonCured;
                    }
                    if (j.SFPID == 3)
                    {
                        worksheet.Range["C" + (startRow + 2)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow + 2)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow + 2)].Number = j.Absents;
                        worksheet.Range["H" + (startRow + 2)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 2)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 2)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 2)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 2)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 2)].Number = j.Defaulters;
                    }
                    if (j.SFPID == 4)
                    {
                        worksheet.Range["C" + (startRow + 3)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow + 3)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow + 3)].Number = j.Absents;
                        worksheet.Range["H" + (startRow + 3)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 3)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 3)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 3)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 3)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 3)].Number = j.Defaulters;
                    }
                    //startRow += 1;
                }
            }

            int stockRow = 20;
            foreach (var h in mamstockdata)
            {
                if (h.StockId == 1)
                {
                    worksheet.Range["L" + stockRow].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + stockRow].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + stockRow].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + stockRow].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + stockRow].Number = (double)h.Losses;
                    worksheet.Range["Q" + stockRow].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + stockRow].Number = (double)h.ExpectedRecepients;
                }
                if (h.StockId == 2)
                {
                    worksheet.Range["L" + (stockRow + 1)].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + (stockRow + 1)].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + (stockRow + 1)].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + (stockRow + 1)].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + (stockRow + 1)].Number = (double)h.Losses;
                    worksheet.Range["Q" + (stockRow + 1)].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + (stockRow+1)].Number = (double)h.ExpectedRecepients;
                }
                //stockRow += 1;
            }

            string ContentType = "Application/msexcel";
            string filename = "IMAM_" + "Report_TSFP_Province" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

        //Province level
        public IActionResult PartnerReport(FormatFilterReqImp req)
        {
            int YearFrom;
            int YearTo;
            int MonthFrom;
            int MonthTo;
            int TimeFrom, TimeTo;
            string impcode;

            YearFrom = req.YearFrom;
            YearTo = req.YearTo;
            MonthFrom = req.MonthFrom;
            MonthTo = req.MonthTo;
            impcode = req.Implementer;

            TimeFrom = YearFrom * 100 + MonthFrom;
            TimeTo = YearTo * 100 + MonthTo;

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Instantiate the spreadsheet creation engine.
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            FileStream inputStream = new FileStream(ResolveApplicationPath("TSFP_Implementer.xlsx"), FileMode.Open, FileAccess.Read);
            IWorkbook workbook = application.Workbooks.Open(inputStream);
            IWorksheet worksheet = workbook.Worksheets["TSFP"];
            this._context.Database.SetCommandTimeout(3000);
            var data = _context.Formatmamreports.AsNoTracking().Where(m => m.Implementer.Contains(impcode) && m.Time >= TimeFrom && m.Time <= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g => new
            {
                SFPID = g.SFPID,
                Implementer = g.Implementer
            }).Select(m => new
            {
                Implementer = m.Key.Implementer,
                SFPID = m.Key.SFPID,
                MUAC12 = m.Sum(i => i.MUAC12),
                MUAC23 = m.Sum(i => i.MUAC23),
                Absents = m.Sum(i => i.Absents),
                Cured = m.Sum(i => i.Cured),
                Deaths = m.Sum(i => i.Deaths),
                Defaulters = m.Sum(i => i.Defaulters),
                NonCured = m.Sum(i => i.NonCured),
                ReferIn = m.Sum(i => i.ReferIn),
                tFemale = m.Sum(i => i.tFemale),
                tMale = m.Sum(i => i.tMale),
                totalbegin = m.Sum(i => i.totalbegin),
                Transfers = m.Sum(i => i.Transfers),
                Zscore23 = m.Sum(i => i.Zscore23),
                SFP_ALS = m.Sum(i => i.SFP_ALS),
                SFP_AWG = m.Sum(i => i.SFP_AWG),

            }).OrderBy(o => o.SFPID).ToList();

            if (data.Count==0)
            {
                //return new ContentResult
                //{
                //    //ContentType = "text/html",
                //    //StatusCode = (int)HttpStatusCode.OK,
                //    //Content = "<a href=/FacilityReport/Partner> Please change your criteria </a>"
                //    return View();
                //};
                ViewBag.noreport = "No report found. Please your criteria";
                return RedirectToAction("Partner");
                //return Content("No data returned. <a href=$@'/FacilityReport/Partner'> Please change your criteria </a>");
            }
                
            var facilityDetail = data.GroupBy(m => new
            {
                Implementer = m.Implementer,
            }).Select(m => new
            {
                Implementer = m.Key.Implementer,
                SFP_ALS = m.Average(n => n.SFP_ALS),
                SFP_AWG = m.Average(n => n.SFP_AWG)
            }).ToList();

            if (!data.Any())
            {
                return BadRequest();
            }

            var mamstockdata = _context.Formatmamstockreports.AsNoTracking().Where(m => m.Implementer.Contains(impcode) && m.Time >= TimeFrom && m.Time <= TimeTo && m.Implementer.Contains(impcode)).GroupBy(g => new {
                Province = g.Province,
                ProvCode = g.ProvCode,
                stockID = g.stockID
            }).Select(m => new
            {
                Province = m.Key.Province,
                ProvCode = m.Key.ProvCode,
                StockId = m.Key.stockID,
                ExpectedRecepients = m.Sum(i => i.ExpectedRecepients),
                Losses = m.Sum(i => i.Losses),
                OpeningBalance = m.Sum(i => i.OpeningBalance),
                QuantityDistributed = m.Sum(i => i.QuantityDistributed),
                QuantityReceived = m.Sum(i => i.QuantityReceived),
                QuantityReferin = m.Sum(i => i.QuantityReferin),
                QuantityReturned = m.Sum(i => i.QuantityReturned),
                QuantityTransferred = m.Sum(i => i.QuantityTransferred)

            }).OrderBy(o => o.StockId).ToList();

            foreach (var i in facilityDetail)
            {
                if (i.Implementer.Length > 0)
                {
                    //worksheet.Range["C1"].Text = i.ProvCode + '-' + i.Province;
                    //worksheet.Range["C2"].Text = i.DistCode + "-" + i.District;
                    worksheet.Range["C3"].Text = i.Implementer;
                    //worksheet.Range["R1"].Text = i.FacilityName;
                    //worksheet.Range["R2"].Text = i.FacilityId.ToString();
                    worksheet.Range["R3"].Text = MonthFrom + "/" + YearFrom;
                    worksheet.Range["R4"].Text = MonthTo + "/" + YearTo;
                    worksheet.Range["A18"].Number = i.SFP_ALS;
                    worksheet.Range["E18"].Number = i.SFP_AWG;

                }
            }

            int startRow = 9;
            foreach (var j in data)
            {
                if (j.Implementer.Length > 0)
                {
                    if (j.SFPID == 1)
                    {
                        worksheet.Range["C" + startRow].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;

                        worksheet.Range["D" + startRow].Number = j.Zscore23;
                        worksheet.Range["E" + startRow].Number = j.MUAC12;
                        worksheet.Range["G" + startRow].Number = j.tMale;
                        worksheet.Range["H" + startRow].Number = j.tFemale;
                        worksheet.Range["P" + startRow].Number = j.Transfers;
                        worksheet.Range["J" + startRow].Number = j.ReferIn;
                        worksheet.Range["M" + startRow].Number = j.Cured;
                        worksheet.Range["N" + startRow].Number = j.Deaths;
                        worksheet.Range["O" + startRow].Number = j.Defaulters;
                        worksheet.Range["Q" + startRow].Number = j.NonCured;
                    }
                    if (j.SFPID == 2)
                    {
                        worksheet.Range["C" + (startRow + 1)].Number = j.totalbegin;
                        //worksheet.Range["F" + startRow].Number = j.MUAC23;
                        //worksheet.Range["L" + startRow].Number = j.Absents;

                        worksheet.Range["D" + (startRow + 1)].Number = j.Zscore23;
                        worksheet.Range["E" + (startRow + 1)].Number = j.MUAC12;
                        worksheet.Range["G" + (startRow + 1)].Number = j.tMale;
                        worksheet.Range["H" + (startRow + 1)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 1)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 1)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 1)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 1)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 1)].Number = j.Defaulters;
                        worksheet.Range["Q" + (startRow + 1)].Number = j.NonCured;
                    }
                    if (j.SFPID == 3)
                    {
                        worksheet.Range["C" + (startRow + 2)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow + 2)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow + 2)].Number = j.Absents;
                        worksheet.Range["H" + (startRow + 2)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 2)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 2)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 2)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 2)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 2)].Number = j.Defaulters;
                    }
                    if (j.SFPID == 4)
                    {
                        worksheet.Range["C" + (startRow + 3)].Number = j.totalbegin;
                        worksheet.Range["F" + (startRow + 3)].Number = j.MUAC23;
                        worksheet.Range["L" + (startRow + 3)].Number = j.Absents;
                        worksheet.Range["H" + (startRow + 3)].Number = j.tFemale;
                        worksheet.Range["P" + (startRow + 3)].Number = j.Transfers;
                        worksheet.Range["J" + (startRow + 3)].Number = j.ReferIn;
                        worksheet.Range["M" + (startRow + 3)].Number = j.Cured;
                        worksheet.Range["N" + (startRow + 3)].Number = j.Deaths;
                        worksheet.Range["O" + (startRow + 3)].Number = j.Defaulters;
                    }
                    //startRow += 1;
                }
            }

            int stockRow = 20;
            foreach (var h in mamstockdata)
            {
                if (h.StockId == 1)
                {
                    worksheet.Range["L" + stockRow].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + stockRow].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + stockRow].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + stockRow].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + stockRow].Number = (double)h.Losses;
                    worksheet.Range["Q" + stockRow].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + stockRow].Number = (double)h.ExpectedRecepients;
                }
                if (h.StockId == 2)
                {
                    worksheet.Range["L" + (stockRow + 1)].Number = (double)h.OpeningBalance;
                    worksheet.Range["M" + (stockRow + 1)].Number = (double)h.QuantityReceived;
                    worksheet.Range["N" + (stockRow + 1)].Number = (double)h.QuantityDistributed;
                    worksheet.Range["O" + (stockRow + 1)].Number = (double)h.QuantityTransferred;
                    worksheet.Range["P" + (stockRow + 1)].Number = (double)h.Losses;
                    worksheet.Range["Q" + (stockRow + 1)].Number = (double)h.QuantityReturned;
                    //worksheet.Range["S" + (stockRow+1)].Number = (double)h.ExpectedRecepients;
                }
                //stockRow += 1;
            }

            string ContentType = "Application/msexcel";
            string filename = "IMAM_" + "Report_TSFP_Implementer" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }
        private string ResolveApplicationPath(string fileName)
        {
            return _hostingEnvironment.WebRootPath + "//Template//" + fileName;
        }
    }
}

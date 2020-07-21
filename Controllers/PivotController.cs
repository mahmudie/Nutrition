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

namespace DataSystem.Controllers
{
    [Authorize]
    public class PivotController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;

        public PivotController(WebNutContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _context.Database.SetCommandTimeout(500);
        }
        public IActionResult Index()
        {
            List<Provinces> Povinces = _context.Provinces.ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "All" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");
            return View();
        }

        public IActionResult pivotMam([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IQueryable<TblMam> get = _context.TblMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women")
            && !m.Mam.AgeGroup.ToLower().Contains("total"));
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("women") && !m.Mam.AgeGroup.ToLower().Contains("total") && m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }
            var data = get.AsNoTracking().Select(m => new dummy()
            {
 totalbegin = m.Totalbegin.GetValueOrDefault(),
                Zscore23 = m.Zscore23.GetValueOrDefault(),
                MUAC12 = m.Muac12.GetValueOrDefault(),
                MUAC23 = m.Muac23.GetValueOrDefault(),
                ReferIn = m.ReferIn.GetValueOrDefault(),
                TotalAdmissions = m.Muac23.GetValueOrDefault() + m.ReferIn.GetValueOrDefault(),
                Cured = m.Cured.GetValueOrDefault(),
                Deaths = m.Deaths.GetValueOrDefault(),
                Defaulters = m.Defaulters.GetValueOrDefault(),
                Transfers = m.Transfers.GetValueOrDefault(),
                NonCured = m.NonCured.GetValueOrDefault(),
                TotalExists = m.Cured.GetValueOrDefault() + m.Deaths.GetValueOrDefault() + m.Defaulters.GetValueOrDefault() + m.NonCured.GetValueOrDefault() + m.Transfers.GetValueOrDefault(),
                TotalExitsNoRefOuts=m.Cured.GetValueOrDefault() + m.Deaths.GetValueOrDefault() + m.Defaulters.GetValueOrDefault() + m.NonCured.GetValueOrDefault(),
                Total_food_recepients =m.Totalbegin.GetValueOrDefault()+m.Muac23.GetValueOrDefault() + m.ReferIn.GetValueOrDefault() - m.Absents.GetValueOrDefault() - m.Deaths.GetValueOrDefault() - m.Transfers.GetValueOrDefault()-m.Defaulters.GetValueOrDefault(),
                Total_end_month = m.Totalbegin.GetValueOrDefault()+ m.Muac23.GetValueOrDefault() + m.ReferIn.GetValueOrDefault() - ( m.Cured.GetValueOrDefault() + m.Deaths.GetValueOrDefault() + m.Defaulters.GetValueOrDefault() + m.NonCured.GetValueOrDefault() + m.Transfers.GetValueOrDefault()),
                //tMale = m.TMale.GetValueOrDefault(),
                tFemale = m.TFemale.GetValueOrDefault(),
                FacilityID = m.Nmr.FacilityId,
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                AgeGroups = m.Mam.AgeGroup,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                ProvCode = m.Nmr.Facility.DistNavigation.ProvCode,

            }).ToList();

            if (!req.Year.Equals(0))
            {
                data = data.Where(m => m.Year.Equals(req.Year)).ToList();
            }

            if (!data.Any())
            {
                return BadRequest();
            }

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook = application.Workbooks.Create(2);

            IWorksheet sheet = workbook.Worksheets[0];
            sheet.ImportData(data, 1, 1, true);
            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";

            pivotSheet["A2"].Text = "OPD-MAM - PLW";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["AgeGroups"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField totalbegin = pivotTable.Fields["totalbegin"];
                // IPivotField Zscore23 = pivotTable.Fields["Zscore23"];
                // IPivotField MUAC12 = pivotTable.Fields["MUAC12"];
                IPivotField MUAC23 = pivotTable.Fields["MUAC23"];
                IPivotField ReferIn = pivotTable.Fields["ReferIn"];
                IPivotField TotalAdmissions = pivotTable.Fields["TotalAdmissions"];
                IPivotField Cured = pivotTable.Fields["Cured"];
                IPivotField Deaths = pivotTable.Fields["Deaths"];
                IPivotField Defaulters = pivotTable.Fields["Defaulters"];
                IPivotField Transfers = pivotTable.Fields["Transfers"];
                IPivotField NonCured = pivotTable.Fields["NonCured"];
                IPivotField TotalExists = pivotTable.Fields["TotalExists"];
                IPivotField TotalExitsNoRefOuts=pivotTable.Fields["TotalExitsNoRefOuts"];
                IPivotField Total_food_recepients = pivotTable.Fields["Total_food_recepients"];
                IPivotField Total_end_month = pivotTable.Fields["Total_end_month"];
                //IPivotField tmale = pivotTable.Fields["tMale"];
                IPivotField tFemale = pivotTable.Fields["tFemale"];

                pivotTable.DataFields.Add(totalbegin, "Total at beginning", PivotSubtotalTypes.Sum);
                // pivotTable.DataFields.Add(Zscore23, "W/H < -2 to -3 Z score", PivotSubtotalTypes.Sum);
                // pivotTable.DataFields.Add(MUAC12, "MUAC >= 11,5 < 12,5cm", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MUAC23, "MUAC < 23cm", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(ReferIn, "Referred In", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalAdmissions, "Total Admissions", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Cured, "Cured ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Deaths, "Deaths ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Defaulters, "Defaulters ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Transfers, "Transfers ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(NonCured, "Non-Cured", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalExists, "Total Exists", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Total_food_recepients, "Total food recipients", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Total_end_month, "Total end  of month", PivotSubtotalTypes.Sum);
                //pivotTable.DataFields.Add(tmale, "Male ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(tFemale, "Female ", PivotSubtotalTypes.Sum);

                IPivotField pCurrate = pivotTable.CalculatedFields.Add("Curedd1", "Cured/TotalExitsNoRefOuts");
                pCurrate.Name = "%cured";
                pCurrate.NumberFormat = "#.0%";

                IPivotField pDeath = pivotTable.CalculatedFields.Add("Death1", "Deaths/TotalExitsNoRefOuts");
                pDeath.Name = "%death";
                pDeath.NumberFormat = "#.0%";

                IPivotField pDefaulters = pivotTable.CalculatedFields.Add("Defaulter1", "Defaulters/TotalExitsNoRefOuts");
                pDefaulters.Name = "%defaulters";
                pDefaulters.NumberFormat = "#.0%";

                //IPivotField pTransfers = pivotTable.CalculatedFields.Add("Transfers ", "Transfers/TotalExists*100");
                //pTransfers.Name = "%transfers";
                //pTransfers.NumberFormat = "#.0";

                IPivotField pNonCured = pivotTable.CalculatedFields.Add("NonCured1", "NonCured/TotalExitsNoRefOuts");
                pNonCured.Name = "%NonCured";
                pNonCured.NumberFormat = "#.0%";

                // IPivotField pMale = pivotTable.CalculatedFields.Add("Male", "tmale/TotalExists*100");
                // pMale.Name = "%male";
                // pMale.NumberFormat = "#.0";

                // IPivotField pFemale = pivotTable.CalculatedFields.Add("Female", "tFemale/TotalExists*100");
                // pFemale.Name = "%female";
                // pFemale.NumberFormat = "#.0";
            }
            IPivotTableOptions option = pivotTable.Options;
            option.ErrorString = "X";
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;
            pivotSheet.Activate();

            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "OPD-MAM_Women" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

        public IActionResult pivotMamChild([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IQueryable<TblMam> get = _context.TblMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children") && !m.Mam.AgeGroup.ToLower().Contains("total"));
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblMam.Where(m => m.Mam.AgeGroup.ToLower().Contains("children") && !m.Mam.AgeGroup.ToLower().Contains("total") && m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }
            var data = get.AsNoTracking().Select(m => new dummy()
            {
                totalbegin = m.Totalbegin.GetValueOrDefault(),
                Zscore23 = m.Zscore23.GetValueOrDefault(),
                MUAC12 = m.Muac12.GetValueOrDefault(),
                MUAC23 = m.Muac23.GetValueOrDefault(),
                ReferIn = m.ReferIn.GetValueOrDefault(),
                TotalAdmissions =  m.Zscore23.GetValueOrDefault() + m.Muac12.GetValueOrDefault() + m.ReferIn.GetValueOrDefault(),
                Cured = m.Cured.GetValueOrDefault(),
                Deaths = m.Deaths.GetValueOrDefault(),
                Defaulters = m.Defaulters.GetValueOrDefault(),
                Transfers = m.Transfers.GetValueOrDefault(),
                NonCured = m.NonCured.GetValueOrDefault(),
                TotalExists = m.Cured.GetValueOrDefault() + m.Deaths.GetValueOrDefault() + m.Defaulters.GetValueOrDefault() + m.NonCured.GetValueOrDefault() + m.Transfers.GetValueOrDefault(),
                TotalExitsNoRefOuts=m.Cured.GetValueOrDefault() + m.Deaths.GetValueOrDefault() + m.Defaulters.GetValueOrDefault() + m.NonCured.GetValueOrDefault(),
                Total_end_month = m.Totalbegin.GetValueOrDefault()+ m.Zscore23.GetValueOrDefault() + m.Muac12.GetValueOrDefault() +
                m.ReferIn.GetValueOrDefault() - ( m.Cured.GetValueOrDefault() + m.Deaths.GetValueOrDefault() + m.Defaulters.GetValueOrDefault() + m.NonCured.GetValueOrDefault() + m.Transfers.GetValueOrDefault()),
                tMale = m.TMale.GetValueOrDefault(),
                tFemale = m.TFemale.GetValueOrDefault(),
                FacilityID = m.Nmr.FacilityId,
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                AgeGroups = m.Mam.AgeGroup,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                ProvCode = m.Nmr.Facility.DistNavigation.ProvCode,

            }).ToList();


            if (!data.Any())
            {
                return BadRequest();
            }
            if (!req.Year.Equals(0))
            {
                data = data.Where(m => m.Year.Equals(req.Year)).ToList();
            }



            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook = application.Workbooks.Create(2);

            IWorksheet sheet = workbook.Worksheets[0];
            sheet.ImportData(data, 1, 1, true);
            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";

            pivotSheet["A2"].Text = "OPD-MAM Under 5";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["AgeGroups"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField totalbegin = pivotTable.Fields["totalbegin"];
                IPivotField Zscore23 = pivotTable.Fields["Zscore23"];
                IPivotField MUAC12 = pivotTable.Fields["MUAC12"];
                IPivotField MUAC23 = pivotTable.Fields["MUAC23"];
                IPivotField ReferIn = pivotTable.Fields["ReferIn"];
                IPivotField TotalAdmissions = pivotTable.Fields["TotalAdmissions"];
                IPivotField Cured = pivotTable.Fields["Cured"];
                IPivotField Deaths = pivotTable.Fields["Deaths"];
                IPivotField Defaulters = pivotTable.Fields["Defaulters"];
                IPivotField Transfers = pivotTable.Fields["Transfers"];
                IPivotField NonCured = pivotTable.Fields["NonCured"];
                IPivotField TotalExists = pivotTable.Fields["TotalExists"];
                IPivotField TotalExitsNoRefOuts = pivotTable.Fields["TotalExitsNoRefOuts"];
                IPivotField Total_end_month = pivotTable.Fields["Total_end_month"];
                IPivotField tmale = pivotTable.Fields["tMale"];
                IPivotField tFemale = pivotTable.Fields["tFemale"];

                pivotTable.DataFields.Add(totalbegin, "Total at beginning", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Zscore23, "W/H < -2 to -3 Z score", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MUAC12, "MUAC >= 11,5 < 12,5cm", PivotSubtotalTypes.Sum);
                // pivotTable.DataFields.Add(MUAC23, "MUAC < 23cm", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(ReferIn, "Referred In", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalAdmissions, "Total Admissions", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Cured, "Cured ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Deaths, "Deaths ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Defaulters, "Defaulters ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Transfers, "Transfers ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(NonCured, "Non-Cured", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalExists, "Total Exists", PivotSubtotalTypes.Sum);
                // pivotTable.DataFields.Add(Total_food_recepients, "Total food recipients", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Total_end_month, "Total end  of month", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(tmale, "Male ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(tFemale, "Female ", PivotSubtotalTypes.Sum);

                IPivotField pCurrate = pivotTable.CalculatedFields.Add("Curedd2", "Cured/TotalExitsNoRefOuts");
                pCurrate.Name = "%cured";
                pCurrate.NumberFormat = "#.0%";

                IPivotField pDeath = pivotTable.CalculatedFields.Add("Death2", "Deaths/TotalExitsNoRefOuts");
                pDeath.Name = "%death";
                pDeath.NumberFormat = "#.0%";

                IPivotField pDefaulters = pivotTable.CalculatedFields.Add("Defaulter2", "Defaulters/TotalExitsNoRefOuts");
                pDefaulters.Name = "%defaulter";
                pDefaulters.NumberFormat = "#.0%";

                //IPivotField pTransfers = pivotTable.CalculatedFields.Add("Transfers ", "Transfers/TotalExists*100");
                //pTransfers.Name = "%transfers";
                //pTransfers.NumberFormat = "#.0";

                IPivotField pNonCured = pivotTable.CalculatedFields.Add("NonCured2", "NonCured/TotalExitsNoRefOuts");
                pNonCured.Name = "%NonCured";
                pNonCured.NumberFormat = "#.0%";

                // IPivotField pMale = pivotTable.CalculatedFields.Add("Male", "tmale/TotalExists*100");
                // pMale.Name = "%male";
                // pMale.NumberFormat = "#.0";

                // IPivotField pFemale = pivotTable.CalculatedFields.Add("Female", "tFemale/TotalExists*100");
                // pFemale.Name = "%female";
                // pFemale.NumberFormat = "#.0";
            }

            IPivotTableOptions option = pivotTable.Options;
            option.ErrorString = "X";
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;
            pivotSheet.Activate();

            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "OPDMAM_Children" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();

            return File(ms, ContentType, filename);

        }

        public IActionResult pivotIYCF([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<TblIycf> get = _context.TblIycf;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblIycf.Where(m => m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }
            var data = get.AsNoTracking().Select(m => new Iycf()
            {
                FacilityID = m.Nmr.FacilityId,
                Causes = m.Iycf.CauseShortName,
                mChildU5months = m.MChildU5months.GetValueOrDefault(),
                mChild524months = m.MChild524months.GetValueOrDefault(),
                pregnanatwomen = m.Pregnanatwomen.GetValueOrDefault(),
                Firstvisit = m.Firstvisit.GetValueOrDefault(),
                Revisit = m.Revisit.GetValueOrDefault(),
                ReferIn = m.ReferIn.GetValueOrDefault(),
                ReferOut = m.ReferOut.GetValueOrDefault(),
                TotalWomen = m.MChildU5months.GetValueOrDefault() + m.MChild524months.GetValueOrDefault() +
                m.Pregnanatwomen.GetValueOrDefault(),
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName
            }).ToList();



            if (!data.Any())
            {
                return NotFound();
            }

            if (!req.Year.Equals(0))
            {
                data = data.Where(m => m.Year == req.Year).ToList();
            }
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];
            sheet.ImportData(data, 1, 1, true);

            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";


            pivotSheet["A2"].Text = "Infant and Young Child Feeding";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Causes"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField mChildU5months = pivotTable.Fields["mChildU5months"];
                IPivotField mChild524months = pivotTable.Fields["mChild524months"];
                IPivotField pregnanatwomen = pivotTable.Fields["pregnanatwomen"];
                IPivotField TotalWomen = pivotTable.Fields["TotalWomen"];
                IPivotField Firstvisit = pivotTable.Fields["Firstvisit"];
                IPivotField Revisit = pivotTable.Fields["Revisit"];
                IPivotField ReferIn = pivotTable.Fields["ReferIn"];
                IPivotField ReferOut = pivotTable.Fields["ReferOut"];

                pivotTable.DataFields.Add(mChildU5months, "Mother with Children< 6 month", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(mChild524months, "Mother with Children 6-24moth  ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(pregnanatwomen, "Pregnant women ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalWomen, "Total ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Firstvisit, "First visit  ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Revisit, "Re-visit", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(ReferIn, "Refer in ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(ReferOut, "Refer out ", PivotSubtotalTypes.Sum);

            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;

            }

            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;

            pivotSheet.Activate();
            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "IYCF_Pivot" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

        public IActionResult pivotMN([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<TblMn> get = _context.TblMn;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblMn.Where(m => m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];

            var data = get.AsNoTracking().Select(m => new Mn()
            {
                FacilityID = m.Nmr.FacilityId,
                chu2m = m.chu2m.GetValueOrDefault(),
                chu2f = m.chu2f.GetValueOrDefault(),
                refbyCHW = m.refbyCHW.GetValueOrDefault(),
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                MNItems = m.Mn.Mnitems,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName
            }).ToList();

            if (!data.Any())
            {
                return NotFound();
            }

            if (!req.Year.Equals(0))
            {
                data = data.Where(m => m.Year == req.Year).ToList();
            }
            sheet.ImportData(data, 1, 1, true);

            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";

            pivotSheet["A2"].Text = "GPM (Health Facility)";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["MNItems"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField chu2m = pivotTable.Fields["chu2m"];
                IPivotField chu2f = pivotTable.Fields["chu2f"];
                IPivotField refbyChw = pivotTable.Fields["refbyCHW"];

                pivotTable.DataFields.Add(chu2m, "Children U2 Male", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(chu2f, "Children U2 Female", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(refbyChw, "Referred by CHWs", PivotSubtotalTypes.Sum);

            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;
            }

            IPivotTableOptions option = pivotTable.Options;
            option.ErrorString = "X";
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;

            pivotSheet.Activate();

            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "GMPHealthFacility" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);
        }



        public IActionResult pivotSubmission([Bind("ProvCode,Year,option")]CreateReq req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<nmrsubmission> get = _context.nmrsubmission;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.nmrsubmission.Where(m => m.ProvCode.Equals(req.ProvCode));
            }
            var data = _context.nmrsubmission.FromSql("Select * from NMR_submission").ToList();
            if (!data.Any())
            {
                return NotFound();
            }

            if (!req.Year.Equals(0))
            {
                data = data.Where(m => m.Year.Equals(req.Year)).ToList();
            }

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];
             sheet.ImportData(data, 1, 1, true);
            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";

            pivotSheet["A2"].Text = "NMR Submission";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField SAM_Inpatient_submission = pivotTable.Fields["IPDSAM_submission"];
                IPivotField SAM_Outpatient_submission = pivotTable.Fields["OPDSAM_submission"];
                IPivotField MAM_submission = pivotTable.Fields["OPDMAM_submission"];
                //IPivotField IYCF_submission = pivotTable.Fields["IYCF_submission"];
                IPivotField MNS_submission = pivotTable.Fields["MNS_submission"];
                IPivotField MAM_Stock_submission = pivotTable.Fields["OPDMAM_stock_submission"];
                IPivotField SAM_Inpatient_stock_submission = pivotTable.Fields["IPDSAM_stock_submission"];
                IPivotField SAM_outpatient_stock_submission = pivotTable.Fields["OPDSAM_stock_submission"];


                pivotTable.DataFields.Add(SAM_Inpatient_submission, "IPD-SAM submission", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(SAM_Outpatient_submission, "OPD-SAM submission", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MAM_submission, "OPD-MAM submission", PivotSubtotalTypes.Sum);
                //pivotTable.DataFields.Add(IYCF_submission, "IYCF submission", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MNS_submission, "GM submission", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(SAM_Inpatient_stock_submission, "IPD-SAM stock submission", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(SAM_outpatient_stock_submission, "OPD-SAM stock submission", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MAM_Stock_submission, "OPD-MAM Stock submission", PivotSubtotalTypes.Sum);
                pivotTable.ShowDataFieldInRow = true;
            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;
            }

            IPivotTableOptions option = pivotTable.Options;
            option.ErrorString = "X";
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;

            pivotSheet.Activate();
            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "submission" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

        public IActionResult pivotBNA([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<Nmr> get = _context.Nmr;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.Nmr.Where(m => m.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }
            var data = get.AsNoTracking().Select(m => new bnaPivot()
            {
                Girls = m.GirlsScreened.GetValueOrDefault(),
                Boys = m.BoysScreened.GetValueOrDefault(),
                Plwr = m.Plwreported.GetValueOrDefault(),
                ipdRutf = m.IpdRutfstockOutWeeks.GetValueOrDefault(),
                ipdAdmission = m.IpdAdmissionsByChws.GetValueOrDefault(),
                opdRutf = m.OpdRutfstockOutWeeks.GetValueOrDefault(),
                opdAdmission = m.OpdAdmissionsByChws.GetValueOrDefault(),
                mamRutf = m.MamRusfstockoutWeeks.GetValueOrDefault(),
                mamAdmission = m.MamAddminsionByChws.GetValueOrDefault(),
                FacilityName = m.Facility.FacilityName,
                FacilityType = m.Facility.FacilityTypeNavigation.FacType,
                Year = m.Year,
                Month = m.Month,
                mYear = m.Month < 10 ? (m.Year + 621) : (m.Year + 622),
                mMonth = m.Month < 10 ? (m.Month + 3) : (m.Month - 9),
                District = m.Facility.DistNavigation.DistName,
                Implementer = m.Implementer,
                Province = m.Facility.DistNavigation.ProvCodeNavigation.ProvName
            }).ToList();
            if (!data.Any())
            {
                return NotFound();
            }

            if (!req.Year.Equals(0))
            {
                data = data.Where(m => m.Year.Equals(req.Year)).ToList();
            }
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];
            sheet.ImportData(data, 1, 1, true);
            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";

            pivotSheet["A2"].Text = "BNA questions";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField IPD_RUTFStockOut_weeks = pivotTable.Fields["ipdRutf"];
                IPivotField IPD_AdmissionsByCHWs = pivotTable.Fields["ipdAdmission"];

                IPivotField OPD_RUTFStockOut_weeks = pivotTable.Fields["opdRutf"];
                IPivotField OPD_AdmissionsByCHWs = pivotTable.Fields["opdAdmission"];

                IPivotField MAM_RUSFstockout_weeks = pivotTable.Fields["mamRutf"];
                IPivotField MAM_addminsionByCHWS = pivotTable.Fields["mamAdmission"];


                IPivotField GirlsScreened = pivotTable.Fields["Girls"];
                IPivotField BoysScreened = pivotTable.Fields["Boys"];
                IPivotField PLWReported = pivotTable.Fields["Plwr"];

                pivotTable.DataFields.Add(IPD_RUTFStockOut_weeks, "1- SAM inpatient-how many weeks did the site encountered stock out of RUTF", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(IPD_AdmissionsByCHWs, "2- SAM inpatient-number of new admissions referred by CHWs", PivotSubtotalTypes.Sum);

                pivotTable.DataFields.Add(OPD_RUTFStockOut_weeks, "3- SAM outpatient-how many weeks did the site encountered stock out of RUTF", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(OPD_AdmissionsByCHWs, "4- SAM outpatient-number of new admissions referred by CHWs", PivotSubtotalTypes.Sum);


                pivotTable.DataFields.Add(MAM_RUSFstockout_weeks, "5- MAM-how many weeks did the site encountered stock out of RUSF", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MAM_addminsionByCHWS, "6- MAM-number of new admissions referred by CHWs", PivotSubtotalTypes.Sum);

                pivotTable.DataFields.Add(GirlsScreened, "9- How many children Under 5 years and PLW screened - Girls ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(BoysScreened, "10- How many children Under 5 years and PLW screened - Boys", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(PLWReported, "11- How many children Under 5 years and PLW screened - PLW", PivotSubtotalTypes.Sum);
                pivotTable.ShowDataFieldInRow = true;

            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;
            }
            IPivotTableOptions option = pivotTable.Options;
            option.ErrorString = "X";
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;

            pivotSheet.Activate();
            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "BNAQ" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);
        }

        public IActionResult pivotMAMstock([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            IQueryable<TblFstock> get = _context.TblFstock;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblFstock.Where(m => m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }

            var data = get.AsNoTracking().Select(m => new mamStock()
            {
                OpeningBalance = m.OpeningBalance.GetValueOrDefault(),
                QuantityReceived = m.QuantityReceived.GetValueOrDefault(),
                QuantityDistributed = m.QuantityDistributed.GetValueOrDefault(),
                QuantityReferredIn= m.QuantityReferin.GetValueOrDefault(),
                QuantityTransferout = m.QuantityTransferred.GetValueOrDefault(),
                Losses = m.Losses.GetValueOrDefault(),
                QuantityReturned = m.QuantityReturned.GetValueOrDefault(),
                ClosingBalance = (m.OpeningBalance.GetValueOrDefault() + m.QuantityReceived.GetValueOrDefault()+m.QuantityReferin.GetValueOrDefault()) -
             (m.QuantityDistributed.GetValueOrDefault() + m.QuantityTransferred.GetValueOrDefault() + m.Losses.GetValueOrDefault()
             + m.QuantityReturned.GetValueOrDefault()),
                ExpectedRecipients = m.ExpectedRecepients.GetValueOrDefault(),
                Quantityneeded = (m.ExpectedRecepients.GetValueOrDefault() * m.Stock.DistAmountKg.GetValueOrDefault())
             - (m.OpeningBalance.GetValueOrDefault() + m.QuantityReceived.GetValueOrDefault() -
             m.QuantityDistributed.GetValueOrDefault() - m.QuantityTransferred.GetValueOrDefault() - m.Losses.GetValueOrDefault()
             - m.QuantityReturned.GetValueOrDefault()),
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                Items = m.Stock.Item

            }).ToList();

            if (!data.Any())
            {
                return NotFound();
            }

            if (!req.Year.Equals(0))
            {
                data = data.Where(m => m.Year.Equals(req.Year)).ToList();
            }

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];
            sheet.ImportData(data, 1, 1, true);
            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";

            pivotSheet["A2"].Text = "Food stock report and balance (Kg)";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Items"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField Openingbalance = pivotTable.Fields["OpeningBalance"];
                IPivotField QuantityReceived = pivotTable.Fields["QuantityReceived"];
                IPivotField QuantityDistributed = pivotTable.Fields["QuantityDistributed"];
                IPivotField QuantityReferredIn = pivotTable.Fields["QuantityReferredIn"];
                IPivotField QuantityTransferout = pivotTable.Fields["QuantityTransferout"];
                IPivotField Losses = pivotTable.Fields["Losses"];
                IPivotField QuantityReturned = pivotTable.Fields["QuantityReturned"];
                IPivotField ClosingBalance = pivotTable.Fields["ClosingBalance"];
                IPivotField ExpectedRecipients = pivotTable.Fields["ExpectedRecipients"];
                IPivotField Quantityneeded = pivotTable.Fields["Quantityneeded"];

                pivotTable.DataFields.Add(Openingbalance, "Opening Balance", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(QuantityReceived, "Quantity Received  ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(QuantityDistributed, "Quantity Distributed ", PivotSubtotalTypes.Sum);
                 pivotTable.DataFields.Add(QuantityReferredIn, "Quantity ReferredIn ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(QuantityTransferout, "Quantity Transferout ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Losses, "Losses  ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(QuantityReturned, "Quantity Returned ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(ClosingBalance, "Closing Balance ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(ExpectedRecipients, "Expected Recipients ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Quantityneeded, "Quantity Needed ", PivotSubtotalTypes.Sum);

            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;
            }
            IPivotTableOptions option = pivotTable.Options;
            option.ErrorString = "X";           
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;

            pivotSheet.Activate();
            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "OPDMAMStock" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }


        public IActionResult samStock([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<TblStockIpt> get = _context.TblStockIpt;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblStockIpt.Where(m => m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }

            var data = get.AsNoTracking().Select(m => new samStockPivot()
            {
                Balance =m.Openingbalance.GetValueOrDefault(),
                Received = m.Received.GetValueOrDefault(),
                Used = m.Used.GetValueOrDefault(),
                Expired = m.Expired.GetValueOrDefault(),
                Damage = m.Damaged.GetValueOrDefault(),
                Loss = m.Loss.GetValueOrDefault(),
                Item = m.Sstock.Item,
                Ending = m.Openingbalance.GetValueOrDefault() + m.Received.GetValueOrDefault() - m.Used.GetValueOrDefault()
                - m.Expired.GetValueOrDefault() - m.Damaged.GetValueOrDefault() - m.Loss.GetValueOrDefault(),
                FacilityID=m.Nmr.FacilityId,
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
            });

            if (!data.Any())
            {
                return NotFound();
            }
            if (req.Year != 0)
            {
                data = data.Where(m => m.Year == req.Year);
            }
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];

            sheet.ImportData(data, 1, 1, true);

            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";


            pivotSheet["A2"].Text = "IPD-SAM stock and food item";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;
            options.ErrorString="NA";

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Item"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField Openingbalance = pivotTable.Fields["Balance"];
                IPivotField Received = pivotTable.Fields["Received"];
                IPivotField Used = pivotTable.Fields["Used"];
                IPivotField Expired = pivotTable.Fields["Expired"];
                IPivotField Damage = pivotTable.Fields["Damage"];
                IPivotField Loss = pivotTable.Fields["Loss"];
                IPivotField Ending = pivotTable.Fields["Ending"];


                pivotTable.DataFields.Add(Openingbalance, "Opening balance", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Received, "Received ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Used, "Used ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Expired, "Expired ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Damage, "Damage ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Loss, "Loss ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Ending, "Ending ", PivotSubtotalTypes.Sum);

            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;

            }
         
            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;
            pivotSheet.Activate();

            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "IPDSAMStock" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }


        public IActionResult pivotSam([Bind("ProvCode,Year,option")]CreateReq req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<TblOtp> get = _context.TblOtp;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblOtp.Where(m => m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }
            var data = get.AsNoTracking().Select(m => new samPivot()
            {
                Begin = m.Totalbegin.GetValueOrDefault(),
                Odema = m.Odema.GetValueOrDefault(),
                Z3score = m.Z3score.GetValueOrDefault(),
                Muac115 = m.Muac115.GetValueOrDefault(),
                totalnewAdmission = m.Odema.GetValueOrDefault() + m.Z3score.GetValueOrDefault()
                + m.Muac115.GetValueOrDefault(),
                Fromscotp = m.Fromscotp.GetValueOrDefault(),
                Fromsfp = m.Fromsfp.GetValueOrDefault(),
                Defaultreturn = m.Defaultreturn.GetValueOrDefault(),
                TotalReferIn = m.Fromscotp.GetValueOrDefault() + m.Fromsfp.GetValueOrDefault() + m.Defaultreturn.GetValueOrDefault(),
                totalAdmission = m.Odema.GetValueOrDefault() + m.Z3score.GetValueOrDefault()
                + m.Muac115.GetValueOrDefault() + m.Fromscotp.GetValueOrDefault() + m.Fromsfp.GetValueOrDefault()
                + m.Defaultreturn.GetValueOrDefault(),
                Cured = m.Cured.GetValueOrDefault(),
                Death = m.Death.GetValueOrDefault(),
                Default = m.Default.GetValueOrDefault(),
                RefOut = m.RefOut.GetValueOrDefault(),
                NonCured = m.NonCured.GetValueOrDefault(),
                TotalExists = m.Cured.GetValueOrDefault() + m.Death.GetValueOrDefault() + m.Default.GetValueOrDefault() + m.RefOut.GetValueOrDefault()
                + m.NonCured.GetValueOrDefault(),
                TotalExistsNoRefOut=m.Cured.GetValueOrDefault() + m.Death.GetValueOrDefault() + m.Default.GetValueOrDefault() + m.NonCured.GetValueOrDefault(),
                TMale = m.TMale.GetValueOrDefault(),
                TFemale = m.TFemale.GetValueOrDefault(),
                TotalAtEndMonth = m.Totalbegin.GetValueOrDefault() + m.Odema.GetValueOrDefault() + m.Z3score.GetValueOrDefault()
                + m.Muac115.GetValueOrDefault() + m.Fromscotp.GetValueOrDefault() + m.Fromsfp.GetValueOrDefault()
                + m.Defaultreturn.GetValueOrDefault() - (m.Cured.GetValueOrDefault() + m.Death.GetValueOrDefault() + m.Default.GetValueOrDefault() + m.RefOut.GetValueOrDefault()
                + m.NonCured.GetValueOrDefault()),
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityID=m.Nmr.FacilityId,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                Age = m.Otp.AgeGroup,

            });

            if (!data.Any())
            {
                return BadRequest();
            }
            data =data.Where(m => (m.totalAdmission + m.Begin)>0);
            if (req.Year != 0)
            {
                data = data.Where(m => m.Year == req.Year);
            }
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;


            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];

            sheet.ImportData(data, 1, 1, true);

            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";


            pivotSheet["A2"].Text = "OPD-SAM report";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Age"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField totalbegin = pivotTable.Fields["Begin"];
                IPivotField odema = pivotTable.Fields["Odema"];
                IPivotField Zscore23 = pivotTable.Fields["Z3score"];
                IPivotField MUAC115 = pivotTable.Fields["Muac115"];
                IPivotField TotalNewAdmissions = pivotTable.Fields["totalnewAdmission"];
                IPivotField fromscotp = pivotTable.Fields["Fromscotp"];
                IPivotField fromsfp = pivotTable.Fields["Fromsfp"];
                IPivotField defaultreturn = pivotTable.Fields["Defaultreturn"];
                IPivotField TotalReferIn = pivotTable.Fields["TotalReferIn"];
                IPivotField TotalAdmissions = pivotTable.Fields["totalAdmission"];
                IPivotField Cured = pivotTable.Fields["Cured"];
                IPivotField Death = pivotTable.Fields["Death"];
                IPivotField Default = pivotTable.Fields["Default"];
                IPivotField RefOut = pivotTable.Fields["RefOut"];
                IPivotField NonCured = pivotTable.Fields["NonCured"];
                IPivotField TotalExists = pivotTable.Fields["TotalExists"]; 
                IPivotField TotalExitsNoRefOut = pivotTable.Fields["TotalExitsNoRefOut"]; 
                IPivotField TotalAtEndMonth = pivotTable.Fields["TotalAtEndMonth"];
                IPivotField tmale = pivotTable.Fields["TMale"];
                IPivotField tFemale = pivotTable.Fields["TFemale"];

                pivotTable.DataFields.Add(totalbegin, "Total at beginning", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(odema, "Odema", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Zscore23, "W/H <-3 Z Score", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MUAC115, "MUAC <115 mm", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalNewAdmissions, "Total New Admissions", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(fromscotp, "From IP/OTP", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(fromsfp, "From SFP", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(defaultreturn, " Return Default", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalReferIn, "Total Refer in ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalAdmissions, "Total Admissions ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Cured, "Cured ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Death, "Deaths", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Default, "Defaults", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(RefOut, "Refer Out ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(NonCured, "Non-Cured", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalExists, "Total Exists", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalAtEndMonth, "Total End Of Month", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(tmale, "Male ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(tFemale, "Female ", PivotSubtotalTypes.Sum);

                IPivotField pCurrate = pivotTable.CalculatedFields.Add("Curedd", "Cured/TotalExistsNoRefOut");
                pCurrate.Name = " %cured";
                pCurrate.NumberFormat = "#.0%";

                IPivotField pDeath = pivotTable.CalculatedFields.Add("Deaths ", "Death/TotalExistsNoRefOut");
                pDeath.Name = " %death";
                pDeath.NumberFormat = "#.0%";

                IPivotField pDefaulters = pivotTable.CalculatedFields.Add("Defaults ", "Default/TotalExistsNoRefOut");
                pDefaulters.Name = " %default";
                pDefaulters.NumberFormat = "#.0%";


                IPivotField pNonCured = pivotTable.CalculatedFields.Add("NonCured ", "NonCured/TotalExistsNoRefOut");
                pNonCured.Name = " %NonCured";
                pNonCured.NumberFormat = "#.0%";

                IPivotField pMale = pivotTable.CalculatedFields.Add("TMale ", "tmale/(tmale+tfemale)");
                pMale.Name = "%male";
                pMale.NumberFormat = "#.0%";

                IPivotField pFemale = pivotTable.CalculatedFields.Add("TFemale ", "tFemale/(tfemale+tmale)");
                pFemale.Name = "%female";
                pFemale.NumberFormat = "#.0%";
            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;
            }
            options.ErrorString="NA";

            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;
            pivotSheet.Activate();

            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "OPDSAM" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);
        }

        public IActionResult samStockOut([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<TblStockOtp> get = _context.TblStockOtp;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblStockOtp.Where(m => m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }

            var data = get.AsNoTracking().Select(m => new samStockPivot()
            {
                Balance = m.Openingbalance.GetValueOrDefault(),
                Received = m.Received.GetValueOrDefault(),
                Used = m.Used.GetValueOrDefault(),
                Expired = m.Expired.GetValueOrDefault(),
                Damage = m.Damaged.GetValueOrDefault(),
                Loss = m.Loss.GetValueOrDefault(),
                Item = m.Sstockotp.Item,
                Ending = m.Openingbalance.GetValueOrDefault() + m.Received.GetValueOrDefault() -m.Used.GetValueOrDefault()
                - m.Expired.GetValueOrDefault() - m.Damaged.GetValueOrDefault() - m.Loss.GetValueOrDefault(),
                FacilityID=m.Nmr.FacilityId,
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
            });

            if (!data.Any())
            {
                return NotFound();
            }
            if (req.Year != 0)
            {
                data = data.Where(m => m.Year == req.Year);
            }
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];

            sheet.ImportData(data, 1, 1, true);

            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";


            pivotSheet["A2"].Text = "OPD-SAM Stock and Item Report";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Item"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField Openingbalance = pivotTable.Fields["Balance"];
                IPivotField Received = pivotTable.Fields["Received"];
                IPivotField Used = pivotTable.Fields["Used"];
                IPivotField Expired = pivotTable.Fields["Expired"];
                IPivotField Damage = pivotTable.Fields["Damage"];
                IPivotField Loss = pivotTable.Fields["Loss"];
                IPivotField Ending = pivotTable.Fields["Ending"];


                pivotTable.DataFields.Add(Openingbalance, "Opening balance", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Received, "Received ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Used, "Used ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Expired, "Expired ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Damage, "Damage ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Loss, "Loss ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Ending, "Ending ", PivotSubtotalTypes.Sum);

            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;

            }
            options.ErrorString="NA";

            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;
            pivotSheet.Activate();

            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "OPDSAMStock" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }


        public IActionResult pivotsamOut([Bind("ProvCode,Year,option")]CreateReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IQueryable<TblOtptfu> get = _context.TblOtptfu;
            if (!req.ProvCode.Equals("0"))
            {
                get = _context.TblOtptfu.Where(m => m.Nmr.Facility.DistNavigation.ProvCode.Equals(req.ProvCode));
            }

            var data = get.AsNoTracking().Select(m => new samPivot()
            {
                Begin = m.Totalbegin.GetValueOrDefault(),
                Odema = m.Odema.GetValueOrDefault(),
                Z3score = m.Z3score.GetValueOrDefault(),
                Muac115 = m.Muac115.GetValueOrDefault(),
                totalnewAdmission = m.Odema.GetValueOrDefault() + m.Z3score.GetValueOrDefault()
                + m.Muac115.GetValueOrDefault(),
                Fromscotp = m.Fromscotp.GetValueOrDefault(),
                Fromsfp = m.Fromsfp.GetValueOrDefault(),
                Defaultreturn = m.Defaultreturn.GetValueOrDefault(),
                TotalReferIn = m.Fromscotp.GetValueOrDefault() + m.Fromsfp.GetValueOrDefault() + m.Defaultreturn.GetValueOrDefault(),
                totalAdmission = m.Odema.GetValueOrDefault() + m.Z3score.GetValueOrDefault()
                + m.Muac115.GetValueOrDefault() + m.Fromscotp.GetValueOrDefault() + m.Fromsfp.GetValueOrDefault()
                + m.Defaultreturn.GetValueOrDefault(),
                Cured = m.Cured.GetValueOrDefault(),
                Death = m.Death.GetValueOrDefault(),
                Default = m.Default.GetValueOrDefault(),
                RefOut = m.RefOut.GetValueOrDefault(),
                NonCured = m.NonCured.GetValueOrDefault(),
                TotalExists = m.Cured.GetValueOrDefault() + m.Death.GetValueOrDefault() + m.Default.GetValueOrDefault() + m.RefOut.GetValueOrDefault()
                + m.NonCured.GetValueOrDefault(),
                TotalExistsNoRefOut = m.Cured.GetValueOrDefault() + m.Death.GetValueOrDefault() + m.Default.GetValueOrDefault() + m.NonCured.GetValueOrDefault(),
                TMale = m.TMale.GetValueOrDefault(),
                TFemale = m.TFemale.GetValueOrDefault(),
                TotalAtEndMonth = m.Totalbegin.GetValueOrDefault() + m.Odema.GetValueOrDefault() + m.Z3score.GetValueOrDefault()
                + m.Muac115.GetValueOrDefault() + m.Fromscotp.GetValueOrDefault() + m.Fromsfp.GetValueOrDefault()
                + m.Defaultreturn.GetValueOrDefault() - (m.Cured.GetValueOrDefault() + m.Death.GetValueOrDefault() + m.Default.GetValueOrDefault() + m.RefOut.GetValueOrDefault()
                + m.NonCured.GetValueOrDefault()),
                FacilityID=m.Nmr.FacilityId,
                FacilityName = m.Nmr.Facility.FacilityName,
                FacilityType = m.Nmr.Facility.FacilityTypeNavigation.FacType,
                Year = m.Nmr.Year,
                Month = m.Nmr.Month,
                mYear = m.Nmr.Month < 10 ? (m.Nmr.Year + 621) : (m.Nmr.Year + 622),
                mMonth = m.Nmr.Month < 10 ? (m.Nmr.Month + 3) : (m.Nmr.Month - 9),
                District = m.Nmr.Facility.DistNavigation.DistName,
                Implementer = m.Nmr.Implementer,
                Province = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                Age = m.Otptfu.AgeGroup,

            });

            if (!data.Any())
            {
                return BadRequest();
            }
            if (req.Year != 0)
            {
                data = data.Where(m => m.Year == req.Year);
            }
            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;


            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];


            sheet.ImportData(data, 1, 1, true);

            sheet.Name = "Data";
            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet.Name = "PivotTable";


            pivotSheet["A2"].Text = "IPD-SAM Report";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet[sheet.Range.AddressLocal]);
            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            IPivotTableOptions options = pivotTable.Options;
            options.ShowFieldList = false;

            pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            if (req.option == 1)
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;
            }
            else
            {
                pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            }
            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Age"].Axis = PivotAxisTypes.Row;

            if (req.option == 1)
            {
                IPivotField totalbegin = pivotTable.Fields["Begin"];
                IPivotField odema = pivotTable.Fields["Odema"];
                IPivotField Zscore23 = pivotTable.Fields["Z3score"];
                IPivotField MUAC115 = pivotTable.Fields["Muac115"];
                IPivotField TotalNewAdmissions = pivotTable.Fields["totalnewAdmission"];
                IPivotField fromscotp = pivotTable.Fields["Fromscotp"];
                IPivotField fromsfp = pivotTable.Fields["Fromsfp"];
                IPivotField defaultreturn = pivotTable.Fields["Defaultreturn"];
                IPivotField TotalReferIn = pivotTable.Fields["TotalReferIn"];
                IPivotField TotalAdmissions = pivotTable.Fields["totalAdmission"];
                IPivotField Cured = pivotTable.Fields["Cured"];
                IPivotField Death = pivotTable.Fields["Death"];
                IPivotField Default = pivotTable.Fields["Default"];
                IPivotField RefOut = pivotTable.Fields["RefOut"];
                IPivotField NonCured = pivotTable.Fields["NonCured"];
                IPivotField TotalExists = pivotTable.Fields["TotalExists"];
                IPivotField TotalExistsNoRefOut = pivotTable.Fields["TotalExistsNoRefOut"];
                IPivotField TotalAtEndMonth = pivotTable.Fields["TotalAtEndMonth"];
                IPivotField tmale = pivotTable.Fields["TMale"];
                IPivotField tFemale = pivotTable.Fields["TFemale"];

                pivotTable.DataFields.Add(totalbegin, "Total at beginning", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(odema, "Odema", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Zscore23, "W/H <-3 Z Score", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(MUAC115, "MUAC <115 mm", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalNewAdmissions, "Total New Admissions", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(fromscotp, "From IP/OTP", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(fromsfp, "From SFP", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(defaultreturn, " Return Default", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalReferIn, "Total Refer in ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalAdmissions, "Total Admissions ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Cured, "Cured ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Death, "Deaths", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(Default, "Defaults", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(RefOut, "Refer Out ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(NonCured, "Non-Cured", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalExists, "Total Exists", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(TotalAtEndMonth, "Total End Of Month", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(tmale, "Male ", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(tFemale, "Female ", PivotSubtotalTypes.Sum);

                IPivotField pCurrate = pivotTable.CalculatedFields.Add("Curedd", "Cured/TotalExistsNoRefOut");
                pCurrate.Name = " %cured";
                pCurrate.NumberFormat = "#.0%";

                IPivotField pDeath = pivotTable.CalculatedFields.Add("Deaths ", "Death/TotalExistsNoRefOut");
                pDeath.Name = " %death";
                pDeath.NumberFormat = "#.0%";

                IPivotField pDefaulters = pivotTable.CalculatedFields.Add("Defaults ", "Default/TotalExistsNoRefOut");
                pDefaulters.Name = " %default";
                pDefaulters.NumberFormat = "#.0%";

                IPivotField pNonCured = pivotTable.CalculatedFields.Add("NonCureds", "NonCured/TotalExistsNoRefOut");
                pNonCured.Name = " %NonCured";
                pNonCured.NumberFormat = "#.0%";

                IPivotField pMale = pivotTable.CalculatedFields.Add("TMale ", "tmale/(tmale+tfemale)");
                pMale.Name = " %male";
                pMale.NumberFormat = "#.0%";

                IPivotField pFemale = pivotTable.CalculatedFields.Add("TFemale ", "tFemale/(tfemale+tmale)");
                pFemale.Name = " %female";
                pFemale.NumberFormat = "#.0%";
            }
            else
            {
                IPivotField svalue = pivotTable.Fields[req.CollumnName];
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum);
                pivotTable.DataFields.Add(svalue, "", PivotSubtotalTypes.Sum).ShowDataAs = PivotFieldDataFormat.PercentageOfTotal;
            }

            options.ErrorString="NA";

            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark20;
            pivotSheet.Activate();

            string ContentType = "Application/msexcel";
            string filename = "NMR_" + "IPDSAM" + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);
        }
    }
}

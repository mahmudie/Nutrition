using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DataSystem.Models;
using DataSystem.Models.ViewModels.Export;
using DataSystem.Models.ViewModels.PivotTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;

namespace DataSystem.Controllers
{
    [Authorize]
    public class SubmissionController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;
        public SubmissionController(WebNutContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            _context.Database.SetCommandTimeout(5000);

            List<ImpFilter> ImpFilter = _context.ImpFilter.OrderBy(m => m.ImpCode).ToList();
            ImpFilter.Insert(0, new ImpFilter { ImpCode = "0", Implementer = "All" });
            ViewData["ImpList"] = new SelectList(ImpFilter, "ImpCode", "Implementer");
            return View();
        }
        public IActionResult indexm()
        {
            List<ImpFilter> ImpFilter = _context.ImpFilter.OrderBy(m => m.ImpCode).ToList();
            ImpFilter.Insert(0, new ImpFilter { ImpCode = "0", Implementer = "All" });
            ViewData["ImpList"] = new SelectList(ImpFilter, "ImpCode", "Implementer");
            return View();
        }

        public JsonResult provinces(string Implementer)
        {
            List<ProvinceFilter> ProvinceFilter = new List<ProvinceFilter>();
            ProvinceFilter = (from dist in _context.ProvinceFilter where dist.Implementer == Implementer select dist).AsNoTracking().ToList();
            ProvinceFilter.Insert(0, new ProvinceFilter { ProvCode = "0", ProvName = "All" });
            return Json(new SelectList(ProvinceFilter, "ProvCode", "ProvName").ToList());
        }
        public IActionResult pivotsum([Bind("Implementer,Province")]IndicatorsFilter req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int Province;
            string Implementer = null;
            string ProvCode = "0";

            Province = req.Province;
            Implementer = req.Implementer;

            if (Province > 0 & Province < 10)
            {
                ProvCode = "0" + Province;
            }

            var data = _context.reportsubmission.ToList();

            if (!Province.Equals(0) & Implementer.Equals("0"))
            {
                data = data.Where(m => m.ProvCode.Equals(Province)).ToList();
            }
            else if (Province.Equals(0) & !Implementer.Equals("0"))
            {
                data = data.Where(m => m.Implementer.Equals(Implementer)).ToList();
            }
            else if (!Province.Equals(0) & !Implementer.Equals("0"))
            {
                data = data.Where(m => m.Implementer.Equals(Implementer) & m.ProvCode.Equals(ProvCode)).ToList();
            }
            else
            {
                data = data.ToList();
            }

            var dataquery = data.Select(m => new
            {
                NMRID=m.NMRID,
                FacilityID=m.FacilityID,
                FacilityName =m.FacilityName,
                District=m.District,
                ProvCode=m.ProvCode,
                Province=m.Province,
                FacilityType=m.TypeAbbrv,
                Implementer=m.Implementer,
                Year=m.Year,
                Month=m.Month,
                NMR_submission =(m.IPDSAM_submission+ m.OPDSAM_submission+m.OPDMAM_submission+ m.MNS_submission+ m.OPDMAM_stock_submission + m.IPDSAM_stock_submission+ m.OPDSAM_stock_submission)>0 ?1:0,
                UserName =m.UserName

            }).ToList();

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];

            if (!data.Any())
            {
                return NotFound();
            }

            sheet.ImportData(dataquery, 1, 1, true);

            sheet.Name = "Data";

            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet["A2"].Text = "Nutrition Monthly Reports submission and completeness status";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            pivotSheet.Name = "Pivot";

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
           
            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["FacilityID"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["FacilityName"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["FacilityType"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["District"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["District"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;

            IPivotField NMR_submission = pivotTable.Fields["NMR_submission"];

            pivotTable.DataFields.Add(NMR_submission, "Submission", PivotSubtotalTypes.Sum);

            IPivotTableOptions option = pivotTable.Options;
            pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;
            option.ColumnHeaderCaption = "Section";
            option.RowHeaderCaption = "Monthly Report";

            //option.ErrorString = "X";
            pivotTable.ColumnGrand = false;
            pivotTable.RowGrand = false;


            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
            pivotSheet.Activate();


            string ContentType = "Application/msexcel";
            string filename = "SubmissionStatus" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

    public IActionResult pivot([Bind("Implementer,Province")]IndicatorsFilter req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int Province;
            string Implementer = null;
            string ProvCode = "0";

            Province = req.Province;
            Implementer = req.Implementer;

            if (Province > 0 & Province < 10)
            {
                ProvCode = "0" + Province;
            }
            _context.Database.SetCommandTimeout(5000);

            var data = _context.reportsubmission.ToList();

            if (!Province.Equals(0) & Implementer.Equals("0"))
            {
                data = data.Where(m => m.ProvCode.Equals(Province)).ToList();
            }
            else if (Province.Equals(0) & !Implementer.Equals("0"))
            {
                data = data.Where(m => m.Implementer.Equals(Implementer)).ToList();
            }
            else if (!Province.Equals(0) & !Implementer.Equals("0"))
            {
                data = data.Where(m => m.Implementer.Equals(Implementer) & m.ProvCode.Equals(ProvCode)).ToList();
            }
            else
            {
                data = data.ToList();
            }

            var dataquery = data.Select(m => new
            {
                NMRID=m.NMRID,
                FacilityID=m.FacilityID,
                FacilityName =m.FacilityName,
                District=m.District,
                ProvCode=m.ProvCode,
                Province=m.Province,
                FacilityType=m.TypeAbbrv,
                Implementer=m.Implementer,
                Year=m.Year,
                Month=m.Month,
                IPDSAM_submission=m.IPDSAM_submission,
                OPDSAM_submission=m.OPDSAM_submission,
                OPDMAM_submission=m.OPDMAM_submission,
                MNS_submission=m.MNS_submission,
                OPDMAM_stock_submission=m.OPDMAM_stock_submission,
                IPDSAM_stock_submission=m.IPDSAM_stock_submission,
                OPDSAM_stock_submission=m.OPDSAM_stock_submission,
                NMR_submission =(m.IPDSAM_submission+ m.OPDSAM_submission+m.OPDMAM_submission+ m.MNS_submission+ m.OPDMAM_stock_submission + m.IPDSAM_stock_submission+ m.OPDSAM_stock_submission)>0 ?1:0,
                UserName =m.UserName

            }).ToList();

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;

            IWorkbook workbook;
            workbook = application.Workbooks.Create(2);
            IWorksheet sheet = workbook.Worksheets[0];

            if (!data.Any())
            {
                return NotFound();
            }

            sheet.ImportData(dataquery, 1, 1, true);

            sheet.Name = "Data";

            IWorksheet pivotSheet = workbook.Worksheets[1];

            pivotSheet["A2"].Text = "Nutrition Monthly Reports submission and completeness status";
            pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
            pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
            pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
            pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
            pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

            pivotSheet.Name = "Pivot";

            IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

            IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

            pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Page;
            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Page;

            pivotTable.Fields["NMRID"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["NMRID"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["FacilityID"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["FacilityID"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["FacilityName"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["FacilityType"].Subtotals = PivotSubtotalTypes.None;
            pivotTable.Fields["District"].Axis = PivotAxisTypes.Row;
            pivotTable.Fields["District"].Subtotals = PivotSubtotalTypes.None;

            IPivotField pageField = pivotTable.Fields["Year"];
            IPivotField pageField2 = pivotTable.Fields["Month"];

            pageField.PivotFilters.Add(PivotFilterType.ValueEqual, pageField, "1396",null);
            pageField2.PivotFilters.Add(PivotFilterType.ValueEqual, pageField2, "12",null);

            IPivotField IPDSAM_submission = pivotTable.Fields["IPDSAM_submission"];
            IPivotField OPDSAM_submission = pivotTable.Fields["OPDSAM_submission"];
            IPivotField OPDMAM_submission = pivotTable.Fields["OPDMAM_submission"];
            IPivotField MNS_submission = pivotTable.Fields["MNS_submission"];
            IPivotField OPDMAM_stock_submission = pivotTable.Fields["OPDMAM_stock_submission"];
            IPivotField IPDSAM_stock_submission = pivotTable.Fields["IPDSAM_stock_submission"];
            IPivotField OPDSAM_stock_submission = pivotTable.Fields["OPDSAM_stock_submission"];
            IPivotField NMR_submission = pivotTable.Fields["NMR_submission"];


            pivotTable.DataFields.Add(NMR_submission, "Submission", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(IPDSAM_submission, "IPD SAM", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(OPDSAM_submission, "OPD SAM", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(OPDMAM_submission, "OPD MAM", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(MNS_submission, "GM", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(IPDSAM_stock_submission, "IPDSAM Stock", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(OPDSAM_stock_submission, "OPDSAM Stock", PivotSubtotalTypes.Sum);
            pivotTable.DataFields.Add(OPDMAM_stock_submission, "OPDMAM Stock", PivotSubtotalTypes.Sum);
            pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;

            IPivotTableOptions option = pivotTable.Options;

            option.ColumnHeaderCaption = "Section";
            option.RowHeaderCaption = "Monthly Report";

            option.ErrorString = "?";
            pivotTable.ColumnGrand = false;
            pivotTable.RowGrand = false;
            pivotSheet.Range["B10:C10"].ColumnWidth=60;
            pivotSheet.SetColumnWidth(1,60);

            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
            pivotSheet.Activate();


            string ContentType = "Application/msexcel";
            string filename = "SubmissionStatus" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            workbook.Close();
            excelEngine.Dispose();
            return File(ms, ContentType, filename);

        }

    }
}
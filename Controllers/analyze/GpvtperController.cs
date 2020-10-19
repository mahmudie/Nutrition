using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.ViewModels.PivotTable;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;

namespace DataSystem.Controllers.analyze
{
    public class GpvtperController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public GpvtperController(WebNutContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        #region Action Methods
        public ActionResult Index()
        {
            var data = _context.Procedureupdatestat.Select(m => m.Updatedate).FirstOrDefault();
            ViewBag.updatedate = data;
            return View();
        }

        public ActionResult ProvPivot()
        {
            List<Provinces> Povinces = _context.Provinces.OrderBy(m => m.ProvName).ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "All" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");

            return View();
        }

        public IActionResult PPivotView()
        {
            return View();
        }
        public ActionResult DistPivot()
        {
            List<Provinces> Povinces = _context.Provinces.OrderBy(m => m.ProvName).ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "All" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");

            return View();
        }

        public JsonResult districts(string ProvinceId)
        {
            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts.OrderBy(m => m.DistName) where dist.ProvCode == ProvinceId select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "0", DistName = "All" });
            return Json(new SelectList(Districts, "DistCode", "DistName"));
        }

        public ActionResult HFPivot()
        {
            List<Provinces> Povinces = _context.Provinces.OrderBy(m => m.ProvName).ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "All" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");

            return View();
        }

        public JsonResult facilities(string DistCode)
        {
            List<FacilityInfo> facilities = new List<FacilityInfo>();
            facilities = (from fac in _context.FacilityInfo orderby fac.FacilityId where fac.DistCode == DistCode select fac).ToList();
            facilities.Insert(0, new FacilityInfo { FacilityId = 0, FacilityName = "All" });
            return Json(new SelectList(facilities, "FacilityId", "FacilityFull"));
        }
        #endregion
        #region Pivot Table Methods
        #region Provincial Pivot Table
        public IActionResult PPivot([Bind("Province,Calendar,Period")]IndicatorsFilter req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int Province;
            int Calendar;
            int Period = 0;
            string ProvCode = "0";

            Province = req.Province;
            Calendar = req.Calendar;
            Period = req.Period;

            if (Province > 0 & Province < 10)
            {
                ProvCode = "0" + Province;
            }


            switch (Calendar)
            {
                case 1:
                    {
                        _context.Database.SetCommandTimeout(5000);

                        var data = _context.totalpivotcombinedpprovs.ToList();

                        if (!Province.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode)).ToList();
                        }
                        else
                        {
                            data = data.ToList();
                        }

                        //Syncfusion Excel
                        ExcelEngine excelEngine = new ExcelEngine();

                        IApplication application = excelEngine.Excel;

                        application.DefaultVersion = ExcelVersion.Excel2016;

                        IWorkbook workbook;
                        workbook = application.Workbooks.Create(2);
                        IWorksheet sheet = workbook.Worksheets[0];

                        if (!data.Any())
                        {
                            return NotFound();
                        }

                        try
                        {
                            sheet.Range["A1"].Text = "ProvinceId";
                            sheet.Range["B1"].Text = "Province";
                            sheet.Range["C1"].Text = "FacilityType";
                            sheet.Range["D1"].Text = "IndicatorName";
                            sheet.Range["E1"].Text = "Implementer";
                            sheet.Range["F1"].Text = "Module";
                            sheet.Range["G1"].Text = "Year";
                            sheet.Range["H1"].Text = "Quarter";
                            sheet.Range["I1"].Text = "Month";
                            sheet.Range["J1"].Text = "Num";
                            sheet.Range["K1"].Text = "Denom";


                            sheet.Range["A2"].Text = "%Reports.ProvinceId";
                            sheet.Range["B2"].Text = "%Reports.Province";
                            sheet.Range["C2"].Text = "%Reports.FacilityType";
                            sheet.Range["D2"].Text = "%Reports.IndicatorName";
                            sheet.Range["E2"].Text = "%Reports.Implementer";
                            sheet.Range["F2"].Text = "%Reports.Module";
                            sheet.Range["G2"].Text = "%Reports.Year";
                            sheet.Range["H2"].Text = "%Reports.Quarter";
                            sheet.Range["I2"].Text = "%Reports.Month";
                            sheet.Range["J2"].Text = "%Reports.Num";
                            sheet.Range["K2"].Text = "%Reports.Denom";

                            ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

                            marker.AddVariable("Reports", data);

                            marker.ApplyMarkers();
                            sheet.Name = "Data";
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        sheet.Name = "Data";

                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Provincial Level Indicators";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Module"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;

                        pivotTable.Fields["IndicatorName"].Axis = PivotAxisTypes.Row;

                        if (Period.Equals(1))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                        }
                        else if (Period.Equals(2))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Quarter"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        else if (Period.Equals(3))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }

                        IPivotField Num = pivotTable.Fields["Num"];
                        IPivotField Denom = pivotTable.Fields["Denom"];

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("Perc", "Num/Denom");
                        DefaultRate.Name = "Percentage";
                        DefaultRate.NumberFormat = "#.0%";

                        pivotTable.ShowDataFieldInRow = true;
                        IPivotTableOptions option = pivotTable.Options;
                        pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                        option.ErrorString = "";
                        pivotTable.ColumnGrand = false;
                        pivotTable.RowGrand = false;
                        if (Period.Equals(1))
                        {
                            option.ColumnHeaderCaption = "Year";
                        }
                        if (Period.Equals(2))
                        {
                            option.ColumnHeaderCaption = "Quarter";
                        }
                        if (Period.Equals(3))
                        {
                            option.ColumnHeaderCaption = "Month";
                        }

                        option.RowHeaderCaption = "Indicators";
                        option.ErrorString = "";

                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
                        pivotSheet.Activate();

                        string ContentType = "Application/msexcel";
                        string filename = "ProvincialIndicators" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);
                    }
                case 2:
                    {
                        _context.Database.SetCommandTimeout(5000);

                        var data = _context.totalpivotcombinedpprovm.ToList();

                        if (!Province.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode)).ToList();
                        }
                        else
                        {
                            data = data.ToList();
                        }

                        //Syncfusion Excel
                        ExcelEngine excelEngine = new ExcelEngine();

                        IApplication application = excelEngine.Excel;

                        application.DefaultVersion = ExcelVersion.Excel2016;

                        IWorkbook workbook;
                        workbook = application.Workbooks.Create(2);
                        IWorksheet sheet = workbook.Worksheets[0];

                        if (!data.Any())
                        {
                            return NotFound();
                        }

                        try
                        {
                            sheet.Range["A1"].Text = "ProvinceId";
                            sheet.Range["B1"].Text = "Province";
                            sheet.Range["C1"].Text = "FacilityType";
                            sheet.Range["D1"].Text = "IndicatorName";
                            sheet.Range["E1"].Text = "Implementer";
                            sheet.Range["F1"].Text = "Module";
                            sheet.Range["G1"].Text = "Year";
                            sheet.Range["H1"].Text = "Quarter";
                            sheet.Range["I1"].Text = "Month";
                            sheet.Range["J1"].Text = "Num";
                            sheet.Range["K1"].Text = "Denom";


                            sheet.Range["A2"].Text = "%Reports.ProvinceId";
                            sheet.Range["B2"].Text = "%Reports.Province";
                            sheet.Range["C2"].Text = "%Reports.FacilityType";
                            sheet.Range["D2"].Text = "%Reports.IndicatorName";
                            sheet.Range["E2"].Text = "%Reports.Implementer";
                            sheet.Range["F2"].Text = "%Reports.Module";
                            sheet.Range["G2"].Text = "%Reports.Year";
                            sheet.Range["H2"].Text = "%Reports.Quarter";
                            sheet.Range["I2"].Text = "%Reports.Month";
                            sheet.Range["J2"].Text = "%Reports.Num";
                            sheet.Range["K2"].Text = "%Reports.Denom";

                            ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

                            marker.AddVariable("Reports", data);

                            marker.ApplyMarkers();
                            sheet.Name = "Data";
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        sheet.Name = "Data";

                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Provincial Level Indicators";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Module"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;

                        pivotTable.Fields["IndicatorName"].Axis = PivotAxisTypes.Row;

                        if (Period.Equals(1))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                        }
                        else if (Period.Equals(2))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Quarter"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        else if (Period.Equals(3))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }

                        IPivotField Num = pivotTable.Fields["Num"];
                        IPivotField Denom = pivotTable.Fields["Denom"];

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("Perc", "Num/Denom");
                        DefaultRate.Name = "Percentage";
                        DefaultRate.NumberFormat = "#.0%";


                        pivotTable.ShowDataFieldInRow = true;
                        IPivotTableOptions option = pivotTable.Options;
                        pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                        option.ErrorString = "";
                        pivotTable.ColumnGrand = false;
                        pivotTable.RowGrand = false;
                        if (Period.Equals(1))
                        {
                            option.ColumnHeaderCaption = "Year";
                        }
                        if (Period.Equals(2))
                        {
                            option.ColumnHeaderCaption = "Quarter";
                        }
                        if (Period.Equals(3))
                        {
                            option.ColumnHeaderCaption = "Month";
                        }

                        option.RowHeaderCaption = "Indicators";

                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
                        pivotSheet.Activate();

                        string ContentType = "Application/msexcel";
                        string filename = "ProvincialIndicators" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
            }

            return View();
        }
        #endregion
        #region District Pivot Table
        public IActionResult DPivot([Bind("Province,DistCode,Calendar,Period")]IndicatorsFilter req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int Province;
            int District;
            int Calendar;
            int Period = 0;
            string ProvCode = "0";
            string DistCode = "0";

            Province = req.Province;
            District = req.DistCode;
            Calendar = req.Calendar;
            Period = req.Period;

            if (Province > 0 & Province < 10)
            {
                ProvCode = "0" + Province;
                DistCode = "0" + District;
            }
            else if (Province > 10)
            {
                ProvCode = Province.ToString();
                DistCode = District.ToString();
            }

            switch (Calendar)
            {
                case 1:
                    {
                        _context.Database.SetCommandTimeout(5000);

                        var data = _context.totalpivotcombinedpdists.ToList();

                        if (!Province.Equals(0) & !District.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode) & m.DistrictId.Equals(DistCode)).ToList();
                        }
                        else if (!Province.Equals(0) & District.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode)).ToList();
                        }
                        else
                        {
                            data = data.ToList();
                        }

                        //Syncfusion Excel
                        ExcelEngine excelEngine = new ExcelEngine();

                        IApplication application = excelEngine.Excel;

                        application.DefaultVersion = ExcelVersion.Excel2016;

                        IWorkbook workbook;
                        workbook = application.Workbooks.Create(2);
                        IWorksheet sheet = workbook.Worksheets[0];

                        if (!data.Any())
                        {
                            return NotFound();
                        }

                        try
                        {
                            sheet.Range["A1"].Text = "ProvinceId";
                            sheet.Range["B1"].Text = "Province";
                            sheet.Range["C1"].Text = "DistrictId";
                            sheet.Range["D1"].Text = "District";
                            sheet.Range["E1"].Text = "FacilityType";
                            sheet.Range["F1"].Text = "IndicatorName";
                            sheet.Range["G1"].Text = "Implementer";
                            sheet.Range["H1"].Text = "Module";
                            sheet.Range["I1"].Text = "Year";
                            sheet.Range["J1"].Text = "Quarter";
                            sheet.Range["K1"].Text = "Month";
                            sheet.Range["L1"].Text = "Num";
                            sheet.Range["M1"].Text = "Denom";


                            sheet.Range["A2"].Text = "%Reports.ProvinceId";
                            sheet.Range["B2"].Text = "%Reports.Province";
                            sheet.Range["C2"].Text = "%Reports.DistrictId";
                            sheet.Range["D2"].Text = "%Reports.District";
                            sheet.Range["E2"].Text = "%Reports.FacilityType";
                            sheet.Range["F2"].Text = "%Reports.IndicatorName";
                            sheet.Range["G2"].Text = "%Reports.Implementer";
                            sheet.Range["H2"].Text = "%Reports.Module";
                            sheet.Range["I2"].Text = "%Reports.Year";
                            sheet.Range["J2"].Text = "%Reports.Quarter";
                            sheet.Range["K2"].Text = "%Reports.Month";
                            sheet.Range["L2"].Text = "%Reports.Num";
                            sheet.Range["M2"].Text = "%Reports.Denom";

                            ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

                            marker.AddVariable("Reports", data);

                            marker.ApplyMarkers();
                            sheet.Name = "Data";
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        sheet.Name = "Data";

                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition District Level Indicators";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Module"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;

                        pivotTable.Fields["IndicatorName"].Axis = PivotAxisTypes.Row;

                        if (Period.Equals(1))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                        }
                        else if (Period.Equals(2))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Quarter"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        else if (Period.Equals(3))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }

                        IPivotField Num = pivotTable.Fields["Num"];
                        IPivotField Denom = pivotTable.Fields["Denom"];

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("Perc", "Num/Denom");
                        DefaultRate.Name = "Percentage";
                        DefaultRate.NumberFormat = "#.0%";

                        pivotTable.ShowDataFieldInRow = true;
                        IPivotTableOptions option = pivotTable.Options;
                        pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                        option.ErrorString = "";
                        pivotTable.ColumnGrand = false;
                        pivotTable.RowGrand = false;
                        if (Period.Equals(1))
                        {
                            option.ColumnHeaderCaption = "Year";
                        }
                        if (Period.Equals(2))
                        {
                            option.ColumnHeaderCaption = "Quarter";
                        }
                        if (Period.Equals(3))
                        {
                            option.ColumnHeaderCaption = "Month";
                        }

                        option.RowHeaderCaption = "Indicators";

                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
                        pivotSheet.Activate();

                        string ContentType = "Application/msexcel";
                        string filename = "DistrictIndicators" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
                case 2:
                    {
                        _context.Database.SetCommandTimeout(5000);

                        var data = _context.totalpivotcombinedpdistm.ToList();

                        if (!Province.Equals(0) & !District.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode) & m.DistrictId.Equals(DistCode)).ToList();
                        }
                        else if (!Province.Equals(0) & District.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode)).ToList();
                        }
                        else
                        {
                            data = data.ToList();
                        }

                        //Syncfusion Excel
                        ExcelEngine excelEngine = new ExcelEngine();

                        IApplication application = excelEngine.Excel;

                        application.DefaultVersion = ExcelVersion.Excel2016;

                        IWorkbook workbook;
                        workbook = application.Workbooks.Create(2);
                        IWorksheet sheet = workbook.Worksheets[0];

                        if (!data.Any())
                        {
                            return NotFound();
                        }

                        try
                        {
                            sheet.Range["A1"].Text = "ProvinceId";
                            sheet.Range["B1"].Text = "Province";
                            sheet.Range["C1"].Text = "DistrictId";
                            sheet.Range["D1"].Text = "District";
                            sheet.Range["E1"].Text = "FacilityType";
                            sheet.Range["F1"].Text = "IndicatorName";
                            sheet.Range["G1"].Text = "Implementer";
                            sheet.Range["H1"].Text = "Module";
                            sheet.Range["I1"].Text = "Year";
                            sheet.Range["J1"].Text = "Quarter";
                            sheet.Range["K1"].Text = "Month";
                            sheet.Range["L1"].Text = "Num";
                            sheet.Range["M1"].Text = "Denom";


                            sheet.Range["A2"].Text = "%Reports.ProvinceId";
                            sheet.Range["B2"].Text = "%Reports.Province";
                            sheet.Range["C2"].Text = "%Reports.DistrictId";
                            sheet.Range["D2"].Text = "%Reports.District";
                            sheet.Range["E2"].Text = "%Reports.FacilityType";
                            sheet.Range["F2"].Text = "%Reports.IndicatorName";
                            sheet.Range["G2"].Text = "%Reports.Implementer";
                            sheet.Range["H2"].Text = "%Reports.Module";
                            sheet.Range["I2"].Text = "%Reports.Year";
                            sheet.Range["J2"].Text = "%Reports.Quarter";
                            sheet.Range["K2"].Text = "%Reports.Month";
                            sheet.Range["L2"].Text = "%Reports.Num";
                            sheet.Range["M2"].Text = "%Reports.Denom";

                            ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

                            marker.AddVariable("Reports", data);

                            marker.ApplyMarkers();
                            sheet.Name = "Data";
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        sheet.Name = "Data";

                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition District Level Indicators";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Module"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;

                        pivotTable.Fields["IndicatorName"].Axis = PivotAxisTypes.Row;

                        if (Period.Equals(1))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                        }
                        else if (Period.Equals(2))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Quarter"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        else if (Period.Equals(3))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }

                        IPivotField Num = pivotTable.Fields["Num"];
                        IPivotField Denom = pivotTable.Fields["Denom"];

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("Perc", "Num/Denom");
                        DefaultRate.Name = "Percentage";
                        DefaultRate.NumberFormat = "#.0%";

                        pivotTable.ShowDataFieldInRow = true;
                        IPivotTableOptions option = pivotTable.Options;
                        pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                        option.ErrorString = "";

                        pivotTable.ColumnGrand = false;
                        pivotTable.RowGrand = false;
                        if (Period.Equals(1))
                        {
                            option.ColumnHeaderCaption = "Year";
                        }
                        if (Period.Equals(2))
                        {
                            option.ColumnHeaderCaption = "Quarter";
                        }
                        if (Period.Equals(3))
                        {
                            option.ColumnHeaderCaption = "Month";
                        }

                        option.RowHeaderCaption = "Indicators";

                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
                        pivotSheet.Activate();

                        string ContentType = "Application/msexcel";
                        string filename = "DistrictIndicators" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
            }

            return View();
        }
        #endregion
        #region Health Facility Pivot Table
        public IActionResult HPivot([Bind("Province,DistCode,Facility,Calendar,Period")]IndicatorsFilter req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int Province;
            int District;
            int Calendar;
            int FacilityId;
            int Period = 0;
            string ProvCode = "0";
            string DistCode = "0";

            Province = req.Province;
            District = req.DistCode;
            Calendar = req.Calendar;
            FacilityId = req.Facility;
            Period = req.Period;

            if (Province > 0 & Province < 10)
            {
                ProvCode = "0" + Province;
                DistCode = "0" + District;
            }
            else if (Province > 10)
            {
                ProvCode = Province.ToString();
                DistCode = District.ToString();
            }

            switch (Calendar)
            {
                case 1:
                    {
                        _context.Database.SetCommandTimeout(5000);

                        var data = _context.totalpivotcombinedphfs.ToList();

                        if (!Province.Equals(0) & !District.Equals(0) & !FacilityId.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode) & m.DistrictId.Equals(DistCode) & m.FacilityId.Equals(FacilityId)).ToList();
                        }
                        else if (!Province.Equals(0) & !District.Equals(0) & FacilityId.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode) & m.DistrictId.Equals(DistCode)).ToList();
                        }
                        else if (!Province.Equals(0) & District.Equals(0) & FacilityId.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode)).ToList();
                        }
                        else
                        {
                            data = data.ToList();
                        }

                        //Syncfusion Excel
                        ExcelEngine excelEngine = new ExcelEngine();

                        IApplication application = excelEngine.Excel;

                        application.DefaultVersion = ExcelVersion.Excel2016;

                        IWorkbook workbook;
                        workbook = application.Workbooks.Create(2);
                        IWorksheet sheet = workbook.Worksheets[0];

                        if (!data.Any())
                        {
                            return NotFound();
                        }

                        try
                        {
                            sheet.Range["A1"].Text = "ProvinceId";
                            sheet.Range["B1"].Text = "Province";
                            sheet.Range["C1"].Text = "DistrictId";
                            sheet.Range["D1"].Text = "District";
                            sheet.Range["E1"].Text = "FacilityId";
                            sheet.Range["F1"].Text = "FacilityName";
                            sheet.Range["G1"].Text = "FacilityType";
                            sheet.Range["H1"].Text = "IndicatorName";
                            sheet.Range["I1"].Text = "Implementer";
                            sheet.Range["J1"].Text = "Module";
                            sheet.Range["K1"].Text = "Year";
                            sheet.Range["L1"].Text = "Quarter";
                            sheet.Range["M1"].Text = "Month";
                            sheet.Range["N1"].Text = "Num";
                            sheet.Range["O1"].Text = "Denom";


                            sheet.Range["A2"].Text = "%Reports.ProvinceId";
                            sheet.Range["B2"].Text = "%Reports.Province";
                            sheet.Range["C2"].Text = "%Reports.DistrictId";
                            sheet.Range["D2"].Text = "%Reports.District";
                            sheet.Range["E2"].Text = "%Reports.FacilityId";
                            sheet.Range["F2"].Text = "%Reports.FacilityName";
                            sheet.Range["G2"].Text = "%Reports.FacilityType";
                            sheet.Range["H2"].Text = "%Reports.IndicatorName";
                            sheet.Range["I2"].Text = "%Reports.Implementer";
                            sheet.Range["J2"].Text = "%Reports.Module";
                            sheet.Range["K2"].Text = "%Reports.Year";
                            sheet.Range["L2"].Text = "%Reports.Quarter";
                            sheet.Range["M2"].Text = "%Reports.Month";
                            sheet.Range["N2"].Text = "%Reports.Num";
                            sheet.Range["O2"].Text = "%Reports.Denom";

                            ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

                            marker.AddVariable("Reports", data);

                            marker.ApplyMarkers();
                            sheet.Name = "Data";
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        sheet.Name = "Data";

                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Health Facility Level Indicators";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Module"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityId"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;

                        pivotTable.Fields["IndicatorName"].Axis = PivotAxisTypes.Row;

                        if (Period.Equals(1))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                        }
                        else if (Period.Equals(2))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Quarter"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        else if (Period.Equals(3))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        IPivotField Num = pivotTable.Fields["Num"];
                        IPivotField Denom = pivotTable.Fields["Denom"];

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("Perc", "Num/Denom");
                        DefaultRate.Name = "Percentage";
                        DefaultRate.NumberFormat = "#.0%";

                        pivotTable.ShowDataFieldInRow = true;
                        IPivotTableOptions option = pivotTable.Options;
                        pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                        option.ErrorString = "";
                        pivotTable.ColumnGrand = false;
                        pivotTable.RowGrand = false;
                        if (Period.Equals(1))
                        {
                            option.ColumnHeaderCaption = "Year";
                        }
                        if (Period.Equals(2))
                        {
                            option.ColumnHeaderCaption = "Quarter";
                        }
                        if (Period.Equals(3))
                        {
                            option.ColumnHeaderCaption = "Month";
                        }

                        option.RowHeaderCaption = "Indicators";

                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
                        pivotSheet.Activate();

                        string ContentType = "Application/msexcel";
                        string filename = "HFIndicators_numeric" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
                case 2:
                    {
                        _context.Database.SetCommandTimeout(5000);
                        var data = _context.totalpivotcombinedphfm.ToList();

                        if (!Province.Equals(0) & !District.Equals(0) & !FacilityId.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode) & m.DistrictId.Equals(DistCode) & m.FacilityId.Equals(FacilityId)).ToList();
                        }
                        else if (!Province.Equals(0) & !District.Equals(0) & FacilityId.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode) & m.DistrictId.Equals(DistCode)).ToList();
                        }
                        else if (!Province.Equals(0) & District.Equals(0) & FacilityId.Equals(0))
                        {
                            data = data.Where(m => m.ProvinceId.Equals(ProvCode)).ToList();
                        }
                        else
                        {
                            data = data.ToList();
                        }

                        //Syncfusion Excel
                        ExcelEngine excelEngine = new ExcelEngine();

                        IApplication application = excelEngine.Excel;

                        application.DefaultVersion = ExcelVersion.Excel2016;

                        IWorkbook workbook;
                        workbook = application.Workbooks.Create(2);
                        IWorksheet sheet = workbook.Worksheets[0];

                        if (!data.Any())
                        {
                            return NotFound();
                        }

                        try
                        {
                            sheet.Range["A1"].Text = "ProvinceId";
                            sheet.Range["B1"].Text = "Province";
                            sheet.Range["C1"].Text = "DistrictId";
                            sheet.Range["D1"].Text = "District";
                            sheet.Range["E1"].Text = "FacilityId";
                            sheet.Range["F1"].Text = "FacilityName";
                            sheet.Range["G1"].Text = "FacilityType";
                            sheet.Range["H1"].Text = "IndicatorName";
                            sheet.Range["I1"].Text = "Implementer";
                            sheet.Range["J1"].Text = "Module";
                            sheet.Range["K1"].Text = "Year";
                            sheet.Range["L1"].Text = "Quarter";
                            sheet.Range["M1"].Text = "Month";
                            sheet.Range["N1"].Text = "Num";
                            sheet.Range["O1"].Text = "Denom";


                            sheet.Range["A2"].Text = "%Reports.ProvinceId";
                            sheet.Range["B2"].Text = "%Reports.Province";
                            sheet.Range["C2"].Text = "%Reports.DistrictId";
                            sheet.Range["D2"].Text = "%Reports.District";
                            sheet.Range["E2"].Text = "%Reports.FacilityId";
                            sheet.Range["F2"].Text = "%Reports.FacilityName";
                            sheet.Range["G2"].Text = "%Reports.FacilityType";
                            sheet.Range["H2"].Text = "%Reports.IndicatorName";
                            sheet.Range["I2"].Text = "%Reports.Implementer";
                            sheet.Range["J2"].Text = "%Reports.Module";
                            sheet.Range["K2"].Text = "%Reports.Year";
                            sheet.Range["L2"].Text = "%Reports.Quarter";
                            sheet.Range["M2"].Text = "%Reports.Month";
                            sheet.Range["N2"].Text = "%Reports.Num";
                            sheet.Range["O2"].Text = "%Reports.Denom";

                            ITemplateMarkersProcessor marker = workbook.CreateTemplateMarkersProcessor();

                            marker.AddVariable("Reports", data);

                            marker.ApplyMarkers();
                            sheet.Name = "Data";
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        sheet.Name = "Data";

                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Health Facility Level Indicators";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Module"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityId"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityType"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["Implementer"].Axis = PivotAxisTypes.Page;

                        pivotTable.Fields["IndicatorName"].Axis = PivotAxisTypes.Row;

                        if (Period.Equals(1))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                        }
                        else if (Period.Equals(2))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Quarter"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        else if (Period.Equals(3))
                        {
                            pivotTable.Fields["Year"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Month"].Axis = PivotAxisTypes.Column;
                            pivotTable.Fields["Year"].Subtotals = PivotSubtotalTypes.None;
                        }
                        IPivotField Num = pivotTable.Fields["Num"];
                        IPivotField Denom = pivotTable.Fields["Denom"];

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("Perc", "Num/Denom");
                        DefaultRate.Name = "Percentage";
                        DefaultRate.NumberFormat = "#.0%";


                        pivotTable.ShowDataFieldInRow = true;
                        IPivotTableOptions option = pivotTable.Options;
                        pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                        option.ErrorString = "";
                        pivotTable.ColumnGrand = false;
                        pivotTable.RowGrand = false;
                        if (Period.Equals(1))
                        {
                            option.ColumnHeaderCaption = "Year";
                        }
                        if (Period.Equals(2))
                        {
                            option.ColumnHeaderCaption = "Quarter";
                        }
                        if (Period.Equals(3))
                        {
                            option.ColumnHeaderCaption = "Month";
                        }

                        option.RowHeaderCaption = "Indicators";

                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark14;
                        pivotSheet.Activate();

                        string ContentType = "Application/msexcel";
                        string filename = "HFIndicators_numeric" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
            }

            return View();
        }
        #endregion
        #endregion

        #region
        public IActionResult UpdateProcedure()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("exec dbo.AddProvincialKpi");
                _context.Database.ExecuteSqlCommand("exec dbo.AddTotalIndicatorsn");
                _context.Database.ExecuteSqlCommand("exec dbo.AddTotalIndicatorsp");

            }
            catch (Exception)
            {

                throw;
            }

            _context.Database.ExecuteSqlCommand("exec dbo.Updatetables");

            return RedirectToAction("Index");
        }
        #endregion
    }
}
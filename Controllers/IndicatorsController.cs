using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using DataSystem.Models;
using DataSystem.Models.ViewModels.PivotTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;

namespace DataSystem.Controllers
{
    [Authorize]
    public class IndicatorsController : Controller
    {
        private readonly WebNutContext _context;
        private readonly IMapper _mapper;

        public IndicatorsController(WebNutContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

      public IActionResult Index()
        {
            List<Provinces> Povinces = _context.Provinces.ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "All" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");
            return View();
        }

        public IActionResult sindicators()
        {
            List<Provinces> Povinces = _context.Provinces.ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "All" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");
            return View();
        }
        public IActionResult mindicators()
        {
            List<Provinces> Povinces = _context.Provinces.ToList();
            Povinces.Insert(0, new Provinces { ProvCode = "0", ProvName = "All" });
            ViewData["ProvList"] = new SelectList(Povinces, "ProvCode", "ProvName");
            return View();
        }

        public JsonResult districts(string ProvinceId)
        {
            List<Districts> Districts = new List<Districts>();
            Districts = (from dist in _context.Districts where dist.ProvCode == ProvinceId  select dist).ToList();
            Districts.Insert(0, new Districts { DistCode = "0", DistName = "All" });
            return Json(new SelectList(Districts, "DistCode", "DistName"));
        }

        public IActionResult IndPivot([Bind("Province,DistCode,Viewtype,Period")]IndicatorsFilter req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int Province;
            int District;
            string Viewtype = null;
            int Period=0;
            string ProvCode="0";
            string DistCode="0";


            Province =req.Province;
            District =req.DistCode;
            Viewtype =req.Viewtype;
            Period=req.Period;

            if(Province>0 & Province<10)
            {
                ProvCode = "0" + Province;
            }
            if(District>0)
            {
                DistCode = "0" + District;
            }

            _context.Database.SetCommandTimeout(9000);
            var data =_context.statReports.ToList();

            if (!Province.Equals(0) & District.Equals(0))
            {
                data = data.Where(m => m.ProvCode.Equals(ProvCode)).ToList();
            }

            else if (Province.Equals(0) & !District.Equals(0))
            {
                data = data.Where(m => m.DistCode.Equals(DistCode)).ToList();
            }
            else if (!Province.Equals(0) & !District.Equals(0))
            {
                data =data.Where(m =>m.ProvCode.Equals(ProvCode) & m.DistCode.Equals(DistCode)).ToList();
            }
            else
            {
                data = data.ToList();
            }


        switch (Viewtype)
        {
            case "1":
                    {
                        _context.Database.SetCommandTimeout(5000);
                        var province = data.GroupBy(x => new
                        {
                            x.Province,
                            x.Year,
                            x.Quarter,
                            x.Month
                        })
                        .Select(g => new
                        {
                            Province = g.Key.Province,
                            Year = g.Key.Year,
                            Quarter = g.Key.Quarter,
                            Month = g.Key.Month,
                            samAdmittedTotal = g.Sum(m => m.samAdmittedTotal),
                            samAdmittedMale = g.Sum(m => m.samAdmittedMale),
                            samAdmittedFemale = g.Sum(m => m.samAdmittedFemale),
                            samAdmitIpd = g.Sum(m => m.samAdmitIpd),
                            samAdmitIpdMale = g.Sum(m => m.samAdmitIpdMale),
                            samAdmitIpdFemale = g.Sum(m => m.samAdmitIpdFemale),
                            samAdmitOpd = g.Sum(m => m.samAdmitOpd),
                            samAdmitOpdMale = g.Sum(m => m.samAdmitOpdMale),
                            samAdmitOpdFemale = g.Sum(m => m.samAdmitOpdFemale),
                            mamU5 = g.Sum(m => m.mamU5),
                            mamPlw = g.Sum(m => m.mamPlw),
                            withSamServices = g.Sum(m => m.withSamServices),
                            withSamIpd = g.Sum(m => m.withSamIpd),
                            withSamOpd = g.Sum(m => m.withSamOpd),
                            mamU5Services = g.Sum(m => m.mamU5Services),
                            mamPlwServices = g.Sum(m => m.mamPlwServices),
                            samCured = g.Sum(m => m.samCured),
                            samDeaths = g.Sum(m => m.samDeaths),
                            samDefaults = g.Sum(m => m.samDefaults),
                            ipdCured = g.Sum(m => m.ipdCured),
                            ipdDeaths = g.Sum(m => m.ipdDeaths),
                            ipdDefaults = g.Sum(m => m.ipdDefaults),
                            ipdExists = g.Sum(m => m.ipdExists),
                            samExists = g.Sum(m => m.samExists)
                        });
                        //Export data and create pivot table


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

                        sheet.ImportData(province, 1, 1, true);

                        sheet.Name = "Data";

                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Monthly Services Indicators achievments and progress";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
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

                        IPivotField samAdmittedTotal = pivotTable.Fields["samAdmittedTotal"];
                        IPivotField samAdmittedMale = pivotTable.Fields["samAdmittedMale"];
                        IPivotField samAdmittedFemale = pivotTable.Fields["samAdmittedFemale"];
                        IPivotField samAdmitIpd = pivotTable.Fields["samAdmitIpd"];
                        IPivotField samAdmitIpdMale = pivotTable.Fields["samAdmitIpdMale"];
                        IPivotField samAdmitIpdFemale = pivotTable.Fields["samAdmitIpdFemale"];
                        IPivotField samAdmitOpd = pivotTable.Fields["samAdmitOpd"];
                        IPivotField samAdmitOpdMale = pivotTable.Fields["samAdmitOpdMale"];
                        IPivotField samAdmitOpdFemale = pivotTable.Fields["samAdmitOpdFemale"];
                        IPivotField mamU5 = pivotTable.Fields["mamU5"];
                        IPivotField mamPlw = pivotTable.Fields["mamPlw"];
                        IPivotField withSamServices = pivotTable.Fields["withSamServices"];
                        IPivotField withSamIpd = pivotTable.Fields["withSamIpd"];
                        IPivotField withSamOpd = pivotTable.Fields["withSamOpd"];
                        IPivotField mamU5Services = pivotTable.Fields["mamU5Services"];
                        IPivotField mamPlwServices = pivotTable.Fields["mamPlwServices"];
                        IPivotField samCured = pivotTable.Fields["samCured"];
                        IPivotField samDeaths = pivotTable.Fields["samDeaths"];
                        IPivotField samDefaults = pivotTable.Fields["samDefaults"];
                        IPivotField ipdCured = pivotTable.Fields["ipdCured"];
                        IPivotField ipdDeaths = pivotTable.Fields["ipdDeaths"];
                        IPivotField ipdDefaults = pivotTable.Fields["ipdDefaults"];

                        pivotTable.DataFields.Add(samAdmittedTotal, "# of U5 cases of SAM admitted (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedMale, "# of U5 cases of SAM admitted male (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedFemale, "# of U5 cases of SAM admitted female (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpd, "# of U5 cases of SAM admitted (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdMale, "# of U5 cases of SAM admitted male (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdFemale, "# of U5 cases of SAM admitted female (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpd, "# of U5 cases of SAM admitted (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdMale, "# of U5 cases of SAM admitted male (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdFemale, "# of U5 cases of SAM admitted female (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5, "# of U5 cases of MAM admitted", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlw, "# of PLW cases of MAM admitted ", PivotSubtotalTypes.Sum);

                        IPivotField Currate = pivotTable.CalculatedFields.Add("samCured1", "samCured/samExists");
                        Currate.Name = "SAM Cure rate (All)";
                        Currate.NumberFormat = "#.0%";

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("samDefaults1", "samDefaults/samExists");
                        DefaultRate.Name = "SAM defualt rate (All)";
                        DefaultRate.NumberFormat = "#.0%";


                        IPivotField DeathRate = pivotTable.CalculatedFields.Add("samDeaths1", "samDeaths/samExists");
                        DeathRate.Name = "SAM death rate (All)";
                        DeathRate.NumberFormat = "#.0%";

                        IPivotField IPDCurrate = pivotTable.CalculatedFields.Add("ipdCured1", "ipdCured/ipdExists");
                        IPDCurrate.Name = "SAM Cure rate (IPD)";
                        IPDCurrate.NumberFormat = "#.0%";

                        IPivotField IPDDefaultRate = pivotTable.CalculatedFields.Add("ipdDefaults1", "ipdDefaults/ipdExists");
                        IPDDefaultRate.Name = "SAM defualt rate (IPD)";
                        IPDDefaultRate.NumberFormat = "#.0%";

                        IPivotField IPDDeathRate = pivotTable.CalculatedFields.Add("ipdDeaths1", "ipdDeaths/ipdExists");
                        IPDDeathRate.Name = "SAM death rate (IPD)";
                        IPDDeathRate.NumberFormat = "#.0%";

                        pivotTable.DataFields.Add(withSamServices, "# of nutrition sites with SAM services (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(withSamIpd, "# of nutrition sites with SAM services (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5Services, "# of nutrition sites with MAM services (U5)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlwServices, "# of nutrition sites with MAM services (PLW)", PivotSubtotalTypes.Sum);
                        pivotTable.ShowDataFieldInRow = true;
                        IPivotTableOptions option = pivotTable.Options;
                        pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;
                        
                        //option.ErrorString = "?";
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
                string filename = "Province report" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;
                workbook.Close();
                excelEngine.Dispose();
                return File(ms, ContentType, filename);

            }
            break;

                case "2":
                    {
                        _context.Database.SetCommandTimeout(5000);
                        var district = data.GroupBy(x => new
                        {
                            x.Province,
                            x.District,
                            x.Year,
                            x.Quarter,
                            x.Month
                        })
                        .Select(g => new
                        {
                            Province = g.Key.Province,
                            District=g.Key.District,
                            Year = g.Key.Year,
                            Quarter = g.Key.Quarter,
                            Month = g.Key.Month,
                            samAdmittedTotal = g.Sum(m => m.samAdmittedTotal),
                            samAdmittedMale = g.Sum(m => m.samAdmittedMale),
                            samAdmittedFemale = g.Sum(m => m.samAdmittedFemale),
                            samAdmitIpd = g.Sum(m => m.samAdmitIpd),
                            samAdmitIpdMale = g.Sum(m => m.samAdmitIpdMale),
                            samAdmitIpdFemale = g.Sum(m => m.samAdmitIpdFemale),
                            samAdmitOpd = g.Sum(m => m.samAdmitOpd),
                            samAdmitOpdMale = g.Sum(m => m.samAdmitOpdMale),
                            samAdmitOpdFemale = g.Sum(m => m.samAdmitOpdFemale),
                            mamU5 = g.Sum(m => m.mamU5),
                            mamPlw = g.Sum(m => m.mamPlw),
                            withSamServices = g.Sum(m => m.withSamServices),
                            withSamIpd = g.Sum(m => m.withSamIpd),
                            withSamOpd = g.Sum(m => m.withSamOpd),
                            mamU5Services = g.Sum(m => m.mamU5Services),
                            mamPlwServices = g.Sum(m => m.mamPlwServices),
                            samCured = g.Sum(m => m.samCured),
                            samDeaths = g.Sum(m => m.samDeaths),
                            samDefaults = g.Sum(m => m.samDefaults),
                            ipdCured = g.Sum(m => m.ipdCured),
                            ipdDeaths = g.Sum(m => m.ipdDeaths),
                            ipdDefaults = g.Sum(m => m.ipdDefaults),
                            ipdExists = g.Sum(m => m.ipdExists),
                            samExists = g.Sum(m => m.samExists)
                        });
                        //Export data and create pivot table

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

                        sheet.ImportData(district, 1, 1, true);

                        sheet.Name = "Data";
                        IWorksheet pivotSheet = workbook.Worksheets[1];
                        pivotSheet["A2"].Text = "Nutrition Monthly Services Indicators achievments and progress";
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
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Row;
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

                        IPivotField samAdmittedTotal = pivotTable.Fields["samAdmittedTotal"];
                        IPivotField samAdmittedMale = pivotTable.Fields["samAdmittedMale"];
                        IPivotField samAdmittedFemale = pivotTable.Fields["samAdmittedFemale"];
                        IPivotField samAdmitIpd = pivotTable.Fields["samAdmitIpd"];
                        IPivotField samAdmitIpdMale = pivotTable.Fields["samAdmitIpdMale"];
                        IPivotField samAdmitIpdFemale = pivotTable.Fields["samAdmitIpdFemale"];
                        IPivotField samAdmitOpd = pivotTable.Fields["samAdmitOpd"];
                        IPivotField samAdmitOpdMale = pivotTable.Fields["samAdmitOpdMale"];
                        IPivotField samAdmitOpdFemale = pivotTable.Fields["samAdmitOpdFemale"];
                        IPivotField mamU5 = pivotTable.Fields["mamU5"];
                        IPivotField mamPlw = pivotTable.Fields["mamPlw"];
                        IPivotField withSamServices = pivotTable.Fields["withSamServices"];
                        IPivotField withSamIpd = pivotTable.Fields["withSamIpd"];
                        IPivotField withSamOpd = pivotTable.Fields["withSamOpd"];
                        IPivotField mamU5Services = pivotTable.Fields["mamU5Services"];
                        IPivotField mamPlwServices = pivotTable.Fields["mamPlwServices"];
                        IPivotField samCured = pivotTable.Fields["samCured"];
                        IPivotField samDeaths = pivotTable.Fields["samDeaths"];
                        IPivotField samDefaults = pivotTable.Fields["samDefaults"];
                        IPivotField ipdCured = pivotTable.Fields["ipdCured"];
                        IPivotField ipdCureipdDeathsd = pivotTable.Fields["ipdDeaths"];
                        IPivotField ipdDefaults = pivotTable.Fields["ipdDefaults"];

                        pivotTable.DataFields.Add(samAdmittedTotal, "# of U5 cases of SAM admitted (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedMale, "# of U5 cases of SAM admitted male (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedFemale, "# of U5 cases of SAM admitted female (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpd, "# of U5 cases of SAM admitted (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdMale, "# of U5 cases of SAM admitted male (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdFemale, "# of U5 cases of SAM admitted female (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpd, "# of U5 cases of SAM admitted (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdMale, "# of U5 cases of SAM admitted male (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdFemale, "# of U5 cases of SAM admitted female (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5, "# of U5 cases of MAM admitted", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlw, "# of PLW cases of MAM admitted ", PivotSubtotalTypes.Sum);

                        IPivotField Currate = pivotTable.CalculatedFields.Add("samCured2", "samCured/samExists");
                        Currate.Name = "SAM Cure rate (All)";
                        Currate.NumberFormat = "#.0%";

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("samDefaults2", "samDefaults/samExists");
                        DefaultRate.Name = "SAM defualt rate (All)";
                        DefaultRate.NumberFormat = "#.0%";


                        IPivotField DeathRate = pivotTable.CalculatedFields.Add("samDeaths2", "samDeaths/samExists");
                        DeathRate.Name = "SAM death rate (All)";
                        DeathRate.NumberFormat = "#.0%";

                        IPivotField IPDCurrate = pivotTable.CalculatedFields.Add("ipdCured2", "ipdCured/ipdExists");
                        IPDCurrate.Name = "SAM Cure rate (IPD)";
                        IPDCurrate.NumberFormat = "#.0%";

                        IPivotField IPDDefaultRate = pivotTable.CalculatedFields.Add("ipdDefaults2", "ipdDefaults/ipdExists");
                        IPDDefaultRate.Name = "SAM defualt rate (IPD)";
                        IPDDefaultRate.NumberFormat = "#.0%";

                        IPivotField IPDDeathRate = pivotTable.CalculatedFields.Add("ipdDeaths2", "ipdDeaths/ipdExists");
                        IPDDeathRate.Name = "SAM death rate (IPD)";
                        IPDDeathRate.NumberFormat = "#.0%";

                        pivotTable.DataFields.Add(withSamServices, "# of nutrition sites with SAM services (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(withSamIpd, "# of nutrition sites with SAM services (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5Services, "# of nutrition sites with MAM services (U5)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlwServices, "# of nutrition sites with MAM services (PLW)", PivotSubtotalTypes.Sum);
                        pivotTable.ShowDataFieldInRow = true;
                        pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;
                        IPivotTableOptions option = pivotTable.Options;
                        //option.ErrorString = "?";
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
                        string filename = "District report" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
                    break;

                case "3":
                    {
                        _context.Database.SetCommandTimeout(5000);
                        var faility = data.GroupBy(x => new
                        {
                            x.Province,
                            x.District,
                            x.FacilityID,
                            x.FacilityName,
                            x.Year,
                            x.Quarter,
                            x.Month
                        })
                        .Select(g => new
                        {
                            Province = g.Key.Province,
                            District = g.Key.District,
                            FacilityId=g.Key.FacilityID,
                            FacilityName =g.Key.FacilityName,
                            Year = g.Key.Year,
                            Quarter = g.Key.Quarter,
                            Month = g.Key.Month,
                            samAdmittedTotal = g.Sum(m => m.samAdmittedTotal),
                            samAdmittedMale = g.Sum(m => m.samAdmittedMale),
                            samAdmittedFemale = g.Sum(m => m.samAdmittedFemale),
                            samAdmitIpd = g.Sum(m => m.samAdmitIpd),
                            samAdmitIpdMale = g.Sum(m => m.samAdmitIpdMale),
                            samAdmitIpdFemale = g.Sum(m => m.samAdmitIpdFemale),
                            samAdmitOpd = g.Sum(m => m.samAdmitOpd),
                            samAdmitOpdMale = g.Sum(m => m.samAdmitOpdMale),
                            samAdmitOpdFemale = g.Sum(m => m.samAdmitOpdFemale),
                            mamU5 = g.Sum(m => m.mamU5),
                            mamPlw = g.Sum(m => m.mamPlw),
                            withSamServices = g.Sum(m => m.withSamServices),
                            withSamIpd = g.Sum(m => m.withSamIpd),
                            withSamOpd = g.Sum(m => m.withSamOpd),
                            mamU5Services = g.Sum(m => m.mamU5Services),
                            mamPlwServices = g.Sum(m => m.mamPlwServices),
                            samCured = g.Sum(m => m.samCured),
                            samDeaths = g.Sum(m => m.samDeaths),
                            samDefaults = g.Sum(m => m.samDefaults),
                            ipdCured = g.Sum(m => m.ipdCured),
                            ipdDeaths = g.Sum(m => m.ipdDeaths),
                            ipdDefaults = g.Sum(m => m.ipdDefaults),
                            ipdExists = g.Sum(m => m.ipdExists),
                            samExists = g.Sum(m => m.samExists)
                        });
                        //Export data and create pivot table

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

                        sheet.ImportData(faility, 1, 1, true);

                        sheet.Name = "Data";
                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Monthly Services Indicators achievments and progress";
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
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Row;
                        
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

                        IPivotField samAdmittedTotal = pivotTable.Fields["samAdmittedTotal"];
                        IPivotField samAdmittedMale = pivotTable.Fields["samAdmittedMale"];
                        IPivotField samAdmittedFemale = pivotTable.Fields["samAdmittedFemale"];
                        IPivotField samAdmitIpd = pivotTable.Fields["samAdmitIpd"];
                        IPivotField samAdmitIpdMale = pivotTable.Fields["samAdmitIpdMale"];
                        IPivotField samAdmitIpdFemale = pivotTable.Fields["samAdmitIpdFemale"];
                        IPivotField samAdmitOpd = pivotTable.Fields["samAdmitOpd"];
                        IPivotField samAdmitOpdMale = pivotTable.Fields["samAdmitOpdMale"];
                        IPivotField samAdmitOpdFemale = pivotTable.Fields["samAdmitOpdFemale"];
                        IPivotField mamU5 = pivotTable.Fields["mamU5"];
                        IPivotField mamPlw = pivotTable.Fields["mamPlw"];
                        IPivotField withSamServices = pivotTable.Fields["withSamServices"];
                        IPivotField withSamIpd = pivotTable.Fields["withSamIpd"];
                        IPivotField withSamOpd = pivotTable.Fields["withSamOpd"];
                        IPivotField mamU5Services = pivotTable.Fields["mamU5Services"];
                        IPivotField mamPlwServices = pivotTable.Fields["mamPlwServices"];
                        IPivotField samCured = pivotTable.Fields["samCured"];
                        IPivotField samDeaths = pivotTable.Fields["samDeaths"];
                        IPivotField samDefaults = pivotTable.Fields["samDefaults"];
                        IPivotField ipdCured = pivotTable.Fields["ipdCured"];
                        IPivotField ipdCureipdDeathsd = pivotTable.Fields["ipdDeaths"];
                        IPivotField ipdDefaults = pivotTable.Fields["ipdDefaults"];

                        pivotTable.DataFields.Add(samAdmittedTotal, "# of U5 cases of SAM admitted (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedMale, "# of U5 cases of SAM admitted male (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedFemale, "# of U5 cases of SAM admitted female (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpd, "# of U5 cases of SAM admitted (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdMale, "# of U5 cases of SAM admitted male (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdFemale, "# of U5 cases of SAM admitted female (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpd, "# of U5 cases of SAM admitted (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdMale, "# of U5 cases of SAM admitted male (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdFemale, "# of U5 cases of SAM admitted female (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5, "# of U5 cases of MAM admitted", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlw, "# of PLW cases of MAM admitted ", PivotSubtotalTypes.Sum);

                        IPivotField Currate = pivotTable.CalculatedFields.Add("samCured3", "samCured/samExists");
                        Currate.Name = "SAM Cure rate (All)";
                        Currate.NumberFormat = "#.0%";

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("samDefaults3", "samDefaults/samExists");
                        DefaultRate.Name = "SAM defualt rate (All)";
                        DefaultRate.NumberFormat = "#.0%";


                        IPivotField DeathRate = pivotTable.CalculatedFields.Add("samDeaths3", "samDeaths/samExists");
                        DeathRate.Name = "SAM death rate (All)";
                        DeathRate.NumberFormat = "#.0%";

                        IPivotField IPDCurrate = pivotTable.CalculatedFields.Add("ipdCured3", "ipdCured/ipdExists");
                        IPDCurrate.Name = "SAM Cure rate (IPD)";
                        IPDCurrate.NumberFormat = "#.0%";

                        IPivotField IPDDefaultRate = pivotTable.CalculatedFields.Add("ipdDefaults3", "ipdDefaults/ipdExists");
                        IPDDefaultRate.Name = "SAM defualt rate (IPD)";
                        IPDDefaultRate.NumberFormat = "#.0%";

                        IPivotField IPDDeathRate = pivotTable.CalculatedFields.Add("ipdDeaths3", "ipdDeaths/ipdExists");
                        IPDDeathRate.Name = "SAM death rate (IPD)";
                        IPDDeathRate.NumberFormat = "#.0%";

                        pivotTable.DataFields.Add(withSamServices, "# of nutrition sites with SAM services (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(withSamIpd, "# of nutrition sites with SAM services (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5Services, "# of nutrition sites with MAM services (U5)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlwServices, "# of nutrition sites with MAM services (PLW)", PivotSubtotalTypes.Sum);
                        pivotTable.ShowDataFieldInRow = true;
                        pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;
                        IPivotTableOptions option = pivotTable.Options;
                        //option.ErrorString = "X";
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
                        string filename = "Facility report" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
                    break;
            }

            return View();
        }

        public IActionResult IndPivotM([Bind("Province,DistCode,Viewtype,Period")]IndicatorsFilter req)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            int Province;
            int District;
            string Viewtype = null;
            int Period = 0;
            string ProvCode = "0";
            string DistCode = "0";


            Province = req.Province;
            District = req.DistCode;
            Viewtype = req.Viewtype;
            Period = req.Period;

            if (Province > 0 & Province < 10)
            {
                ProvCode = "0" + Province;
            }
            if (District > 0)
            {
                DistCode = "0" + District;
            }
            _context.Database.SetCommandTimeout(9000);

            var data = _context.statReports.ToList();

            if (!Province.Equals(0) & District.Equals(0))
            {
                data = data.Where(m => m.ProvCode.Equals(ProvCode)).ToList();
            }

            else if (Province.Equals(0) & !District.Equals(0))
            {
                data = data.Where(m => m.DistCode.Equals(DistCode)).ToList();
            }
            else if (!Province.Equals(0) & !District.Equals(0))
            {
                data = data.Where(m => m.ProvCode.Equals(ProvCode) & m.DistCode.Equals(DistCode)).ToList();
            }
            else
            {
                data = data.ToList();
            }


            switch (Viewtype)
            {
                case "1":
                    {
                        _context.Database.SetCommandTimeout(5000);
                        var province = data.GroupBy(x => new
                        {
                            x.Province,
                            x.mYear,
                            x.mQuarter,
                            x.mMonth
                        })
                        .Select(g => new
                        {
                            Province = g.Key.Province,
                            Year = g.Key.mYear,
                            Quarter = g.Key.mQuarter,
                            Month = g.Key.mMonth,
                            samAdmittedTotal = g.Sum(m => m.samAdmittedTotal),
                            samAdmittedMale = g.Sum(m => m.samAdmittedMale),
                            samAdmittedFemale = g.Sum(m => m.samAdmittedFemale),
                            samAdmitIpd = g.Sum(m => m.samAdmitIpd),
                            samAdmitIpdMale = g.Sum(m => m.samAdmitIpdMale),
                            samAdmitIpdFemale = g.Sum(m => m.samAdmitIpdFemale),
                            samAdmitOpd = g.Sum(m => m.samAdmitOpd),
                            samAdmitOpdMale = g.Sum(m => m.samAdmitOpdMale),
                            samAdmitOpdFemale = g.Sum(m => m.samAdmitOpdFemale),
                            mamU5 = g.Sum(m => m.mamU5),
                            mamPlw = g.Sum(m => m.mamPlw),
                            withSamServices = g.Sum(m => m.withSamServices),
                            withSamIpd = g.Sum(m => m.withSamIpd),
                            withSamOpd = g.Sum(m => m.withSamOpd),
                            mamU5Services = g.Sum(m => m.mamU5Services),
                            mamPlwServices = g.Sum(m => m.mamPlwServices),
                            samCured = g.Sum(m => m.samCured),
                            samDeaths = g.Sum(m => m.samDeaths),
                            samDefaults = g.Sum(m => m.samDefaults),
                            ipdCured = g.Sum(m => m.ipdCured),
                            ipdDeaths = g.Sum(m => m.ipdDeaths),
                            ipdDefaults = g.Sum(m => m.ipdDefaults),
                            ipdExists = g.Sum(m => m.ipdExists),
                            samExists = g.Sum(m => m.samExists)
                        });
                        //Export data and create pivot table


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

                        sheet.ImportData(province, 1, 1, true);

                        sheet.Name = "Data";




                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Monthly Services Indicators achievments and progress";
                        pivotSheet.Range["A2"].CellStyle.Font.Size = 14f;
                        pivotSheet.Range["A2"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].Text = "Date extracted: " + DateTime.Now.ToString(); ;
                        pivotSheet.Range["A3"].CellStyle.Font.Size = 10f;
                        pivotSheet.Range["A3"].CellStyle.Font.Bold = true;
                        pivotSheet.Range["A3"].CellStyle.Font.Italic = true;

                        pivotSheet.Name = "Pivot";

                        IPivotCache cash_data = workbook.PivotCaches.Add(sheet.UsedRange);

                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A5"], cash_data);

                        pivotTable.Fields["Province"].Axis = PivotAxisTypes.Row;
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

                        IPivotField samAdmittedTotal = pivotTable.Fields["samAdmittedTotal"];
                        IPivotField samAdmittedMale = pivotTable.Fields["samAdmittedMale"];
                        IPivotField samAdmittedFemale = pivotTable.Fields["samAdmittedFemale"];
                        IPivotField samAdmitIpd = pivotTable.Fields["samAdmitIpd"];
                        IPivotField samAdmitIpdMale = pivotTable.Fields["samAdmitIpdMale"];
                        IPivotField samAdmitIpdFemale = pivotTable.Fields["samAdmitIpdFemale"];
                        IPivotField samAdmitOpd = pivotTable.Fields["samAdmitOpd"];
                        IPivotField samAdmitOpdMale = pivotTable.Fields["samAdmitOpdMale"];
                        IPivotField samAdmitOpdFemale = pivotTable.Fields["samAdmitOpdFemale"];
                        IPivotField mamU5 = pivotTable.Fields["mamU5"];
                        IPivotField mamPlw = pivotTable.Fields["mamPlw"];
                        IPivotField withSamServices = pivotTable.Fields["withSamServices"];
                        IPivotField withSamIpd = pivotTable.Fields["withSamIpd"];
                        IPivotField withSamOpd = pivotTable.Fields["withSamOpd"];
                        IPivotField mamU5Services = pivotTable.Fields["mamU5Services"];
                        IPivotField mamPlwServices = pivotTable.Fields["mamPlwServices"];
                        IPivotField samCured = pivotTable.Fields["samCured"];
                        IPivotField samDeaths = pivotTable.Fields["samDeaths"];
                        IPivotField samDefaults = pivotTable.Fields["samDefaults"];
                        IPivotField ipdCured = pivotTable.Fields["ipdCured"];
                        IPivotField ipdCureipdDeathsd = pivotTable.Fields["ipdDeaths"];
                        IPivotField ipdDefaults = pivotTable.Fields["ipdDefaults"];

                        pivotTable.DataFields.Add(samAdmittedTotal, "# of U5 cases of SAM admitted (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedMale, "# of U5 cases of SAM admitted male (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedFemale, "# of U5 cases of SAM admitted female (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpd, "# of U5 cases of SAM admitted (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdMale, "# of U5 cases of SAM admitted male (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdFemale, "# of U5 cases of SAM admitted female (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpd, "# of U5 cases of SAM admitted (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdMale, "# of U5 cases of SAM admitted male (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdFemale, "# of U5 cases of SAM admitted female (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5, "# of U5 cases of MAM admitted", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlw, "# of PLW cases of MAM admitted ", PivotSubtotalTypes.Sum);

                        IPivotField Currate = pivotTable.CalculatedFields.Add("samCured1", "samCured/samExists");
                        Currate.Name = "SAM Cure rate (All)";
                        Currate.NumberFormat = "#.0%";

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("samDefaults1", "samDefaults/samExists");
                        DefaultRate.Name = "SAM defualt rate (All)";
                        DefaultRate.NumberFormat = "#.0%";


                        IPivotField DeathRate = pivotTable.CalculatedFields.Add("samDeaths1", "samDeaths/samExists");
                        DeathRate.Name = "SAM death rate (All)";
                        DeathRate.NumberFormat = "#.0%";

                        IPivotField IPDCurrate = pivotTable.CalculatedFields.Add("ipdCured1", "ipdCured/ipdExists");
                        IPDCurrate.Name = "SAM Cure rate (IPD)";
                        IPDCurrate.NumberFormat = "#.0%";

                        IPivotField IPDDefaultRate = pivotTable.CalculatedFields.Add("ipdDefaults1", "ipdDefaults/ipdExists");
                        IPDDefaultRate.Name = "SAM defualt rate (IPD)";
                        IPDDefaultRate.NumberFormat = "#.0%";

                        IPivotField IPDDeathRate = pivotTable.CalculatedFields.Add("ipdDeaths1", "ipdDeaths/ipdExists");
                        IPDDeathRate.Name = "SAM death rate (IPD)";
                        IPDDeathRate.NumberFormat = "#.0%";

                        pivotTable.DataFields.Add(withSamServices, "# of nutrition sites with SAM services (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(withSamIpd, "# of nutrition sites with SAM services (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5Services, "# of nutrition sites with MAM services (U5)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlwServices, "# of nutrition sites with MAM services (PLW)", PivotSubtotalTypes.Sum);
                        pivotTable.ShowDataFieldInRow = true;
                        pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;
                        IPivotTableOptions option = pivotTable.Options;
                        //option.ErrorString = "X";
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
                        string filename = "Province report" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
                    break;

                case "2":
                    {
                        _context.Database.SetCommandTimeout(5000);
                        var district = data.GroupBy(x => new
                        {
                            x.Province,
                            x.District,
                            x.mYear,
                            x.mQuarter,
                            x.mMonth
                        })
                        .Select(g => new
                        {
                            Province = g.Key.Province,
                            District = g.Key.District,
                            Year = g.Key.mYear,
                            Quarter = g.Key.mQuarter,
                            Month = g.Key.mMonth,
                            samAdmittedTotal = g.Sum(m => m.samAdmittedTotal),
                            samAdmittedMale = g.Sum(m => m.samAdmittedMale),
                            samAdmittedFemale = g.Sum(m => m.samAdmittedFemale),
                            samAdmitIpd = g.Sum(m => m.samAdmitIpd),
                            samAdmitIpdMale = g.Sum(m => m.samAdmitIpdMale),
                            samAdmitIpdFemale = g.Sum(m => m.samAdmitIpdFemale),
                            samAdmitOpd = g.Sum(m => m.samAdmitOpd),
                            samAdmitOpdMale = g.Sum(m => m.samAdmitOpdMale),
                            samAdmitOpdFemale = g.Sum(m => m.samAdmitOpdFemale),
                            mamU5 = g.Sum(m => m.mamU5),
                            mamPlw = g.Sum(m => m.mamPlw),
                            withSamServices = g.Sum(m => m.withSamServices),
                            withSamIpd = g.Sum(m => m.withSamIpd),
                            withSamOpd = g.Sum(m => m.withSamOpd),
                            mamU5Services = g.Sum(m => m.mamU5Services),
                            mamPlwServices = g.Sum(m => m.mamPlwServices),
                            samCured = g.Sum(m => m.samCured),
                            samDeaths = g.Sum(m => m.samDeaths),
                            samDefaults = g.Sum(m => m.samDefaults),
                            ipdCured = g.Sum(m => m.ipdCured),
                            ipdDeaths = g.Sum(m => m.ipdDeaths),
                            ipdDefaults = g.Sum(m => m.ipdDefaults),
                            ipdExists = g.Sum(m => m.ipdExists),
                            samExists = g.Sum(m => m.samExists)
                        });
                        //Export data and create pivot table

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

                        sheet.ImportData(district, 1, 1, true);

                        sheet.Name = "Data";
                        IWorksheet pivotSheet = workbook.Worksheets[1];
                        pivotSheet["A2"].Text = "Nutrition Monthly Services Indicators achievments and progress";
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
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Row;
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

                        IPivotField samAdmittedTotal = pivotTable.Fields["samAdmittedTotal"];
                        IPivotField samAdmittedMale = pivotTable.Fields["samAdmittedMale"];
                        IPivotField samAdmittedFemale = pivotTable.Fields["samAdmittedFemale"];
                        IPivotField samAdmitIpd = pivotTable.Fields["samAdmitIpd"];
                        IPivotField samAdmitIpdMale = pivotTable.Fields["samAdmitIpdMale"];
                        IPivotField samAdmitIpdFemale = pivotTable.Fields["samAdmitIpdFemale"];
                        IPivotField samAdmitOpd = pivotTable.Fields["samAdmitOpd"];
                        IPivotField samAdmitOpdMale = pivotTable.Fields["samAdmitOpdMale"];
                        IPivotField samAdmitOpdFemale = pivotTable.Fields["samAdmitOpdFemale"];
                        IPivotField mamU5 = pivotTable.Fields["mamU5"];
                        IPivotField mamPlw = pivotTable.Fields["mamPlw"];
                        IPivotField withSamServices = pivotTable.Fields["withSamServices"];
                        IPivotField withSamIpd = pivotTable.Fields["withSamIpd"];
                        IPivotField withSamOpd = pivotTable.Fields["withSamOpd"];
                        IPivotField mamU5Services = pivotTable.Fields["mamU5Services"];
                        IPivotField mamPlwServices = pivotTable.Fields["mamPlwServices"];
                        IPivotField samCured = pivotTable.Fields["samCured"];
                        IPivotField samDeaths = pivotTable.Fields["samDeaths"];
                        IPivotField samDefaults = pivotTable.Fields["samDefaults"];
                        IPivotField ipdCured = pivotTable.Fields["ipdCured"];
                        IPivotField ipdCureipdDeathsd = pivotTable.Fields["ipdDeaths"];
                        IPivotField ipdDefaults = pivotTable.Fields["ipdDefaults"];

                        pivotTable.DataFields.Add(samAdmittedTotal, "# of U5 cases of SAM admitted (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedMale, "# of U5 cases of SAM admitted male (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedFemale, "# of U5 cases of SAM admitted female (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpd, "# of U5 cases of SAM admitted (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdMale, "# of U5 cases of SAM admitted male (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdFemale, "# of U5 cases of SAM admitted female (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpd, "# of U5 cases of SAM admitted (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdMale, "# of U5 cases of SAM admitted male (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdFemale, "# of U5 cases of SAM admitted female (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5, "# of U5 cases of MAM admitted", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlw, "# of PLW cases of MAM admitted ", PivotSubtotalTypes.Sum);

                        IPivotField Currate = pivotTable.CalculatedFields.Add("samCured2", "samCured/samExists");
                        Currate.Name = "SAM Cure rate (All)";
                        Currate.NumberFormat = "#.0%";

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("samDefaults2", "samDefaults/samExists");
                        DefaultRate.Name = "SAM defualt rate (All)";
                        DefaultRate.NumberFormat = "#.0%";


                        IPivotField DeathRate = pivotTable.CalculatedFields.Add("samDeaths2", "samDeaths/samExists");
                        DeathRate.Name = "SAM death rate (All)";
                        DeathRate.NumberFormat = "#.0%";

                        IPivotField IPDCurrate = pivotTable.CalculatedFields.Add("ipdCured2", "ipdCured/ipdExists");
                        IPDCurrate.Name = "SAM Cure rate (IPD)";
                        IPDCurrate.NumberFormat = "#.0%";

                        IPivotField IPDDefaultRate = pivotTable.CalculatedFields.Add("ipdDefaults2", "ipdDefaults/ipdExists");
                        IPDDefaultRate.Name = "SAM defualt rate (IPD)";
                        IPDDefaultRate.NumberFormat = "#.0%";

                        IPivotField IPDDeathRate = pivotTable.CalculatedFields.Add("ipdDeaths2", "ipdDeaths/ipdExists");
                        IPDDeathRate.Name = "SAM death rate (IPD)";
                        IPDDeathRate.NumberFormat = "#.0%";

                        pivotTable.DataFields.Add(withSamServices, "# of nutrition sites with SAM services (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(withSamIpd, "# of nutrition sites with SAM services (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5Services, "# of nutrition sites with MAM services (U5)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlwServices, "# of nutrition sites with MAM services (PLW)", PivotSubtotalTypes.Sum);
                        pivotTable.ShowDataFieldInRow = true;
                        pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;
                        IPivotTableOptions option = pivotTable.Options;
                        //option.ErrorString = "X";
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
                        string filename = "District report" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
                    break;

                case "3":
                    {
                        _context.Database.SetCommandTimeout(5000);
                        var faility = data.GroupBy(x => new
                        {
                            x.Province,
                            x.District,
                            x.FacilityID,
                            x.FacilityName,
                            x.mYear,
                            x.mQuarter,
                            x.mMonth
                        })
                        .Select(g => new
                        {
                            Province = g.Key.Province,
                            District = g.Key.District,
                            FacilityId = g.Key.FacilityID,
                            FacilityName = g.Key.FacilityName,
                            Year = g.Key.mYear,
                            Quarter = g.Key.mQuarter,
                            Month = g.Key.mMonth,
                            samAdmittedTotal = g.Sum(m => m.samAdmittedTotal),
                            samAdmittedMale = g.Sum(m => m.samAdmittedMale),
                            samAdmittedFemale = g.Sum(m => m.samAdmittedFemale),
                            samAdmitIpd = g.Sum(m => m.samAdmitIpd),
                            samAdmitIpdMale = g.Sum(m => m.samAdmitIpdMale),
                            samAdmitIpdFemale = g.Sum(m => m.samAdmitIpdFemale),
                            samAdmitOpd = g.Sum(m => m.samAdmitOpd),
                            samAdmitOpdMale = g.Sum(m => m.samAdmitOpdMale),
                            samAdmitOpdFemale = g.Sum(m => m.samAdmitOpdFemale),
                            mamU5 = g.Sum(m => m.mamU5),
                            mamPlw = g.Sum(m => m.mamPlw),
                            withSamServices = g.Sum(m => m.withSamServices),
                            withSamIpd = g.Sum(m => m.withSamIpd),
                            withSamOpd = g.Sum(m => m.withSamOpd),
                            mamU5Services = g.Sum(m => m.mamU5Services),
                            mamPlwServices = g.Sum(m => m.mamPlwServices),
                            samCured = g.Sum(m => m.samCured),
                            samDeaths = g.Sum(m => m.samDeaths),
                            samDefaults = g.Sum(m => m.samDefaults),
                            ipdCured = g.Sum(m => m.ipdCured),
                            ipdDeaths = g.Sum(m => m.ipdDeaths),
                            ipdDefaults = g.Sum(m => m.ipdDefaults),
                            ipdExists = g.Sum(m => m.ipdExists),
                            samExists = g.Sum(m => m.samExists)
                        });
                        //Export data and create pivot table

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

                        sheet.ImportData(faility, 1, 1, true);

                        sheet.Name = "Data";
                        IWorksheet pivotSheet = workbook.Worksheets[1];

                        pivotSheet["A2"].Text = "Nutrition Monthly Services Indicators achievments and progress";
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
                        pivotTable.Fields["District"].Axis = PivotAxisTypes.Page;
                        pivotTable.Fields["FacilityName"].Axis = PivotAxisTypes.Row;

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

                        IPivotField samAdmittedTotal = pivotTable.Fields["samAdmittedTotal"];
                        IPivotField samAdmittedMale = pivotTable.Fields["samAdmittedMale"];
                        IPivotField samAdmittedFemale = pivotTable.Fields["samAdmittedFemale"];
                        IPivotField samAdmitIpd = pivotTable.Fields["samAdmitIpd"];
                        IPivotField samAdmitIpdMale = pivotTable.Fields["samAdmitIpdMale"];
                        IPivotField samAdmitIpdFemale = pivotTable.Fields["samAdmitIpdFemale"];
                        IPivotField samAdmitOpd = pivotTable.Fields["samAdmitOpd"];
                        IPivotField samAdmitOpdMale = pivotTable.Fields["samAdmitOpdMale"];
                        IPivotField samAdmitOpdFemale = pivotTable.Fields["samAdmitOpdFemale"];
                        IPivotField mamU5 = pivotTable.Fields["mamU5"];
                        IPivotField mamPlw = pivotTable.Fields["mamPlw"];
                        IPivotField withSamServices = pivotTable.Fields["withSamServices"];
                        IPivotField withSamIpd = pivotTable.Fields["withSamIpd"];
                        IPivotField withSamOpd = pivotTable.Fields["withSamOpd"];
                        IPivotField mamU5Services = pivotTable.Fields["mamU5Services"];
                        IPivotField mamPlwServices = pivotTable.Fields["mamPlwServices"];
                        IPivotField samCured = pivotTable.Fields["samCured"];
                        IPivotField samDeaths = pivotTable.Fields["samDeaths"];
                        IPivotField samDefaults = pivotTable.Fields["samDefaults"];
                        IPivotField ipdCured = pivotTable.Fields["ipdCured"];
                        IPivotField ipdCureipdDeathsd = pivotTable.Fields["ipdDeaths"];
                        IPivotField ipdDefaults = pivotTable.Fields["ipdDefaults"];

                        pivotTable.DataFields.Add(samAdmittedTotal, "# of U5 cases of SAM admitted (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedMale, "# of U5 cases of SAM admitted male (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmittedFemale, "# of U5 cases of SAM admitted female (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpd, "# of U5 cases of SAM admitted (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdMale, "# of U5 cases of SAM admitted male (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitIpdFemale, "# of U5 cases of SAM admitted female (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpd, "# of U5 cases of SAM admitted (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdMale, "# of U5 cases of SAM admitted male (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(samAdmitOpdFemale, "# of U5 cases of SAM admitted female (OPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5, "# of U5 cases of MAM admitted", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlw, "# of PLW cases of MAM admitted ", PivotSubtotalTypes.Sum);

                        IPivotField Currate = pivotTable.CalculatedFields.Add("samCured3", "samCured/samExists");
                        Currate.Name = "SAM Cure rate (All)";
                        Currate.NumberFormat = "#.0%";

                        IPivotField DefaultRate = pivotTable.CalculatedFields.Add("samDefaults3", "samDefaults/samExists");
                        DefaultRate.Name = "SAM defualt rate (All)";
                        DefaultRate.NumberFormat = "#.0%";


                        IPivotField DeathRate = pivotTable.CalculatedFields.Add("samDeaths3", "samDeaths/samExists");
                        DeathRate.Name = "SAM death rate (All)";
                        DeathRate.NumberFormat = "#.0%";

                        IPivotField IPDCurrate = pivotTable.CalculatedFields.Add("ipdCured3", "ipdCured/ipdExists");
                        IPDCurrate.Name = "SAM Cure rate (IPD)";
                        IPDCurrate.NumberFormat = "#.0%";

                        IPivotField IPDDefaultRate = pivotTable.CalculatedFields.Add("ipdDefaults3", "ipdDefaults/ipdExists");
                        IPDDefaultRate.Name = "SAM defualt rate (IPD)";
                        IPDDefaultRate.NumberFormat = "#.0%";

                        IPivotField IPDDeathRate = pivotTable.CalculatedFields.Add("ipdDeaths3", "ipdDeaths/ipdExists");
                        IPDDeathRate.Name = "SAM death rate (IPD)";
                        IPDDeathRate.NumberFormat = "#.0%";

                        pivotTable.DataFields.Add(withSamServices, "# of nutrition sites with SAM services (OPD+IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(withSamIpd, "# of nutrition sites with SAM services (IPD)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamU5Services, "# of nutrition sites with MAM services (U5)", PivotSubtotalTypes.Sum);
                        pivotTable.DataFields.Add(mamPlwServices, "# of nutrition sites with MAM services (PLW)", PivotSubtotalTypes.Sum);
                        pivotTable.ShowDataFieldInRow = true;
                        pivotTable.Options.RowLayout=PivotTableRowLayout.Tabular;
                        IPivotTableOptions option = pivotTable.Options;
                        //option.ErrorString = "X";
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
                        string filename = "Facility report" + "" + DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day + ".xlsx";

                        MemoryStream ms = new MemoryStream();
                        workbook.SaveAs(ms);
                        ms.Position = 0;
                        workbook.Close();
                        excelEngine.Dispose();
                        return File(ms, ContentType, filename);

                    }
                    break;
            }

            return View();
        }
    }
}
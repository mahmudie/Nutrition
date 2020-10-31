using DataSystem.Models;
using DataSystem.Models.GLM;
using DataSystem.Models.GLM.Dtos;
using DataSystem.Models.GLM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace DataSystem.Controllers.GLM
{
    [Authorize(Roles = "administrator,unicef,pnd,dataentry,guest")]
    public class ExcelReportsController : BaseController
    {
        protected readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ExcelReportsController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult KeyFindings()
        {
            var Years = new List<int?>();
            var yearList = _context.ReportsView.Select(m => new { m.Year }).Distinct().Where(m => m.Year != null).ToList();

            foreach (var yl in yearList)
            {
                Years.Add(yl.Year);
            }

            var provinces = _context.Provinces.Select(m => new ProvinceDropdownDto { Id = m.ProvCode, Name = m.ProvName })
                .OrderBy(m => m.Name)
                .ToList();

            var vm = new ReportDownloadViewModel()
            {
                Provinces = provinces,
                Years = Years
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KeyFindings(ReportDownloadViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var Years = new List<int?>();
                var yearList = _context.ReportsView.Select(m => new { m.Year }).Distinct().Where(m => m.Year != null).ToList();

                foreach (var yl in yearList)
                {
                    Years.Add(yl.Year);
                }

                var provinces = _context.Provinces.Select(m => new ProvinceDropdownDto { Id = m.ProvCode, Name = m.ProvName })
                    .OrderBy(m => m.Name)
                    .ToList();

                vm.Provinces = provinces;
                vm.Years = Years;

                return View(vm);
            }

            // get reports
            var reports = _context.ReportsView
                .Where(m => m.ProvinceId == vm.ProvinceId && m.Year == vm.Year)
                .OrderBy(m => m.Id)
                .OrderBy(m => m.ReportReceivedDate)
                .ToList();


            if (reports.Count > 0)
            {
                var sections = _context.Sections
                    .Where(m => m.Id == 21)
                    .OrderBy(m => m.SortOrder)
                    .ToList();

                var scqfs = _context.SectionColQuestionFields
                    .Where(m => m.SectionId == 21)
                    .ToList();

                var fieldOptionsAll = _context.FieldOptions.ToList();
                var fieldOptions = new List<FieldOption>();

                var tables = new List<DataTable>();

                if (scqfs.Count > 0)
                {
                    foreach (var section in sections)
                    {
                        var columns = scqfs
                            .Select(m => new {
                                Id = m.ColumnId,
                                SectionId = m.SectionId,
                                Title = m.ColumnTitle,
                                SortOrder = m.CSortOrder,
                                Type = m.ColumnType
                            })
                            .Distinct()
                            .Where(m => m.SectionId == section.Id)
                            .ToList();

                        var questions = scqfs
                            .Select(m => new {
                                Id = m.QuestionId,
                                SectionId = m.SectionId,
                                Title = m.QuestionTitle,
                                SortOrder = m.QSortOrder
                            })
                            .Distinct()
                            .Where(m => m.SectionId == section.Id)
                            .ToList();

                        // create data tabel
                        var sectionTable = new DataTable(section.Title);
                        sectionTable.TableName = section.Title;

                        // add the main columns
                        //sectionTable.Columns.Add("?");
                        sectionTable.Columns.Add("Province");
                        sectionTable.Columns.Add("District");
                        sectionTable.Columns.Add("Facility");
                        sectionTable.Columns.Add("Year");
                        sectionTable.Columns.Add("Month");
                        sectionTable.Columns.Add("Date");

                        if (columns.Count > 0 && questions.Count > 0)
                        {
                            // add columns to the table
                            foreach (var c in columns)
                            {
                                if (c.Type == "standard")
                                {
                                    sectionTable.Columns.Add(c.Title);
                                }
                            }

                            // add report rows to the table
                            foreach (var report in reports)
                            {
                                // add question rows to the table
                                foreach (var q in questions)
                                {
                                    DataRow tableRow = sectionTable.NewRow();

                                    // add question title
                                    //tableRow["?"] = q.Title;

                                    // add report column titles
                                    tableRow["Province"] = report.Province;
                                    tableRow["District"] = report.District;
                                    tableRow["Facility"] = report.Facility + " / " + report.FacilityID;
                                    tableRow["Year"] = report.Year;
                                    tableRow["Month"] = report.Month;
                                    tableRow["Date"] = String.Format("{0:MM/dd/yyyy}", report.ReportReceivedDate);

                                    // add fields data to the row
                                    foreach (var c in columns)
                                    {
                                        //var field = _context.Fields
                                        //    .Include(m => m.FieldOptions)
                                        //    .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
                                        //    .SingleOrDefault();

                                        var field = scqfs
                                            .Select(m => new
                                            {
                                                Id = m.FieldId,
                                                DataType = m.DataType,
                                                InputType = m.InputType,
                                                ColumnId = m.ColumnId,
                                                QuestionId = m.QuestionId,
                                            })
                                            .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
                                            .SingleOrDefault();

                                        if (field != null)
                                        {
                                            // get field options for the dropdown types
                                            if (field.InputType == "dropdown" && field.InputType != "yesno")
                                            {
                                                fieldOptions = fieldOptionsAll
                                                    .Select(m => new FieldOption
                                                    {
                                                        Value = m.Value,
                                                        Caption = m.Caption,
                                                        FieldId = m.FieldId
                                                    })
                                                    .Where(m => m.FieldId == field.Id)
                                                    .ToList();
                                            }

                                            // add text field value
                                            if (field.DataType == "text")
                                            {
                                                if (_context.TextValues.Any(m => m.FieldId == field.Id && m.ReportId == report.Id))
                                                {
                                                    var textValue = _context.TextValues
                                                    .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                                    .SingleOrDefault();

                                                    if (field.InputType == "dropdown" && field.DataType != "yesno")
                                                    {
                                                        

                                                        foreach (var fo in fieldOptions)
                                                        {
                                                            if (fo.Value == textValue.Data)
                                                            {
                                                                tableRow[c.Title] = (String.IsNullOrEmpty(textValue.Data)) ? "N/A" : fo.Caption;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tableRow[c.Title] = (textValue != null) ? textValue.Data : null;
                                                    }
                                                }
                                            }

                                            // add number field value
                                            if (field.DataType == "number" || field.DataType == "yesno")
                                            {
                                                if (_context.NumberValues.Any(m => m.FieldId == field.Id && m.ReportId == report.Id))
                                                {
                                                    var numberValue = _context.NumberValues
                                                        .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                                        .SingleOrDefault();

                                                    if (field.InputType == "dropdown" && field.DataType != "yesno")
                                                    {
                                                        foreach (var fo in fieldOptions)
                                                        {
                                                            if (fo.Value == numberValue.Data.ToString())
                                                            {
                                                                tableRow[c.Title] = fo.Caption;
                                                            }
                                                            else
                                                            {
                                                                tableRow[c.Title] = "N/A";
                                                            }
                                                        }
                                                    }
                                                    else if (field.DataType == "yesno")
                                                    {
                                                        tableRow[c.Title] = (numberValue.Data == 2) ? "N/A" : numberValue.Data.ToString();
                                                    }
                                                    else
                                                    {
                                                        tableRow[c.Title] = (numberValue != null) ? numberValue.Data.ToString() : null;
                                                    }
                                                }
                                            }

                                            // add date field value
                                            if (field.DataType == "date")
                                            {
                                                if (_context.DateValues.Any(m => m.FieldId == field.Id && m.ReportId == report.Id))
                                                {
                                                    var dateValue = _context.DateValues
                                                    .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                                    .SingleOrDefault();

                                                    tableRow[c.Title] = (dateValue != null) ? String.Format("{0:MM/dd/yyyy}", dateValue.Data) : null;
                                                }
                                            }
                                        }
                                    }

                                    // add row to the table
                                    sectionTable.Rows.Add(tableRow);
                                }
                            }
                        }

                        tables.Add(sectionTable);
                    }
                }

                //return Json(tables);
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    IWorkbook workbook = application.Workbooks.Create(1);

                    foreach (var t in tables)
                    {
                        IWorksheet sheet = workbook.Worksheets[0];

                        sheet.Name = "Key Findings";

                        sheet.ImportDataTable(t, true, 1, 1, true);

                        IListObject table = sheet.ListObjects.Create("KeyFindings", sheet.UsedRange);

                        table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium14;

                        sheet.UsedRange.AutofitColumns();
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        var prov = _context.Provinces
                            .Select(m => new { m.ProvCode, m.ProvName })
                            .SingleOrDefault(m => m.ProvCode == vm.ProvinceId);

                        var filename = "KeyFindings_" + prov.ProvName + "_" + vm.Year + ".xlsx";

                        workbook.SaveAs(stream);

                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Reports");
            }
        }

        public IActionResult DownloadSections(string Id = null)
        {
            if (_context.Reports.Any(m => m.Id == Id))
            {
                var sections = _context.Sections
                    .Where(m => m.DataFormId == 1)
                    .OrderBy(m => m.SortOrder)
                    .ToList();

                var vm = new ExcelReportsViewModel()
                {
                    Sections = sections,
                    ReportId = Id
                };

                return View(vm);
            }
            else
            {
                return RedirectToAction("Index", "Reports");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DownloadSections(ExcelReportsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var sections = _context.Sections
                    .Where(m => m.DataFormId == 1)
                    .OrderBy(m => m.SortOrder)
                    .ToList();

                vm.Sections = sections;

                return View("Index", vm);
            }

            if (_context.Reports.Any(m => m.Id == vm.ReportId))
            {
                var sections = _context.Sections
                    .Where(m => vm.SectionIds.Contains(m.Id))
                    .OrderBy(m => m.SortOrder)
                    .ToList();

                var scqfs = _context.SectionColQuestionFields
                    .Where(m => vm.SectionIds.Contains(m.SectionId))
                    .ToList();

                var tables = new List<DataTable>();

                if (scqfs.Count > 0)
                {
                    foreach (var section in sections)
                    {
                        var columns = scqfs
                            .Select(m => new {
                                Id = m.ColumnId,
                                SectionId = m.SectionId,
                                Title = m.ColumnTitle,
                                SortOrder = m.CSortOrder,
                                Type = m.ColumnType
                            })
                            .Distinct()
                            .Where(m => m.SectionId == section.Id)
                            .ToList();

                        var questions = scqfs
                            .Select(m => new {
                                Id = m.QuestionId,
                                SectionId = m.SectionId,
                                Title = m.QuestionTitle,
                                SortOrder = m.QSortOrder
                            })
                            .Distinct()
                            .Where(m => m.SectionId == section.Id)
                            .ToList();

                        // create data tabel
                        var sectionTable = new DataTable(section.Title);
                        sectionTable.TableName = section.Title;

                        // add the main question column
                        sectionTable.Columns.Add("?");

                        if (columns.Count > 0 && questions.Count > 0)
                        {
                            // add columns to the table
                            foreach (var c in columns)
                            {
                                if (c.Type == "standard")
                                {
                                    sectionTable.Columns.Add(c.Title);
                                }
                            }

                            // add question rows to the table
                            foreach (var q in questions)
                            {
                                DataRow tableRow = sectionTable.NewRow();

                                // add question title
                                tableRow["?"] = q.Title;

                                // add fields data to the row
                                foreach (var c in columns)
                                {
                                    var field = scqfs
                                        .Select(m => new
                                        {
                                            Id = m.FieldId,
                                            DataType = m.DataType,
                                            ColumnId = m.ColumnId,
                                            QuestionId = m.QuestionId,
                                        })
                                        .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
                                        .SingleOrDefault();

                                    if (field != null)
                                    {
                                        // add text field value
                                        if (field.DataType == "text")
                                        {
                                            var textValue = _context.TextValues
                                                .Where(m => m.ReportId == vm.ReportId && m.FieldId == field.Id)
                                                .SingleOrDefault();

                                            tableRow[c.Title] = (textValue != null) ? textValue.Data : null;
                                        }

                                        // add number field value
                                        if (field.DataType == "number" || field.DataType == "yesno")
                                        {
                                            var numberValue = _context.NumberValues
                                                .Where(m => m.ReportId == vm.ReportId && m.FieldId == field.Id)
                                                .SingleOrDefault();

                                            if (numberValue != null)
                                            {
                                                if (field.DataType == "yesno" && numberValue.Data == 2)
                                                {
                                                    tableRow[c.Title] = "N/A";
                                                }
                                                else
                                                {
                                                    tableRow[c.Title] = numberValue.Data.ToString();
                                                }
                                            }
                                            else
                                            {
                                                tableRow[c.Title] = null;
                                            }
                                        }

                                        // add date field value
                                        if (field.DataType == "date")
                                        {
                                            var dateValue = _context.DateValues
                                                .Where(m => m.ReportId == vm.ReportId && m.FieldId == field.Id)
                                                .SingleOrDefault();

                                            tableRow[c.Title] = (dateValue != null) ? String.Format("{0:MM/dd/yyyy}", dateValue.Data) : null;
                                        }
                                    }
                                }

                                // add row to the table
                                sectionTable.Rows.Add(tableRow);
                            }
                        }

                        tables.Add(sectionTable);
                    }
                }

                //return Json(tables);
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    IWorkbook workbook = application.Workbooks.Create(tables.Count);

                    int counter = 0;

                    foreach (var t in tables)
                    {
                        IWorksheet sheet = workbook.Worksheets[counter];

                        sheet.Name = (counter + 1) + "_" + Slugify(t.TableName);

                        sheet.ImportDataTable(t, true, 1, 1, true);

                        IListObject table = sheet.ListObjects.Create("List_" + counter, sheet.UsedRange);

                        table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium14;

                        sheet.UsedRange.AutofitColumns();

                        // increment the counter
                        counter++;
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        var filename = "Report_";

                        filename += DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day;
                        filename += "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;

                        filename += ".xlsx";

                        workbook.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Reports");
            }

            //if (_context.Reports.Any(m => m.Id == vm.ReportId))
            //{
            //    var sections = _context.Sections
            //        .Where(m => vm.SectionIds.Contains(m.Id))
            //        .OrderBy(m => m.SortOrder)
            //        .ToList();

            //    var tables = new List<DataTable>();

            //    if (sections.Count > 0)
            //    {
            //        foreach (var section in sections)
            //        {
            //            var columns = _context.Columns
            //                .Where(m => m.SectionId == section.Id)
            //                .OrderBy(m => m.SortOrder)
            //                .ToList();

            //            var questions = _context.Questions
            //                .Where(m => m.SectionId == section.Id)
            //                .OrderBy(m => m.SortOrder)
            //                .ToList();

            //            // create data tabel
            //            var sectionTable = new DataTable(section.Title);
            //            sectionTable.TableName = section.Title;

            //            // add the main question column
            //            sectionTable.Columns.Add("?");

            //            if (columns.Count > 0 && questions.Count > 0)
            //            {
            //                // add columns to the table
            //                foreach (var c in columns)
            //                {
            //                    if (c.ColumnType == "standard")
            //                    {
            //                        sectionTable.Columns.Add(c.Title);
            //                    }
            //                }

            //                // add question rows to the table
            //                foreach (var q in questions)
            //                {
            //                    DataRow tableRow = sectionTable.NewRow();

            //                    // add question title
            //                    tableRow["?"] = q.Title;

            //                    // add fields data to the row
            //                    foreach (var c in columns)
            //                    {
            //                        var field = _context.Fields
            //                            .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
            //                            .SingleOrDefault();

            //                        if (field != null)
            //                        {
            //                            // add text field value
            //                            if (field.DataType == "text")
            //                            {
            //                                var textValue = _context.TextValues
            //                                    .Where(m => m.ReportId == vm.ReportId && m.FieldId == field.Id)
            //                                    .SingleOrDefault();

            //                                tableRow[c.Title] = (textValue != null) ? textValue.Data : null;
            //                            }

            //                            // add number field value
            //                            if (field.DataType == "number" || field.DataType == "yesno")
            //                            {
            //                                var numberValue = _context.NumberValues
            //                                    .Where(m => m.ReportId == vm.ReportId && m.FieldId == field.Id)
            //                                    .SingleOrDefault();

            //                                if (numberValue != null)
            //                                {
            //                                    if (field.DataType == "yesno" && numberValue.Data == 2)
            //                                    {
            //                                        tableRow[c.Title] = "N/A";
            //                                    }
            //                                    else
            //                                    {
            //                                        tableRow[c.Title] = numberValue.Data.ToString();
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    tableRow[c.Title] = null;
            //                                }
            //                            }

            //                            // add date field value
            //                            if (field.DataType == "date")
            //                            {
            //                                var dateValue = _context.DateValues
            //                                    .Where(m => m.ReportId == vm.ReportId && m.FieldId == field.Id)
            //                                    .SingleOrDefault();

            //                                tableRow[c.Title] = (dateValue != null) ? String.Format("{0:MM/dd/yyyy}", dateValue.Data) : null;
            //                            }
            //                        }
            //                    }

            //                    // add row to the table
            //                    sectionTable.Rows.Add(tableRow);
            //                }
            //            }

            //            tables.Add(sectionTable);
            //        }
            //    }

            //    //return Json(tables);
            //    using(ExcelEngine excelEngine = new ExcelEngine())
            //    {
            //        IApplication application = excelEngine.Excel;
            //        application.DefaultVersion = ExcelVersion.Excel2016;

            //        IWorkbook workbook = application.Workbooks.Create(tables.Count);

            //        int counter = 0;

            //        foreach (var t in tables)
            //        {
            //            IWorksheet sheet = workbook.Worksheets[counter];

            //            sheet.Name = (counter + 1) + "_" + Slugify(t.TableName);

            //            sheet.ImportDataTable(t, true, 1, 1, true);

            //            IListObject table = sheet.ListObjects.Create("List_" + counter, sheet.UsedRange);

            //            table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium14;

            //            sheet.UsedRange.AutofitColumns();

            //            // increment the counter
            //            counter++;
            //        }

            //        using (MemoryStream stream = new MemoryStream())
            //        {
            //            var filename = "Report_";

            //            filename += DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day;
            //            filename += "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute;

            //            filename += ".xlsx";

            //            workbook.SaveAs(stream);
            //            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            //        }
            //    }
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Reports");
            //}
        }
    }
}
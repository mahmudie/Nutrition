using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.GLM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.XlsIO;

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

        public async Task<ActionResult> Index(string Id = null)
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

                var tables = new List<DataTable>();

                if (sections.Count > 0)
                {
                    foreach (var section in sections)
                    {
                        var columns = _context.Columns
                            .Where(m => m.SectionId == section.Id)
                            .OrderBy(m => m.SortOrder)
                            .ToList();

                        var questions = _context.Questions
                            .Where(m => m.SectionId == section.Id)
                            .OrderBy(m => m.SortOrder)
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
                                if (c.ColumnType == "standard")
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
                                    var field = _context.Fields
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
                using(ExcelEngine excelEngine = new ExcelEngine())
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
        }
    }
}
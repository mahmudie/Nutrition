using DataSystem.GLM.Dtos;
using DataSystem.Models;
using DataSystem.Models.GLM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TEST1.Controllers
{
    public class FieldsController : Controller
    {
        protected readonly WebNutContext _context;

        public FieldsController(WebNutContext context)
        {
            _context = context;
        }

        public IActionResult Index(long? SectionId)
        {
            if (SectionId != null)
            {
                ViewData["SectionTitle"] = _context.Sections
                    .Where(m => m.Id == SectionId)
                    .Select(m => m.Title)
                    .SingleOrDefault();

                var columns = _context.Columns
                    .Where(m => m.SectionId == SectionId)
                    .OrderBy(m => m.SortOrder)
                    .ToList();

                var questions = _context.Questions
                    .Where(m => m.SectionId == SectionId)
                    .OrderBy(m => m.SortOrder)
                    .ToList();

                // prepare the grid
                var grid = new FormGridViewModel();

                if (questions.Count() > 0 && columns.Count() > 0)
                {
                    // add columns grid's columns property
                    foreach (var c in columns)
                    {
                        if (c.ColumnType == "standard")
                        {
                            grid.Columns.Add(c);
                        }
                    }

                    // add question rows ro grid's questions property
                    foreach (var q in questions)
                    {
                        var question = new FormGridQuestionViewModel();

                        question.QuestionTitle = q.Title;

                        // add fields to question row's fields property
                        foreach (var c in columns)
                        {
                            if (c.ColumnType == "standard")
                            {
                                var field = new FormGridQuestionFieldViewModel();

                                field.QuestionId = q.Id;
                                field.ColumnId = c.Id;

                                field.Id = _context.Fields
                                    .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
                                    .Select(m => m.Id)
                                    .SingleOrDefault();

                                question.Fields.Add(field);
                            }
                        }

                        grid.Questions.Add(question);
                    }
                }

                return View(grid);
            }
            else
            {
                return RedirectToAction("Index", "Sections");
            }
        }

        public IActionResult Create(long? QuestionId, long? ColumnId)
        {
            if (QuestionId != null && ColumnId != null)
            {
                if (!_context.Fields.Any(m => m.QuestionId == QuestionId && m.ColumnId == ColumnId))
                {
                    var viewModel = new FieldViewModel()
                    {
                        QuestionId = (long)QuestionId,
                        ColumnId = (long)ColumnId
                    };

                    return View(viewModel);
                } 
                else
                {
                    return RedirectToAction("Index", "Sections");
                }
            }
            else
            {
                return RedirectToAction("Index", "Sections");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FieldViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // check if this id is taken
            if (_context.Fields.Any(m => m.Id == viewModel.Id))
            {
                ViewBag.Error = true;
                ViewBag.ErrorMessage = "The Field Id: " + viewModel.Id + " already exists.";

                return View(viewModel);
            }

            // check the input type 
            if (viewModel.InputType == null || viewModel.DataType == "date")
            {
                viewModel.InputType = (viewModel.DataType == "yesno") ? "dropdown" : "textbox";
            }

            var field = new Field();

            field.Id = (long)viewModel.Id;
            field.QuestionId = viewModel.QuestionId;
            field.ColumnId = viewModel.ColumnId;
            field.DataType = viewModel.DataType;
            field.InputType = viewModel.InputType;
            field.IsRequired = viewModel.IsRequired;
            field.YesNoDefaultCaption = viewModel.YesNoDefaultCaption;
            field.IsExpiryDate = viewModel.IsExpiryDate;
            field.ExpiryWarningPeriod = viewModel.ExpiryWarningPeriod;

            _context.Fields.Add(field);

            _context.SaveChanges();

            //---

            // set empty values fields for already added reports in the related table 
            // based on the field's selected data type 
            var reports = _context.Reports.Select(m => new { Id = m.Id }).ToList();

            if (reports.Count() > 0)
            {
                // add empty fields into the text values table
                if (field.DataType == "text")
                {
                    foreach (var report in reports)
                    {
                        _context.TextValues.Add(new TextValue
                        {
                            ReportId = report.Id,
                            FieldId = field.Id,
                            Data = null
                        });
                    }
                }

                // add empty fields into the number values table
                if (field.DataType == "number" || field.DataType == "yesno")
                {
                    foreach (var report in reports)
                    {
                        _context.NumberValues.Add(new NumberValue
                        {
                            ReportId = report.Id,
                            FieldId = field.Id,
                            Data = null
                        });
                    }
                }

                // add empty fields into the date values table
                if (field.DataType == "date")
                {
                    foreach (var report in reports)
                    {
                        _context.DateValues.Add(new DateValue
                        {
                            ReportId = report.Id,
                            FieldId = field.Id,
                            Data = null
                        });
                    }
                }

                _context.SaveChanges();
            }

            var sectionId = _context.Columns
                .Where(m => m.Id == viewModel.ColumnId)
                .Select(m => m.SectionId)
                .SingleOrDefault();

            if (viewModel.InputType == "dropdown" && viewModel.DataType != "yesno")
            {
                return RedirectToAction("Edit", "Fields", new { Id = field.Id });
            }
            else
            {
                return RedirectToAction("Index", "Fields", new { SectionId = sectionId });
            }
        }

        public IActionResult Edit(long Id)
        {
            var field = _context.Fields
                .Where(m => m.Id == Id)
                .Include(m => m.Question)
                .SingleOrDefault();

            var viewModel = new FieldViewModel()
            {
                Id = field.Id,
                DataType = field.DataType,
                InputType = field.InputType,
                IsRequired = field.IsRequired,
                YesNoDefaultCaption = field.YesNoDefaultCaption,
                IsExpiryDate = field.IsExpiryDate,
                ExpiryWarningPeriod = field.ExpiryWarningPeriod,
                QuestionId = field.QuestionId,
                ColumnId = field.ColumnId,
                //
                FieldOptions = _context.FieldOptions
                    .Where(m => m.FieldId == Id)
                    .OrderBy(m => m.Value)
                    .ToList()
            };

            ViewBag.SectionId = field.Question.SectionId;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FieldViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.FieldOptions = _context.FieldOptions
                    .Where(m => m.FieldId == viewModel.Id)
                    .OrderBy(m => m.Value)
                    .ToList();

                return View(viewModel);
            }

            // check if this id is taken
            if (_context.Fields.Any(m => m.Id == viewModel.Id && (m.QuestionId != viewModel.QuestionId || m.ColumnId != viewModel.ColumnId)))
            {
                ViewBag.Error = true;
                ViewBag.ErrorMessage = "The Field Id: " + viewModel.Id + " already exists.";

                return View(viewModel);
            }

            // check the input type 
            if (viewModel.InputType == null || viewModel.DataType == "date")
            {
                viewModel.InputType = "textbox";
            }

            var field = _context.Fields.Find(viewModel.Id);

            // this will be used to remove old data from vlaues tbale
            // in case of field data type change
            var previousDataType = field.DataType;

            field.Id = (long)viewModel.Id;
            field.DataType = viewModel.DataType;
            field.InputType = viewModel.InputType;
            field.IsRequired = viewModel.IsRequired;
            field.YesNoDefaultCaption = viewModel.YesNoDefaultCaption;
            field.IsExpiryDate = viewModel.IsExpiryDate;
            field.ExpiryWarningPeriod = viewModel.ExpiryWarningPeriod;

            _context.Entry(field).State = EntityState.Modified;

            _context.SaveChanges();

            //---

            // remove value from the related value table in case of data type change
            // and also set empty values fields for already added reports in the new
            // table based on the selected data type
            var reports = _context.Reports.Select(m => new { Id = m.Id }).ToList();

            if (reports.Count() > 0)
            {
                // check if the data type has been changed
                // if so make necessary change to the related values tables
                if (field.DataType != previousDataType)
                {
                    // check the old data type of the field and remove values
                    // from old values table related to the data type 

                    // remove related records from text values table
                    if (previousDataType == "text")
                    {
                        var FieldId = new SqlParameter("@FieldId", field.Id);
                        var sql = "DELETE FROM TextValues WHERE FieldId = @FieldId";

                        _context.Database.ExecuteSqlCommand(sql, FieldId);
                    }

                    // remove related records from number values table
                    if (previousDataType == "number" || previousDataType == "yesno")
                    {
                        var FieldId = new SqlParameter("@FieldId", field.Id);
                        var sql = "DELETE FROM NumberValues WHERE FieldId = @FieldId";

                        _context.Database.ExecuteSqlCommand(sql, FieldId);
                    }

                    // remove related records from date values table
                    if (previousDataType == "date")
                    {
                        var FieldId = new SqlParameter("@FieldId", field.Id);
                        var sql = "DELETE FROM DateValues WHERE FieldId = @FieldId";

                        _context.Database.ExecuteSqlCommand(sql, FieldId);
                    }


                    // add empty values in the new values table based on the
                    // new data type selected for the field

                    // add empty fields into the text values table
                    if (field.DataType == "text")
                    {
                        foreach (var report in reports)
                        {
                            _context.TextValues.Add(new TextValue
                            {
                                ReportId = report.Id,
                                FieldId = field.Id,
                                Data = null
                            });
                        }
                    }

                    // add empty fields into the number values table
                    if (field.DataType == "number" || field.DataType == "yesno")
                    {
                        foreach (var report in reports)
                        {
                            _context.NumberValues.Add(new NumberValue
                            {
                                ReportId = report.Id,
                                FieldId = field.Id,
                                Data = null
                            });
                        }
                    }

                    // add empty fields into the date values table
                    if (field.DataType == "date")
                    {
                        foreach (var report in reports)
                        {
                            _context.DateValues.Add(new DateValue
                            {
                                ReportId = report.Id,
                                FieldId = field.Id,
                                Data = null
                            });
                        }
                    }

                    _context.SaveChanges();
                }
            }

            var sectionId = _context.Columns
                .Where(m => m.Id == field.ColumnId)
                .Select(m => m.SectionId)
                .SingleOrDefault();

            return RedirectToAction("Index", "Fields", new { SectionId = sectionId });
        }

        public IActionResult Delete(long Id)
        {
            var field = _context.Fields
                .Include(m => m.Question)
                .Where(m => m.Id == Id)
                .SingleOrDefault();

            return View(field);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(LongId dto)
        {
            var field = _context.Fields
                .Include(m => m.Question)
                .Where(m => m.Id == dto.Id)
                .SingleOrDefault();

            _context.Fields.Remove(field);
            _context.SaveChanges();

            return RedirectToAction("Index", "Fields", new { SectionId = field.Question.SectionId });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.GLM.Dtos;
using DataSystem.Models.GLM;
using DataSystem.Models;
using DataSystem.GLM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using DataSystem.ViewModels;
using DataSystem.Models.GLM.ViewModels;
using System.IO;
using System.Data.SqlClient;

namespace DataSystem.GLM.Controllers
{
    [Authorize(Roles = "administrator,unicef,pnd,dataentry")]
    public class ReportsController : Controller
    {
        protected readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReportsController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var reports  = _context.ReportsView
                .Include(m => m.DataForm)
                .Include(m => m.DateValues)
                .OrderByDescending(m => m.UpdateDate)
                .ToList(); 

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (User.IsInRole("dataentry") && (user.Unicef == 0 && user.Pnd == 0))
            {
                reports = reports.Where(m => m.UserName == user.UserName).ToList();
            }
            else if (User.IsInRole("administrator") && (user.Unicef == 0 && user.Pnd == 0))
            {
                reports = reports.Where(m => m.TenantId == user.TenantId).ToList();
            }
            // prepare the final reports list view model with expiry warning
            // and other configurations

                List<ReportListViewModel> reportList = new List<ReportListViewModel>();

            foreach (var r in reports)
            {
                var report = new ReportListViewModel();

                report.ReportId = r.Id;
                report.Province = r.Province;
                report.District = r.District;
                report.FacilityId = r.FacilityID;
                report.Facility = r.Facility;
                report.ReportedBy = r.ReportedBy;
                report.DataCollectorOffice = r.DataCollectorOffice;
                report.DataForm = r.DataForm.Name;
                report.PreparedDate = r.ReportPreparedDate;
                report.IsCompleted = r.IsCompleted;

                // check for the date field with expiry set to True and
                // check if the date is in expiry warning period and then
                // set expiry warning flag
                if (!r.IsCompleted)
                {
                    if (r.DateValues.Count() > 0)
                    {
                        foreach (var dv in r.DateValues)
                        {
                            if (dv.Data != null && dv.IsExpiryDate==true && dv.ExpiryWarningPeriod != null)
                            {
                                if ((DateTime)dv.Data <= DateTime.Now.AddDays((double)dv.ExpiryWarningPeriod))
                                {
                                    report.HasExpiryWarning = true;
                                }
                            }
                        }
                    }
                }

                // add report to the list
                reportList.Add(report);
            }

            return View(reportList);
        }

        public IActionResult Create()
        {
            var viewModel = new ReportViewModel()
            {
                DataForms = _context.DataForms.ToList()
            };

            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                DistrictName = m.DistName,
                ProvinceId = m.ProvCode
            }).ToList();

            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                FacilityName = m.FacilityId + "-" + m.FacilityName,
                DistrictId = m.DistCode
            }).ToList();

            var implementers = _context.Implementers.Where(m => m.ImpAcronym != null).Select(m => new
            {
                ImpId = m.ImpCode,
                ImpName = m.ImpAcronym
            }).ToList();

            ViewBag.provinces = provinces;
            ViewBag.districts = districts;
            ViewBag.hfs = facilities;
            ViewBag.imps = implementers;

            DateTime today_date = DateTime.Now.Date;
            ViewBag.minDate = today_date.AddDays(-90);
            ViewBag.maxDate = today_date.AddDays(1);
            ViewBag.maxDatemin = today_date;
            ViewBag.maxDeadline = today_date.AddDays(30);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportViewModel viewModel)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (!ModelState.IsValid)
            {
                viewModel.DataForms = _context.DataForms.ToList();

                return View(viewModel);
            }

            string Id = "";

            // get todays date in number of days
            var datetime = Convert.ToDateTime(viewModel.ReportPreparedDate);
            //TimeSpan ts = (new DateTime(datetime.Year, datetime.Month, datetime.Day) - new DateTime(1, 1, 1));
            //var days = (ts.TotalDays + 1).ToString();

            // Get year number

            var year = datetime.Year.ToString("D4");
            //Get month number 
            var month = datetime.Month.ToString("D2");

            //Get weeknumber of the data

            var weeknum = WeekNumber(datetime).ToString("D2");

            // pad facility id with zeros
            var facilityId = Convert.ToInt64(viewModel.FacilityId).ToString("D6");

            // concatenate all the parts
            Id += viewModel.DataFormId.ToString();
            Id += "" + facilityId;
            Id += "" + year;
            Id += "" + month;
            Id += "" + weeknum;

            // check if report id is usnique 
            if (_context.Reports.Any(m => m.Id == Id))
            {
                ViewBag.Error = true;
                ViewBag.ErrorMessage = "This report already exists.";

                viewModel.DataForms = _context.DataForms.ToList();

                return View(viewModel);
            }

            var facility = _context.FacilityInfo.Where(m => m.FacilityId.Equals(viewModel.FacilityId)).FirstOrDefault();
            var report = new Report()
            {
                Id = Id,
                ProvinceId = viewModel.ProvinceId,
                DistrictId = viewModel.DistrictId,
                ReportedBy = viewModel.ReportedBy,
                ReportPreparedDate = viewModel.ReportPreparedDate,
                ReportReceivedDate = viewModel.ReportReceivedDate,
                DataFormId = viewModel.DataFormId,
                FacilityId = viewModel.FacilityId,
                FacilityTypeId = (int)facility.FacilityType,
                DataCollectorOffice = viewModel.DataCollectorOffice,
                ImpId = viewModel.ImpId,
                ReportLat = 0,
                ReportLon = 0,
                TenantId = user.TenantId,
                UserName = user.UserName,
                UpdateDate = DateTime.Now.Date
            };

            _context.Reports.Add(report);

            _context.SaveChanges();

            // add empty report details
            // set report detail fields

            var ReportId = report.Id;

            var textFields = _context.Fields
                    .Where(m => m.DataType == "text")
                    .ToList();

            var numberFields = _context.Fields
                .Where(m => m.DataType == "number" || m.DataType == "yesno")
                .ToList();

            var dateFields = _context.Fields
                .Where(m => m.DataType == "date")
                .ToList();

            // add empty fields with text datatype
            foreach (var field in textFields)
            {
                _context.TextValues.Add(new TextValue
                {
                    ReportId = Id,
                    FieldId = field.Id,
                    Data = null
                });
            }

            _context.SaveChanges();

            // add empty fields with number datatype
            foreach (var field in numberFields)
            {
                _context.NumberValues.Add(new NumberValue
                {
                    ReportId = Id,
                    FieldId = field.Id,
                    Data = null
                });
            }

            _context.SaveChanges();

            // add empty fields with date datatype
            foreach (var field in dateFields)
            {
                _context.DateValues.Add(new DateValue
                {
                    ReportId = Id,
                    FieldId = field.Id,
                    Data = null
                });
            }

            _context.SaveChanges();

            return RedirectToAction("ReportDetails", "Reports", new { Id = Id });
        }

        public IActionResult Edit(string Id)
        {
            var provinces = _context.Provinces.Select(m => new
            {
                ProvinceId = m.ProvCode,
                ProvinceName = m.ProvName
            }).ToList();

            var districts = _context.Districts.Select(m => new
            {
                DistrictId = m.DistCode,
                DistrictName = m.DistName,
                ProvinceId = m.ProvCode
            }).ToList();

            var facilities = _context.FacilityInfo.Select(m => new
            {
                FacilityId = m.FacilityId,
                FacilityName = m.FacilityId + "-" + m.FacilityName,
                DistrictId = m.DistCode
            }).ToList();

            var implementers = _context.Implementers.Where(m => m.ImpAcronym != null).Select(m => new
            {
                ImpId = m.ImpCode,
                ImpName = m.ImpAcronym
            }).ToList();

            ViewBag.provinces = provinces;
            ViewBag.districts = districts;
            ViewBag.hfs = facilities;
            ViewBag.imps = implementers;

            DateTime today_date = DateTime.Now.Date;
            ViewBag.minDate = today_date.AddDays(-90);
            ViewBag.maxDate = today_date.AddDays(1);
            ViewBag.maxDeadline = today_date.AddDays(30);

            var report = _context.Reports.Find(Id);
            var viewModel = new ReportViewModel();

            viewModel.Id = report.Id;
            viewModel.ProvinceId = report.ProvinceId;
            viewModel.DistrictId = report.DistrictId;
            viewModel.FacilityId = report.FacilityId;
            viewModel.ImpId = report.ImpId;
            viewModel.DataCollectorOffice = report.DataCollectorOffice;
            viewModel.ReportedBy = report.ReportedBy;
            viewModel.ReportPreparedDate = report.ReportPreparedDate;
            viewModel.ReportReceivedDate = report.ReportReceivedDate;
            viewModel.ReportLat = report.ReportLat;
            viewModel.ReportLon = report.ReportLon;
            viewModel.DataFormId = report.DataFormId;
            viewModel.TenantId = (int)report.TenantId;

            viewModel.DataForms = _context.DataForms.ToList();

            ViewBag.reportId = Id;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReportViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);


            if (!ModelState.IsValid)
            {
                viewModel.DataForms = _context.DataForms.ToList();

                return View(viewModel);
            }

            var report = _context.Reports.Find(viewModel.Id);
            var facility = _context.FacilityInfo.Where(m => m.FacilityId.Equals(report.FacilityId)).FirstOrDefault();

            report.ProvinceId = viewModel.ProvinceId;
            report.DistrictId = viewModel.DistrictId;
            report.FacilityId = viewModel.FacilityId;
            report.ReportedBy = viewModel.ReportedBy;
            report.FacilityTypeId = (int)facility.FacilityType;
            report.ReportPreparedDate = viewModel.ReportPreparedDate;
            report.ReportReceivedDate = viewModel.ReportReceivedDate;
            report.DataCollectorOffice = viewModel.DataCollectorOffice;
            report.ReportLat = viewModel.ReportLat;
            report.ReportLon = viewModel.ReportLon;
            report.DataFormId = viewModel.DataFormId;
            report.TenantId = (int)report.TenantId;
            report.UserName = user.UserName;
            report.UpdateDate = DateTime.Now.Date;
            report.ImpId = viewModel.ImpId;


            _context.Entry(report).State = EntityState.Modified;

            _context.SaveChanges();

            return RedirectToAction("Index", "Reports");
        }

        public IActionResult ReportDetails(string Id)
        {
            // if report doesn't exist redirect to reports index page
            if (!_context.Reports.Any(m => m.Id == Id))
            {
                return RedirectToAction("Index", "Reports");
            }

            var report = _context.Reports.Find(Id);

            var sections = _context.Sections
                .OrderBy(m => m.SortOrder)
                .ToList();

            var reportDetailsForm = new ReportDetailsFormViewModel();

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

                // create the gird
                var grid = new FormGridViewModel();

                if (columns.Count() > 0 && questions.Count() > 0)
                {
                    // add columns to grid's columns property 
                    foreach (var c in columns)
                    {
                        if (c.ColumnType == "standard")
                        {
                            grid.Columns.Add(c);
                        }
                    }

                    // set multi-column flag
                    grid.MultiColumn = (columns.Count() > 1) ? true : false;

                    // add question rows to grid's questions property
                    foreach (var q in questions)
                    {
                        var question = new FormGridQuestionViewModel();

                        question.QuestionTitle = q.Title;

                        // add fields to qustion row's fields property
                        foreach (var c in columns)
                        {
                            if (c.ColumnType == "standard")
                            {
                                var field = new FormGridQuestionFieldViewModel();

                                field.QuestionId = q.Id;
                                field.ColumnId = c.Id;

                                // add field data
                                var FieldData = _context.Fields
                                    .Include(m => m.FieldOptions)
                                    .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
                                    .SingleOrDefault();

                                if (FieldData != null)
                                {
                                    field.Id = FieldData.Id;
                                    field.DataType = FieldData.DataType;
                                    field.InputType = FieldData.InputType;
                                    field.FieldOptions = FieldData.FieldOptions;

                                    // set YesNoOptions for yes/no field
                                    if (field.DataType == "yesno")
                                    {
                                        if (FieldData.YesNoDefaultCaption != null || FieldData.YesNoDefaultCaption != "")
                                        {
                                            field.YesNoOptions = SetYesNoOptions(FieldData.YesNoDefaultCaption);
                                        }

                                        // set the target section for some of the yes/no fields to activate / deactivate
                                        var myFieldIds = new List<long>() { 281, 282, 283, 284, 285, 286, 287 };
                                        var myTargetSections = new Dictionary<long, long>()
                                        {
                                            { 281, 12 }, { 282, 8 }, { 283, 11 }, { 284, 17 }, { 285, 13 }, { 286, 10 }, { 287, 18 }
                                        };

                                        if (myFieldIds.Contains(FieldData.Id))
                                        {
                                            field.TargetSection = myTargetSections[FieldData.Id];
                                        }
                                    }
                                    // set required attribute of if field's IsRequired is true
                                    field.RequiredAttr = (FieldData.IsRequired) ? "required=\"required\"" : "";
                                }

                                // add text field value
                                if (field.DataType == "text")
                                {
                                    var textValue = _context.TextValues
                                        .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                        .SingleOrDefault();

                                    field.Data = (textValue != null) ? textValue.Data : null;
                                }

                                // add number field value
                                if (field.DataType == "number" || field.DataType == "yesno")
                                {
                                    var numberValue = _context.NumberValues
                                        .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                        .SingleOrDefault();

                                    field.Data = (numberValue != null) ? numberValue.Data.ToString() : null;
                                }

                                // add date field value
                                if (field.DataType == "date")
                                {
                                    var dateValue = _context.DateValues
                                        .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                        .SingleOrDefault();

                                    field.Data = (dateValue != null) ? String.Format("{0:MM/dd/yyyy}", dateValue.Data) : null;
                                }

                                question.Fields.Add(field);
                            }

                        }

                        grid.Questions.Add(question);
                    }
                }

                // add section grid to details-form's section-grid property
                reportDetailsForm.SectionGrids.Add(new SectionGridViewModel
                {
                    Id = section.Id,
                    Title = section.Title,
                    Description = section.Description,
                    Grid = grid
                });

                // report id to report details
                reportDetailsForm.ReportId = report.Id;
            }

            ViewBag.reportId = report.Id;

            return View(reportDetailsForm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReportDetails(IFormCollection form)
        {
            if (form["ReportId"] != "")
            {
                var textFields = _context.Fields
                    .Where(m => m.DataType == "text")
                    .ToList();

                var numberFields = _context.Fields
                    .Where(m => m.DataType == "number" || m.DataType == "yesno")
                    .ToList();

                var dateFields = _context.Fields
                    .Where(m => m.DataType == "date")
                    .ToList();

                // save fields with text datatype
                foreach (var field in textFields)
                {
                    // if the value item doesn't exist, add one
                    if (!_context.TextValues.Any(m => m.FieldId == field.Id && m.ReportId == form["ReportId"]))
                    {
                        var TV = new TextValue
                        {
                            FieldId = field.Id,
                            ReportId = form["ReportId"],
                            Data = null
                        };

                        _context.TextValues.Add(TV);
                        _context.SaveChanges();
                    }

                    if (_context.TextValues.Any(m => m.FieldId == field.Id && m.ReportId == form["ReportId"]))
                    {
                        var textValue = _context.TextValues
                            .Where(m => m.FieldId == field.Id && m.ReportId == form["ReportId"])
                            .SingleOrDefault();

                        var formInputValue = form[field.Id.ToString()];

                        if (!String.IsNullOrWhiteSpace(formInputValue))
                        {
                            textValue.Data = Convert.ToString(formInputValue);
                        }
                        else
                        {
                            textValue.Data = null;
                        }

                        _context.Entry(textValue).State = EntityState.Modified;
                    }
                    
                }

                _context.SaveChanges();

                // this variable hold a list of the Ids of the disabled fields
                List<long> disabledFieldIds = new List<long>();

                // save fields with numer datatype
                foreach (var field in numberFields)
                {
                    // if the value item doesn't exist, add one
                    if (!_context.NumberValues.Any(m => m.FieldId == field.Id && m.ReportId == form["ReportId"]))
                    {
                        var NV = new NumberValue
                        {
                            FieldId = field.Id,
                            ReportId = form["ReportId"],
                            Data = null
                        };

                        _context.NumberValues.Add(NV);
                        _context.SaveChanges();
                    }

                    long? formInputValue = null;

                    if (!String.IsNullOrWhiteSpace(form[field.Id.ToString()]))
                    {
                        formInputValue = Convert.ToInt64(form[field.Id.ToString()]);
                    }

                    // check if field is yes/no and the value selected is not 1 = "Yes" if 
                    // so then add value 2 = "N/A" for its target yes/no input fields
                    if (field.DataType == "yesno" && formInputValue != 1)
                    {
                        var myFieldIds = new List<long>() { 281, 282, 283, 284, 285, 286, 287 };
                        var myTargetSections = new Dictionary<long, long>()
                        {
                            { 281, 12 }, { 282, 8 }, { 283, 11 }, { 284, 17 }, { 285, 13 }, { 286, 10 }, { 287, 18 }
                        };

                        if (myFieldIds.Contains(field.Id))
                        {
                            var sectionId = myTargetSections[field.Id];

                            var questions = _context.Questions
                                .Include(m => m.Fields)
                                .Where(m => m.SectionId == sectionId)
                                .ToList();

                            foreach (var Q in questions)
                            {
                                foreach (var F in Q.Fields)
                                {
                                    if (_context.NumberValues.Any(m => m.FieldId == F.Id && m.ReportId == form["ReportId"]))
                                    {
                                        disabledFieldIds.Add(F.Id);

                                        //var nValue = _context.NumberValues
                                        //    .Where(m => m.FieldId == F.Id && m.ReportId == form["ReportId"])
                                        //    .SingleOrDefault();

                                        //nValue.Data = 2;

                                        //_context.Entry(nValue).State = EntityState.Modified;
                                    }
                                }
                            }

                            //_context.NumberValues
                            //    .Where(m => disabledFieldIds.Contains(m.FieldId) && m.ReportId == form["ReportId"])
                            //    .ToList()
                            //    .ForEach(m => m.Data = 2);

                            //_context.SaveChanges();

                            //using(var ctx = new WebNutContext())
                            //{

                            //}
                        }
                    }

                    var numberValue = _context.NumberValues
                        .Where(m => m.FieldId == field.Id && m.ReportId == form["ReportId"])
                        .SingleOrDefault();

                    numberValue.Data = formInputValue;

                    _context.Entry(numberValue).State = EntityState.Modified;
                }

                _context.SaveChanges();

                // set the  values of the disabled yes/no fields
                var sql = @"UPDATE [NumberValues] SET [Data] = 2 WHERE FieldId IN (";

                for (int i = 0; i < disabledFieldIds.Count(); i++)
                {
                    sql += disabledFieldIds[i];

                    if (i < disabledFieldIds.Count - 1)
                    {
                        sql += ",";
                    }
                    else
                    {
                        sql += ")";
                    }
                }

                sql += " AND ReportId = " + form["ReportId"];

                _context.Database.ExecuteSqlCommand(sql);

                // save fields with date datatype
                foreach (var field in dateFields)
                {
                    // if the value item doesn't exist, add one
                    if (!_context.DateValues.Any(m => m.FieldId == field.Id && m.ReportId == form["ReportId"]))
                    {
                        var DV = new DateValue
                        {
                            FieldId = field.Id,
                            ReportId = form["ReportId"],
                            Data = null
                        };

                        _context.DateValues.Add(DV);
                        _context.SaveChanges();
                    }

                    if (_context.DateValues.Any(m => m.FieldId == field.Id && m.ReportId == form["ReportId"]))
                    {
                        var dateValue = _context.DateValues
                            .Where(m => m.FieldId == field.Id && m.ReportId == form["ReportId"])
                            .SingleOrDefault();

                        var formInputValue = form[field.Id.ToString()];

                        if (!String.IsNullOrWhiteSpace(formInputValue))
                        {
                            dateValue.Data = DateTime.ParseExact(form[field.Id.ToString()], "MM/dd/yyyy", null);
                        }
                        else
                        {
                            dateValue.Data = null;
                        }

                        _context.Entry(dateValue).State = EntityState.Modified;
                    }
                }

                _context.SaveChanges();

                return RedirectToAction("Index", "Reports");
            }
            else
            {
                return RedirectToAction("Index", "Reports");
            }
        }

        public IActionResult ShowReport(string Id)
        {
            // if report doesn't exist redirect to reports index page
            if (!_context.Reports.Any(m => m.Id == Id))
            {
                return RedirectToAction("Index", "Reports");
            }

            // Get report lookup names
            var lookupreport = _context.Reports.Where(m => m.Id.Equals(Id)).FirstOrDefault();
            String ProvinceName, DistrictName, FacilityName, ImplementerName, FacilityTypeName;
            ProvinceName = _context.Provinces.Where(w => w.ProvCode.Equals(lookupreport.ProvinceId)).Select(m => m.ProvName).FirstOrDefault();
            DistrictName = _context.Districts.Where(w => w.DistCode.Equals(lookupreport.DistrictId)).Select(m => m.DistName).FirstOrDefault();
            FacilityName = _context.FacilityInfo.Where(w => w.FacilityId.Equals(lookupreport.FacilityId)).Select(m => m.FacilityName).FirstOrDefault();
            ImplementerName = _context.Implementers.Where(w => w.ImpCode.Equals(lookupreport.ImpId)).Select(m => m.ImpAcronym).FirstOrDefault();
            FacilityTypeName = _context.FacilityTypes.Where(w => w.FacTypeCode.Equals(lookupreport.FacilityTypeId)).Select(m => m.TypeAbbrv).FirstOrDefault();

            ViewBag.ProvinceName = ProvinceName;
            ViewBag.DistrictName = DistrictName;
            ViewBag.FacilityName = FacilityName;
            ViewBag.ImplementerName = ImplementerName;
            ViewBag.FacilityTypeName = FacilityTypeName;

            var report = _context.Reports
                .Include(m => m.DataForm)
                .Where(m => m.Id == Id)
                .SingleOrDefault();

            var sections = _context.Sections
                .OrderBy(m => m.SortOrder)
                .ToList();

            var reportDetailsForm = new ReportDetailsFormViewModel();

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

                // create the gird
                var grid = new FormGridViewModel();

                if (columns.Count() > 0 && questions.Count() > 0)
                {
                    // add columns to grid's columns property 
                    foreach (var c in columns)
                    {
                        grid.Columns.Add(c);
                    }

                    // set multi-column flag
                    grid.MultiColumn = (columns.Count() > 1) ? true : false;

                    // add question rows to grid's questions property
                    foreach (var q in questions)
                    {
                        var question = new FormGridQuestionViewModel();

                        question.QuestionTitle = q.Title;

                        // add fields to qustion row's fields property
                        foreach (var c in columns)
                        {
                            var field = new FormGridQuestionFieldViewModel();

                            field.QuestionId = q.Id;
                            field.ColumnId = c.Id;

                            // check if column is of type standard
                            if (c.ColumnType == "standard")
                            {
                                // add field data 
                                var FieldData = _context.Fields
                                    .Include(m => m.FieldOptions)
                                    .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
                                    .SingleOrDefault();

                                if (FieldData != null)
                                {
                                    field.Id = FieldData.Id;
                                    field.DataType = FieldData.DataType;
                                    field.InputType = FieldData.InputType;
                                    field.FieldOptions = FieldData.FieldOptions;

                                    // set YesNoOptions for yes/no field
                                    if (field.DataType == "yesno")
                                    {
                                        if (FieldData.YesNoDefaultCaption != null || FieldData.YesNoDefaultCaption != "")
                                        {
                                            field.YesNoOptions = SetYesNoOptions(FieldData.YesNoDefaultCaption);
                                        }
                                    }
                                }

                                // add text field value
                                if (field.DataType == "text")
                                {
                                    var textValue = _context.TextValues
                                        .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                        .SingleOrDefault();

                                    if (field.InputType == "dropdown" && field.DataType != "yesno")
                                    {
                                        foreach (var fo in field.FieldOptions)
                                        {
                                            if (fo.Value == textValue.Data)
                                            {
                                                field.Data = fo.Caption;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        field.Data = (textValue != null) ? textValue.Data : null;
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
                                            foreach (var fo in field.FieldOptions)
                                            {
                                                if (fo.Value == numberValue.Data.ToString())
                                                {
                                                    field.Data = fo.Caption;
                                                }
                                            }
                                        }
                                        else if (field.DataType == "yesno")
                                        {
                                            foreach (var yo in field.YesNoOptions)
                                            {
                                                if (yo.Key == numberValue.Data)
                                                {
                                                    field.Data = yo.Value;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            field.Data = (numberValue != null) ? numberValue.Data.ToString() : null;
                                        }
                                    }
                                }

                                // add date field value
                                if (field.DataType == "date")
                                {
                                    var dateValue = _context.DateValues
                                        .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
                                        .SingleOrDefault();

                                    field.Data = (dateValue != null) ? String.Format("{0:MMM dd, yyyy}", dateValue.Data) : null;
                                }
                            }
                            else
                            {
                                // check if column is of type percentage then 
                                // calculate the percentage data and set on field.Data property
                                if (c.ColumnType == "percentage")
                                {
                                    var dividendField = _context.Fields
                                        .SingleOrDefault(m => m.QuestionId == q.Id && m.ColumnId == c.DividendColumn).Id;

                                    var divisorField = _context.Fields
                                        .SingleOrDefault(m => m.QuestionId == q.Id && m.ColumnId == c.DivisorColumn).Id;

                                    long? dividendValue = null;
                                    long? divisorValue = null;

                                    if (_context.NumberValues.Any(m => m.FieldId == dividendField && m.ReportId == report.Id))
                                    {
                                        dividendValue = _context.NumberValues.SingleOrDefault(m => m.FieldId == dividendField && m.ReportId == report.Id).Data;
                                    }

                                    if (_context.NumberValues.Any(m => m.FieldId == divisorField && m.ReportId == report.Id))
                                    {
                                        divisorValue = _context.NumberValues.SingleOrDefault(m => m.FieldId == divisorField && m.ReportId == report.Id).Data;
                                    }

                                    if (dividendValue != null && divisorValue != null)
                                    {
                                        field.Data = Math.Ceiling((double)dividendValue * 100.0 / (double)divisorValue).ToString();
                                    }
                                    else
                                    {
                                        field.Data = null;
                                    }
                                    
                                    field.IsPercentageValue = true;
                                }
                            }
                            question.Fields.Add(field);
                        }

                        grid.Questions.Add(question);
                    }
                }

                // add section grid to details-form's section-grid property
                reportDetailsForm.SectionGrids.Add(new SectionGridViewModel
                {
                    Id = section.Id,
                    Title = section.Title,
                    Description = section.Description,
                    Grid = grid
                });

                // report id to report details
                reportDetailsForm.ReportId = report.Id;

                // add report main data 
                reportDetailsForm.Report = report;
            }

            return View(reportDetailsForm);
        }

        public IActionResult Complete(string Id)
        {
            var report = _context.Reports.Find(Id);

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Complete(StringId dto)
        {
            var report = _context.Reports.Find(dto.Id);

            report.IsCompleted = true;

            _context.Entry(report).State = EntityState.Modified;

            _context.SaveChanges();

            return RedirectToAction("Index", "Reports");
        }

        public IActionResult Delete(string Id)
        {
            var report = _context.Reports.Find(Id);

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(StringId dto)
        {
            var report = _context.Reports.Find(dto.Id);

            _context.Reports.Remove(report);
            _context.SaveChanges();

            return RedirectToAction("Index", "Reports");
        }

        public int WeekNumber(DateTime value)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        protected Dictionary<byte, string> SetYesNoOptions(string defaultCaption = "")
        {
            var caption = (defaultCaption != null && defaultCaption != "") ? defaultCaption : "N/A";

            return new Dictionary<byte, string>
            {
                { 1, "Yes" },
                { 0, "No" },
                { 2, caption }
            };
        }
    }
}
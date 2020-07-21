using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataSystem.Models;
using DataSystem.Models.GLM;
using Microsoft.AspNetCore.Identity;
using DataSystem.GLM.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http;
using System.Net;


namespace DataSystem.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportApiController : ControllerBase
    {
        protected readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ReportApiController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //[Route("Index")]
        //public async Task<ActionResult<IEnumerable<Report>>> Index()
        //{
        //    var Report = await _context.Reports
        //        .Include(m => m.Dataform)
        //        .OrderByDescending(m => m.Id)
        //        .ToListAsync();

        //    return Ok(new
        //    {
        //        Report = Report
        //    });
        //}

        //[HttpGet]
        //[Route("Create")]
        //public async Task<ActionResult> Create()
        //{
        //    return Ok(new
        //    {
        //        DataForms = await _context.DataForms.ToListAsync()
        //    });
        //}

        //[HttpPost]
        //[Route("Create")]
        //public async Task<ActionResult> Create([FromForm] Report form)
        //{
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);

        //    // generate report id based on facility id, year, report frequency and data form id
        //    string Id = "";

        //    // get todays date in number of days
        //    var datetime = Convert.ToDateTime(form.ReportPreparedDate);
        //    TimeSpan ts = (new DateTime(datetime.Year, datetime.Month, datetime.Day) - new DateTime(1, 1, 1));
        //    var days = (ts.TotalDays + 1).ToString();

        //    // pad facility id with zeros
        //    var facilityId = Convert.ToInt64(form.FacilityId).ToString("D6");

        //    // concatenate all the parts
        //    Id += form.DataFormId.ToString();
        //    Id += "-" + facilityId;
        //    Id += "-" + days;

        //    // check if report id is usnique 
        //    if (_context.Reports.Any(m => m.Id == Id))
        //    {
        //        return Ok(new
        //        {
        //            Message = new { Status = "error", Text = "This report already exists." },
        //            DataForms = await _context.DataForms.ToListAsync(),
        //        });
        //    }
        //    var facility = _context.FacilityInfo.Where(m => m.FacilityId.Equals(facilityId)).FirstOrDefault();

        //    var report = new Report()
        //    {
        //        Id = Id,
        //        ProvinceId = form.ProvinceId,
        //        DistrictId = form.DistrictId,
        //        FacilityId = form.FacilityId,
        //        FacilityTypeId = form.FacilityTypeId,
        //        ImpId = form.ImpId,
        //        DataCollectorOffice = form.DataCollectorOffice,
        //        ReportedBy = form.ReportedBy,
        //        ReportPreparedDate = form.ReportPreparedDate,
        //        ReportReceivedDate = form.ReportReceivedDate,
        //        DataFormId = form.DataFormId,
        //        TenantId = user.TenantId,
        //        UserName = user.UserName,
        //        UpdateDate = DateTime.Now.Date
        //    };

        //    _context.Reports.Add(report);

        //    _context.SaveChanges();

        //    // set report detail fields

        //    var ReportId = report.Id;

        //    var textFields = _context.Fields
        //            .Where(m => m.DataType == "text" || m.DataType == "yesno")
        //            .ToList();

        //    var numberFields = _context.Fields
        //        .Where(m => m.DataType == "number")
        //        .ToList();

        //    var dateFields = _context.Fields
        //        .Where(m => m.DataType == "date")
        //        .ToList();

        //    // add empty fields with text datatype
        //    foreach (var field in textFields)
        //    {
        //        _context.TextValues.Add(new TextValue
        //        {
        //            ReportId = Id,
        //            FieldId = field.Id,
        //            Data = null
        //        });
        //    }

        //    _context.SaveChanges();

        //    // add empty fields with number datatype
        //    foreach (var field in numberFields)
        //    {
        //        _context.NumberValues.Add(new NumberValue
        //        {
        //            ReportId = Id,
        //            FieldId = field.Id,
        //            Data = null
        //        });
        //    }

        //    _context.SaveChanges();

        //    // add empty fields with date datatype
        //    foreach (var field in dateFields)
        //    {
        //        _context.DateValues.Add(new DateValue
        //        {
        //            ReportId = Id,
        //            FieldId = field.Id,
        //            Data = null
        //        });
        //    }

        //    _context.SaveChanges();

        //    return Ok(new
        //    {
        //        Message = new { Status = "success", Text = "Report added successfully." },
        //        ReportId = Id
        //    });
        //}

        //[HttpGet("{Id}")]
        //[Route("Edit/{Id}")]
        //public async Task<ActionResult> Edit(string Id)
        //{
        //    var report = _context.Reports.Find(Id);
        //    var DataForms = await _context.DataForms.ToListAsync();

        //    return Ok(new
        //    {
        //        Report = report,
        //        DataForms = DataForms
        //    });
        //}

        //[HttpPost]
        //[Route("Edit")]
        //public async Task<ActionResult> Edit([FromForm] Report form)
        //{
        //    var user = await _userManager.FindByNameAsync(User.Identity.Name);

        //    var report = _context.Reports.Find(form.Id);

        //    report.ProvinceId = form.ProvinceId;
        //    report.DistrictId = form.DistrictId;
        //    report.FacilityId = form.FacilityId;
        //    report.ReportedBy = form.ReportedBy;
        //    report.FacilityTypeId = (int)form.FacilityTypeId;
        //    report.ReportPreparedDate = form.ReportPreparedDate;
        //    report.ReportReceivedDate = form.ReportReceivedDate;
        //    report.DataCollectorOffice = form.DataCollectorOffice;

        //    report.DataFormId = form.DataFormId;
        //    report.TenantId = (int)report.TenantId;
        //    report.UserName = user.UserName;
        //    report.UpdateDate = DateTime.Now.Date;
        //    report.ImpId = form.ImpId;

        //    _context.Entry(report).State = EntityState.Modified;

        //    _context.SaveChanges();

        //    return Ok(new
        //    {
        //        Message = new { Status = "success", Text = "Report updated successfully." }
        //    });
        //}

        //[HttpGet("{Id}")]
        //[Route("ReportDetails/{Id}")]
        //public async Task<ActionResult> ReportDetails(string Id)
        //{
        //    // if report doesn't exist return error
        //    if (!_context.Reports.Any(m => m.Id == Id))
        //    {
        //        return Ok(new
        //        {
        //            Message = new { Status = "error", Text = "Report not found." }
        //        });
        //    }

        //    var report = _context.Reports.Find(Id);

        //    var sections = await _context.Sections
        //        .OrderBy(m => m.SortOrder)
        //        .ToListAsync();

        //    var reportDetailsForm = new ReportDetailsForm();

        //    foreach (var section in sections)
        //    {
        //        var columns = await _context.Columns
        //            .Where(m => m.SectionId == section.Id)
        //            .OrderBy(m => m.SortOrder)
        //            .ToListAsync();

        //        var questions = await _context.Questions
        //            .Where(m => m.SectionId == section.Id)
        //            .OrderBy(m => m.SortOrder)
        //            .ToListAsync();

        //        // create the gird
        //        var grid = new FormGrid();

        //        if (columns.Count() > 0 && questions.Count() > 0)
        //        {
        //            // add columns to grid's columns property 
        //            foreach (var c in columns)
        //            {
        //                grid.Columns.Add(c);
        //            }

        //            // set multi-column flag
        //            grid.MultiColumn = (columns.Count() > 1) ? true : false;

        //            // add question rows to grid's questions property
        //            foreach (var q in questions)
        //            {
        //                var question = new FormGridQuestion();

        //                question.QuestionTitle = q.Title;

        //                // add fields to qustion row's fields property
        //                foreach (var c in columns)
        //                {
        //                    var field = new FormGridQuestionField();

        //                    field.QuestionId = q.Id;
        //                    field.ColumnId = c.Id;

        //                    // add field data
        //                    var FieldData = await _context.Fields
        //                        .Include(m => m.FieldOptions)
        //                        .Where(m => m.QuestionId == q.Id && m.ColumnId == c.Id)
        //                        .SingleOrDefaultAsync();

        //                    if (FieldData != null)
        //                    {
        //                        field.Id = FieldData.Id;
        //                        field.DataType = FieldData.DataType;
        //                        field.InputType = FieldData.InputType;
        //                        field.FieldOptions = FieldData.FieldOptions;
        //                    }

        //                    // add text field value
        //                    if (field.DataType == "text" || field.DataType == "yesno")
        //                    {
        //                        var textValue = await _context.TextValues
        //                            .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
        //                            .SingleOrDefaultAsync();

        //                        field.Data = (textValue != null) ? textValue.Data : null;
        //                    }

        //                    // add number field value
        //                    if (field.DataType == "number")
        //                    {
        //                        var numberValue = await _context.NumberValues
        //                            .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
        //                            .SingleOrDefaultAsync();

        //                        field.Data = (numberValue != null) ? numberValue.Data.ToString() : null;
        //                    }

        //                    // add date field value
        //                    if (field.DataType == "date")
        //                    {
        //                        var dateValue = await _context.DateValues
        //                            .Where(m => m.ReportId == report.Id && m.FieldId == field.Id)
        //                            .SingleOrDefaultAsync();

        //                        field.Data = (dateValue != null) ? String.Format("{0:MM/dd/yyyy}", dateValue.Data) : null;
        //                    }

        //                    question.Fields.Add(field);
        //                }

        //                grid.Questions.Add(question);
        //            }
        //        }

        //        // add section grid to details-form's section-grid property
        //        reportDetailsForm.SectionGrids.Add(new SectionGrid
        //        {
        //            Title = section.Title,
        //            Description = section.Description,
        //            Grid = grid
        //        });

        //        // report id to report details
        //        reportDetailsForm.ReportId = report.Id;
        //    }

        //    return Ok(new
        //    {
        //        Form = reportDetailsForm
        //    });
        //}

        //[HttpPost]
        //[Route("ReportDetails")]
        //public async Task<ActionResult> ReportDetails([FromForm] IFormCollection form)
        //{
        //    if (form["ReportId"] != "")
        //    {
        //        var textFields = await _context.Fields
        //            .Where(m => m.DataType == "text" || m.DataType == "yesno")
        //            .ToListAsync();

        //        var numberFields = await _context.Fields
        //            .Where(m => m.DataType == "number")
        //            .ToListAsync();

        //        var dateFields = await _context.Fields
        //            .Where(m => m.DataType == "date")
        //            .ToListAsync();

        //        // save fields with text datatype
        //        foreach (var field in textFields)
        //        {
        //            var textValue = await _context.TextValues
        //                .Where(m => m.FieldId == field.Id && m.ReportId == form["ReportId"])
        //                .SingleOrDefaultAsync();

        //            var formInputValue = form[field.Id.ToString()];

        //            if (!String.IsNullOrWhiteSpace(formInputValue))
        //            {
        //                textValue.Data = Convert.ToString(formInputValue);
        //            }
        //            else
        //            {
        //                textValue.Data = null;
        //            }

        //            _context.Entry(textValue).State = EntityState.Modified;
        //        }

        //        _context.SaveChanges();

        //        // save fields with numer datatype
        //        foreach (var field in numberFields)
        //        {
        //            var numberValue = await _context.NumberValues
        //                .Where(m => m.FieldId == field.Id && m.ReportId == form["ReportId"])
        //                .SingleOrDefaultAsync();

        //            var formInputValue = form[field.Id.ToString()];

        //            if (!String.IsNullOrWhiteSpace(formInputValue))
        //            {
        //                numberValue.Data = Convert.ToInt64(formInputValue);
        //            }
        //            else
        //            {
        //                numberValue.Data = null;
        //            }

        //            _context.Entry(numberValue).State = EntityState.Modified;
        //        }

        //        _context.SaveChanges();

        //        // save fields with date datatype
        //        foreach (var field in dateFields)
        //        {
        //            var dateValue = await _context.DateValues
        //                .Where(m => m.FieldId == field.Id && m.ReportId == form["ReportId"])
        //                .SingleOrDefaultAsync();

        //            var formInputValue = form[field.Id.ToString()];

        //            if (!String.IsNullOrWhiteSpace(formInputValue))
        //            {
        //                dateValue.Data = DateTime.ParseExact(form[field.Id.ToString()], "MM/dd/yyyy", null);
        //            }
        //            else
        //            {
        //                dateValue.Data = null;
        //            }

        //            _context.Entry(dateValue).State = EntityState.Modified;
        //        }

        //        _context.SaveChanges();

        //        return Ok(new
        //        {
        //            Message = new { Status = "success", Text = "Report Details updated successfully." }
        //        });
        //    }
        //    else
        //    {
        //        return Ok(new
        //        {
        //            Message = new { Status = "error", Text = "Report not found." }
        //        });
        //    }
        //}

        //[HttpPost]
        //[Route("Delete")]
        //public async Task<ActionResult> Delete([FromForm] StringId dto)
        //{
        //    // if report doesn't exist return error
        //    if (!_context.Reports.Any(m => m.Id == dto.Id))
        //    {
        //        return Ok(new
        //        {
        //            Message = new { Status = "error", Text = "Report not found." }
        //        });
        //    }

        //    var report = await _context.Reports.FindAsync(dto.Id);

        //    _context.Reports.Remove(report);
        //    _context.SaveChanges();

        //    return Ok(new
        //    {
        //        Message = new { Status = "success", Text = "Report deleted successfully." }
        //    });
        //}




        //[Authorize]
        //public HttpResponseMessage Get(string ID)
        //{
        //    Report report = _context.Reports.Where(r => r.Id == ID).FirstOrDefault();

        //    if (report == null)
        //    {
        //        return HttpContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, "Couldn't Find any Report with ID: " + ID);
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Found, report);
        //    }

        //}


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Report>>> Get()
        {
            List<Report> report = _context.Reports.ToList();

            if (report.Count == 0)
            {
                //return NotFound("Report not found");
                return Ok(new
                {
                    report = report
                });
            }
            else
            {
                return Ok(new
                {
                    report = report
                });
            }

       }




        //[HttpPost]
        //public HttpResponseMessage Post([FromBody] List<Report> Report)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Request Body is not in the right Format");
        //    }
        //    Boolean result = true;

        //    foreach (var report in Report)
        //    {
        //        if (this.ReportExists(report.ReportId))
        //        {
        //            _context.Entry(report).State = EntityState.Modified;

        //            try
        //            {
        //                _context.SaveChanges();
        //            }
        //            catch (Exception)
        //            {
        //                result = false;
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                _context.Reports.Add(report);
        //                _context.SaveChanges();
        //            }
        //            catch (Exception)
        //            {
        //                result = false;
        //            }
        //        }
        //    }


        //    if (result == false)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, "Some Report Update Might Have Failed");
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.Created, "Report Created/Updated");
        //    }
        //}

        //private bool ReportExists(long id)
        //{
        //    return _context.Reports.Any(e => e.ReportId == id);
        //}

    }
}